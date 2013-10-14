// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see licence.txt in the main folder

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Aura.Shared.Const;
using Aura.Shared.Network;
using Aura.Shared.Util;
using Aura.World.Events;
using Aura.World.Scripting;
using Aura.World.Skills;
using Aura.World.Util;
using Aura.World.World;
using Aura.World.World.Guilds;

namespace Aura.World.Network
{
	public partial class WorldServer : BaseServer<WorldClient>
	{
		public static readonly WorldServer Instance = new WorldServer();
		static WorldServer() { }
		private WorldServer() : base() { }

		/// <summary>
		/// Milliseconds between tries to connect.
		/// </summary>
		private const int LoginTryTime = 10 * 1000;

		public WorldClient LoginServer { get; protected set; }
		private Timer _shutdownTimer1, _shutdownTimer2;

		private readonly Dictionary<string, MabiServer> ServerList = new Dictionary<string, MabiServer>();

		public override void Run(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

			ServerUtil.WriteHeader("World Server", ConsoleColor.DarkGreen);
			Console.Title = "* " + Console.Title;

			// Logger
			// --------------------------------------------------------------
			if (!Directory.Exists("../../logs/"))
				Directory.CreateDirectory("../../logs/");
			Logger.FileLog = "../../logs/world.txt";

			Logger.Info("Initializing server @ " + DateTime.Now);

			// Configuration
			// --------------------------------------------------------------
			Logger.Info("Reading configuration...");
			try
			{
				WorldConf.Load(args);
			}
			catch (FileNotFoundException)
			{
				Logger.Warning("Sorry, I couldn't find 'conf/world.conf'.");
			}
			catch (Exception ex)
			{
				Logger.Warning("There has been a problem while reading 'conf/world.conf'.");
				Logger.Exception(ex);
			}

			// Logger display filter
			// --------------------------------------------------------------
			Logger.Hide = WorldConf.ConsoleFilter;

			// Cache
			// --------------------------------------------------------------
			if (!this.InitCache())
			{
				Logger.Error("Unable to create cache folder at '{0}'.", WorldConf.CachePath);
				ServerUtil.Exit(1);
			}

			// Security checks
			// --------------------------------------------------------------
			ServerUtil.CheckInterPassword(WorldConf.Password);

			// Localization
			// --------------------------------------------------------------
			Logger.Info("Loading localization files (" + WorldConf.Localization + ")...");
			try
			{
				Localization.Parse(WorldConf.DataPath + "/localization/" + WorldConf.Localization);
			}
			catch (FileNotFoundException ex)
			{
				Logger.Warning("Unable to load localization: " + ex.Message);
			}

			// Database
			// --------------------------------------------------------------
			Logger.Info("Connecting to database...");
			ServerUtil.TryConnectToDatabase(WorldConf.DatabaseHost, WorldConf.DatabaseUser, WorldConf.DatabasePass, WorldConf.DatabaseDb);
			Global.Init();

			//Logger.Info("Clearing database cache...");
			//MabiDb.Instance.ClearDatabaseCache();

			// Data
			// --------------------------------------------------------------
			Logger.Info("Loading data files...");
			ServerUtil.LoadData(WorldConf.DataPath);

            // Named spawns
            // --------------------------------------------------------------
            Logger.Info("Loading named spawns...");
            Spawner.Load(WorldConf.DataPath);

			// Guilds
			// --------------------------------------------------------------
			Logger.Info("Loading guilds...");
			GuildManager.LoadGuildStones();

			// Commands
			// --------------------------------------------------------------
			Logger.Info("Loading commands...");
			CommandHandler.Instance.Load();

			// Scripts (NPCs, Portals, etc.)
			// --------------------------------------------------------------
			ScriptManager.Instance.LoadScripts();

			// Monsters
			// --------------------------------------------------------------
			Logger.Info("Spawning monsters...");
			ScriptManager.Instance.LoadSpawns();

			// Setting up weather
			// --------------------------------------------------------------
			Logger.Info("Initializing weather...");
			WeatherManager.Instance.Init();

			// Skills
			// --------------------------------------------------------------
			Logger.Info("Initializing skills...");
			SkillManager.Init();

			// World
			// --------------------------------------------------------------
			WorldManager.Instance.Start();

			// Starto
			// --------------------------------------------------------------
			try
			{
				this.StartListening(new IPEndPoint(IPAddress.Any, WorldConf.ChannelPort));

				Logger.Status("World Server ready, listening on " + _serverSocket.LocalEndPoint.ToString());

				Console.Title = Console.Title.Replace("* ", "");
			}
			catch (Exception ex)
			{
				Logger.Exception(ex, "Unable to set up socket; perhaps you're already running a server?");
				ServerUtil.Exit(1);
			}

			// Login server
			// --------------------------------------------------------------
			this.ConnectToLogin(true);

			// Run the channel register method once, and then subscribe to the event that's run once per minute.
			this.SendChannelStatus(MabiTime.Now);
			EventManager.TimeEvents.RealTimeTick += this.SendChannelStatus;

			// Console commands
			// --------------------------------------------------------------
			Logger.Info("Type 'help' for a list of console commands.");
			ServerUtil.ReadCommands(this.ParseCommand);
		}

		protected void ParseCommand(string[] args, string command)
		{
			switch (args[0])
			{
				case "help":
					{
						Logger.Info("Commands:");
						Logger.Info("  status       Shows some status information about the channel");
						Logger.Info("  shutdown     Announces and executes server shutdown");
						Logger.Info("  help         Shows this");
					}
					break;

				case "status":
					{
						Logger.Info("Creatures: " + WorldManager.Instance.GetCreatureCount());
						Logger.Info("Items: " + WorldManager.Instance.GetItemCount());
						Logger.Info("Online time: " + (DateTime.Now - _startTime).ToString(@"hh\:mm\:ss"));
					}
					break;

				case "shutdown":
					{
						this.StopListening();

						// Seconds till players are dced, 10s min.
						int dcSeconds = 10;
						if (args.Length > 1)
							int.TryParse(args[1], out dcSeconds);
						if (dcSeconds < 10)
							dcSeconds = 10;

						// Seconds till the server shuts down.
						int exitSeconds = dcSeconds + 30;

						// Broadcast a notice.
						Send.ChannelNotice(NoticeType.TopRed, Localization.Get("world.shutdown"), dcSeconds); // The server will shutdown in {0} seconds, [...]

						// Set a timer when to send the dc request all remaining players.
						_shutdownTimer1 = new Timer(_ =>
						{
							var dc = new MabiPacket(Op.RequestClientDisconnect, Id.World).PutSInt((dcSeconds - (dcSeconds - 10)) * 1000);
							WorldManager.Instance.Broadcast(dc, SendTargets.All, null);
						},
							null, (dcSeconds - 10) * 1000, Timeout.Infinite
						);
						Logger.Info("Disconnecting players in " + dcSeconds + " seconds.");

						// Set a timer when to exit this server.
						_shutdownTimer2 = new Timer(_ =>
						{
							Global.RegularVarSave(null, null);
							ServerUtil.Exit(0, false);
						},
							null, exitSeconds * 1000, Timeout.Infinite
						);
						Logger.Info("Shutting down in " + exitSeconds + " seconds.");
					}
					break;

				case "":
					break;

				default:
					Logger.Info("Unkown command.");
					goto case "help";
			}
		}

		void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			try
			{
				Logger.Error("Oh no! Ferghus escaped his memory block and infected the rest of the server!\r\nAura has encountered an unexpected and unrecoverable error. We're going to try to save as much as we can.");
			}
			catch { }
			try
			{
				this.StopListening();
			}
			catch { }
			try
			{
				WorldManager.Instance.EmergencyShutdown();
			}
			catch { }
			try
			{
				Logger.Exception((Exception)e.ExceptionObject, null, true);
				Logger.Status("Closing the server.");
			}
			catch { }
			ServerUtil.Exit(1, false);
		}

		/// <summary>
		/// Tries to connect to login server, keeps trying every 10 seconds
		/// till there is a success. Blocking.
		/// </summary>
		private void ConnectToLogin(bool firstTime)
		{
			Logger.Write("");
			if (firstTime)
				Logger.Info("Trying to connect to login server at {0}:{1}...", WorldConf.LoginHost, WorldConf.LoginPort);
			else
			{
				Logger.Info("Trying to re-connect to login server in {0} seconds.", LoginTryTime / 1000);
				Thread.Sleep(LoginTryTime);
			}

			bool success = false;
			while (!success)
			{
				try
				{
					if (LoginServer != null)
					{
						try
						{
							LoginServer.Socket.Shutdown(SocketShutdown.Both);
							LoginServer.Socket.Close();
						}
						catch
						{ }
					}

					LoginServer = new WorldClient();
					LoginServer.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					LoginServer.Socket.Connect(WorldConf.LoginHost, WorldConf.LoginPort);

					var buffer = new byte[255];

					// Recv Seed, send back empty packet to get done with the challenge.
					LoginServer.Socket.Receive(buffer);
					LoginServer.Crypto = new MabiCrypto(BitConverter.ToUInt32(buffer, 0));
					LoginServer.Send(new MabiPacket(0, 0));

					// Challenge end
					LoginServer.Socket.Receive(buffer);

					// Inject login server to the normal data receiving.
					LoginServer.Socket.BeginReceive(LoginServer.Buffer.Front, 0, LoginServer.Buffer.Front.Length, SocketFlags.None, new AsyncCallback(this.OnReceive), LoginServer);

					// Identify
					LoginServer.State = ClientState.LoggingIn;
					LoginServer.Send(new MabiPacket(Op.Internal.ServerIdentify).PutString(MabiPassword.Hash(WorldConf.Password)));

					success = true;
				}
				catch (Exception ex)
				{
					Logger.Error("Unable to connect to login server. ({1})", "xyz", ex.Message);
					Logger.Info("Trying again in {0} seconds.", LoginTryTime / 1000);
					Thread.Sleep(LoginTryTime);
				}
			}

			Logger.Info("Connection to login server esablished.");
			Logger.Write("");
		}

		/// <summary>
		/// Sends channel status to login server.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public void SendChannelStatus(MabiTime time)
		{
			// Let's asume 20 users would be a lot for now.
			// TODO: Option for max users.
			uint count = WorldManager.Instance.GetCharactersCount();
			byte stress = (byte)Math.Min(75, Math.Ceiling(75 / 20.0f * count));

			if (LoginServer.State == ClientState.LoggedIn)
			{
				LoginServer.Send(
					new MabiPacket(Op.Internal.ChannelStatus)
					.PutString(WorldConf.ServerName)
					.PutString(WorldConf.ChannelName)
					.PutString(WorldConf.ChannelHost)
					.PutShort(WorldConf.ChannelPort)
					.PutByte(stress)
				);
			}
		}

		public bool InitCache()
		{
			if (!Directory.Exists(WorldConf.CachePath))
				Directory.CreateDirectory(WorldConf.CachePath);

			return Directory.Exists(WorldConf.CachePath);
		}

		/// <summary>
		/// Kills client and checks if we have to reconnect to login,
		/// if the client in question was the login server's.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="type"></param>
		protected override void OnClientDisconnect(WorldClient client, Net.DisconnectType type)
		{
			base.OnClientDisconnect(client, type);

			if (client == LoginServer)
			{
				Logger.Info("Lost connection to login server.");
				this.ConnectToLogin(false);
			}
		}
	}
}
