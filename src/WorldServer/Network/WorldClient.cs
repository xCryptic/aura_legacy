// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see licence.txt in the main folder

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Aura.Data;
using Aura.Shared.Const;
using Aura.Shared.Network;
using Aura.Shared.Util;
using Aura.World.Database;
using Aura.World.Player;
using Aura.World.Scripting;
using Aura.World.World;
using Aura.World.Events;

namespace Aura.World.Network
{
	public class WorldClient : Client
	{
		public Account Account;
		public List<MabiCreature> Creatures = new List<MabiCreature>();
		public MabiPC Character;

		public readonly NPCSession NPCSession = new NPCSession();

		public MabiCreature GetCreatureOrNull(ulong id)
		{
			return this.Creatures.FirstOrDefault(a => a.Id == id);
		}

		public MabiPC GetCharacterOrNull(ulong id)
		{
			return this.GetCreatureOrNull(id) as MabiPC;
		}

		public override void Kill()
		{
			if (this.State != ClientState.Dead)
			{
                EventManager.PlayerEvents.OnPlayerLoggedOff(this.Character);

				foreach (var creature in this.Creatures)
					WorldManager.Instance.RemoveCreature(creature);

				if (this.Account != null)
					WorldDb.Instance.SaveAccount(this.Account);

				if (this.Socket != null)
				{
					try
					{
						if (this.Socket.Connected)
							this.Socket.Shutdown(SocketShutdown.Both);
					}
					catch (Exception ex)
					{
						Logger.Error("Failed to shutdown socket: " + ex.Message);
					}
					try
					{
						this.Socket.Close();
					}
					catch (Exception ex)
					{
						Logger.Error("Failed to close socket: " + ex.Message);
					}
				}

				this.State = ClientState.Dead;
			}
			else
			{
				Logger.Warning("Client got killed multiple times." + Environment.NewLine + Environment.StackTrace);
			}
		}

		public void Disconnect(int seconds = 5)
		{
			this.Send(new MabiPacket(Op.RequestClientDisconnect, Id.World).PutSInt(seconds * 1000));
		}

		public void Warp(string region, uint x, uint y)
		{
			var regionId = MabiData.RegionDb.TryGetRegionId(region);
			if (regionId > 0)
				this.Warp(regionId, x, y);
		}

		public void Warp(uint region, uint x, uint y)
		{
			var pos = this.Character.GetPosition();
			this.Send(new MabiPacket(Op.SetLocation, this.Character.Id).PutByte(1).PutInts(this.Character.Region, pos.X, pos.Y));
			this.Send(new MabiPacket(Op.WarpUnk1, this.Character.Id));
			this.Send(new MabiPacket(Op.WarpUnk2, this.Character.Id).PutByte(11));

			WorldManager.Instance.CreatureLeaveRegion(this.Character);

			var creatures = WorldManager.Instance.GetCreaturesInRange(this.Character);
			if (this.Creatures.Count > 1)
				creatures.AddRange(this.Creatures.Where(c => c != this.Character));
			if (creatures.Count > 0)
				Aura.World.Network.Send.EntitiesDisappear(this, creatures);

			Aura.World.Network.Send.CharacterLock(this);
			this.Character.SetLocation(region, x, y);

			Aura.World.Network.Send.EnterRegionPermission(this, this.Character);

			this.Character.OnAltar = DungeonAltar.None;

			foreach (var c in this.Creatures.Where(c => c != this.Character))
			{
				Aura.World.Network.Send.CharacterLock(this, c);
				c.OnAltar = DungeonAltar.None;
			}

			this.Send(new MabiPacket(Op.WarpUnk3, this.Character.Id).PutLong(0).PutInt(0));
		}
	}

	/// <summary>
	/// Used as default client, so we don't have to worry about null checks.
	/// </summary>
	public class DummyClient : Client
	{
		public override void Send(byte[] buffer)
		{ }

		public override void Send(MabiPacket packet)
		{ }
	}
}
