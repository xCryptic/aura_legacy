﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see licence.txt in the main folder

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Common.Constants;
using Common.Data;
using Common.Tools;
using Common.World;
using csscript;
using CSScriptLibrary;
using World.Tools;
using World.World;

namespace World.Scripting
{
	public class ScriptManager
	{
		public readonly static ScriptManager Instance = new ScriptManager();
		static ScriptManager() { }
		private ScriptManager() { }

		private int _loadedNpcs, _cached;

		public void LoadScripts()
		{
			Logger.Info("Loading scripts...");

			_loadedNpcs = _cached = 0;

			// Read script list
			var listPath = Path.Combine(WorldConf.ScriptPath, "scripts.txt");
			var npcListContents = new List<string>();

			if (File.Exists(listPath))
			{
				try
				{
					npcListContents = FileReader.GetAllLines(listPath);
				}
				catch (Exception ex)
				{
					Logger.Warning("Unable to fully parse " + listPath + ". Error: " + ex.Message);
				}
			}
			else
			{
				Logger.Warning("Unable to find script list at '" + listPath + "'.");
			}

			var scriptFiles = npcListContents.ToArray();

			// Check how much is to be cached
			var cacheDir = Path.Combine(WorldConf.ScriptPath, "cache", "npc");
			int cached = 0;
			if (Directory.Exists(cacheDir))
				cached = Directory.GetFiles(Path.Combine(WorldConf.ScriptPath, "cache", "npc"), "*.compiled", SearchOption.AllDirectories).Length;
			int notCached = scriptFiles.Length - cached;

			if (notCached > 30)
				Logger.Info("Caching the scripts may take a few minutes initially.");

			// Load scripts
			int loaded = 0;
			foreach (var line in scriptFiles)
			{
				var file = line;
				bool virt = false;
				if (file.StartsWith("virtual:"))
				{
					virt = true;
					file = file.Replace("virtual:", "").Trim();
				}
				this.LoadScript(Path.Combine(WorldConf.ScriptPath, file), virt);

				// Don't print for every single NPC, so much flickering.
				if (++loaded % 5 == 0 || loaded == 1 || loaded >= scriptFiles.Length)
				{
					int loadPercBar = (int)(20f / scriptFiles.Length * loaded);
					var bar = "[" + "".PadLeft(loadPercBar, '#') + "".PadLeft(20 - loadPercBar, '.') + "]";

					Logger.Info(string.Format("{1} {0,6:0.0}%", 100f / scriptFiles.Length * loaded, bar), false);
					Logger.RLine(); // Move cursor to pos 0, so error msgs can overwrite this line.
				}
			}

			// fin~
			Logger.ClearLine();
			Logger.Info("Done loading " + _loadedNpcs + " scripts.");
			if (!WorldConf.DisableScriptCaching)
				Logger.Info("Cached " + _cached + " scripts.");
		}

		/// <summary>
		/// Loads the script file at the given path and adds the NPC to the world.
		/// </summary>
		/// <param name="path"></param>
		private void LoadScript(string scriptPath, bool virtualLoad = false)
		{
			try
			{
				// Load assembly and create object.
				var scriptObj = this.GetScript(scriptPath).CreateObject("*");

				// Check if the object is derived from NPCScript. Needed to
				// allow simpler scripts that only derive from BaseScript.
				if (scriptObj is NPCScript)
				{
					var script = scriptObj as NPCScript;
					script.ScriptPath = scriptPath;
					script.ScriptName = Path.GetFileName(scriptPath);
					if (!virtualLoad)
					{
						var npc = new MabiNPC();
						npc.Script = script;
						npc.ScriptPath = scriptPath;
						script.NPC = npc;
						script.LoadType = NPCLoadType.Real;
						script.OnLoad();
						script.OnLoadDone();

						// Only load defaults if race is set.
						if (npc.Race != 0 && npc.Race != uint.MaxValue)
							npc.LoadDefault();

						WorldManager.Instance.AddCreature(npc);
					}
					else
					{
						script.LoadType = NPCLoadType.Virtual;
					}
				}
				else
				{
					// Script that doesn't use an NPC, like prop scripts and stuff.

					var script = scriptObj as BaseScript;
					script.ScriptPath = scriptPath;
					script.ScriptName = Path.GetFileName(scriptPath);

					script.OnLoad();
					script.OnLoadDone();
				}

				Interlocked.Increment(ref _loadedNpcs);
			}
			catch (CompilerException ex)
			{
				var errors = ex.Data["Errors"] as CompilerErrorCollection;

				Logger.Error("In " + scriptPath + ":");
				foreach (CompilerError err in errors)
					Logger.Error("  Line " + err.Line.ToString() + ": " + err.ErrorText);
			}
			catch (Exception ex)
			{
				try
				{
					File.Delete(this.GetCompiledPath(scriptPath));
				}
				catch { }
				try
				{
					new FileInfo(scriptPath).LastWriteTime = DateTime.Now; // "Touch" the file, to mark it for recompilation (due to type load errors with inherited scripts) 
				}
				catch { }
				Logger.Error("While processing: " + scriptPath + " ... " + ex.Message);
			}
		}

		/// <summary>
		/// Adds "cache" to the given path.
		/// Example: data/script/folder/script.cs > data/script/cache/folder/script.cs
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		private string GetCompiledPath(string filePath)
		{
			return Path.ChangeExtension(filePath.Replace(WorldConf.ScriptPath, Path.Combine(WorldConf.ScriptPath, "cache")), ".compiled");
		}

		/// <summary>
		/// Loads the given script from the cache, or recaches it,
		/// if necessary. Returns script assembly.
		/// </summary>
		/// <param name="scriptPath"></param>
		/// <returns></returns>
		public Assembly GetScript(string scriptPath)
		{
			var compiledPath = this.GetCompiledPath(scriptPath);

			Assembly asm;
			if (!WorldConf.DisableScriptCaching && File.Exists(compiledPath) && File.GetLastWriteTime(compiledPath) >= File.GetLastWriteTime(scriptPath))
			{
				asm = Assembly.LoadFrom(compiledPath);
			}
			else
			{
				asm = CSScript.LoadCode(this.PreCompileScript(scriptPath));
				if (!WorldConf.DisableScriptCaching)
				{
					try
					{
						if (File.Exists(compiledPath))
							File.Delete(compiledPath);
						else
							Directory.CreateDirectory(Path.GetDirectoryName(compiledPath));

						File.Copy(asm.Location, compiledPath);

						Interlocked.Increment(ref _cached);
					}
					catch (UnauthorizedAccessException)
					{
						// Thrown if file can't be copied. Happens if script was
						// initially loaded from cache.
					}
					catch (Exception ex)
					{
						Logger.Warning(ex.Message);
					}
				}
			}

			return asm;
		}

		private string PreCompileScript(string filePath)
		{
			var file = File.ReadAllText(filePath);
			var sb = new StringBuilder();

			// Usings
			{
				// Default:
				// using System;
				// using Common.Constants;
				// using Common.World;
				// using World.Network;
				// using World.Scripting;
				// using World.World;

				//if (!Regex.Match(file, @"(^|;)\s*using System;").Success) sb.Append("using System;");
				//if (!Regex.Match(file, @"(^|;)\s*using Common.Constants;").Success) sb.Append("using Common.Constants;");
				//if (!Regex.Match(file, @"(^|;)\s*using Common.World;").Success) sb.Append("using Common.World;");
				//if (!Regex.Match(file, @"(^|;)\s*using World.Network;").Success) sb.Append("using World.Network;");
				//if (!Regex.Match(file, @"(^|;)\s*using World.Scripting;").Success) sb.Append("using World.Scripting;");
				//if (!Regex.Match(file, @"(^|;)\s*using World.World;").Success) sb.Append("using World.World;");
			}

			// Wait, End
			{
				// [var] <variable> = Wait();
				// --> [var] <variable>Object = new Response(); yield return <variable>Object; [var] <variable> = <variable>Object.Value;
				file = Regex.Replace(file, @"\s*(?:(var )|(?:[^\s]+ ))?\s*([^\s\)]*)\s*=\s*Wait\s*\(\s*\)\s*;", "$1$2Object = new Response(); yield return $2Object; $1$2 = $2Object.Value;", RegexOptions.Compiled);

				// End();
				// --> yield break;
				file = Regex.Replace(file, @"End\s*\(\s*\)\s*;", "yield break;", RegexOptions.Compiled);
			}

			// Append the (changed) file
			sb.Append(file);

			return sb.ToString();
		}

		public void LoadSpawns()
		{
			int count = 0;
			foreach (var spawnInfo in MabiData.SpawnDb.Entries)
			{
				count += this.Spawn(spawnInfo);
			}

			Logger.Info("Spawned " + count.ToString() + " monsters.");
		}

		public void Spawn(uint spawnId, uint amount = 0)
		{
			var info = MabiData.SpawnDb.Find(spawnId);
			if (info == null)
			{
				Logger.Warning("Unknown spawn Id '" + spawnId.ToString() + "'.");
				return;
			}

			this.Spawn(info, amount);
		}

		public int Spawn(SpawnInfo info, uint amount = 0)
		{
			var result = 0;

			var rand = RandomProvider.Get();

			var raceInfo = MabiData.RaceDb.Find(info.RaceId);
			if (raceInfo == null)
			{
				Logger.Warning("Race not found: " + info.RaceId.ToString());
				return 0;
			}

			string aiFilePath = null;
			if (!string.IsNullOrEmpty(raceInfo.AI))
			{
				aiFilePath = Path.Combine(WorldConf.ScriptPath, "ai", raceInfo.AI + ".cs");
				if (!File.Exists(aiFilePath))
				{
					Logger.Warning("AI script '" + raceInfo.AI + ".cs' couldn't be found.");
					aiFilePath = null;
				}
			}

			if (amount == 0)
				amount = info.Amount;

			for (int i = 0; i < amount; ++i)
			{
				var monster = new MabiNPC();
				monster.SpawnId = info.Id;
				monster.Name = raceInfo.Name;
				var loc = info.GetRandomSpawnPoint(rand);
				monster.SetLocation(info.Region, loc.X, loc.Y);
				monster.ColorA = raceInfo.ColorA;
				monster.ColorB = raceInfo.ColorB;
				monster.ColorC = raceInfo.ColorC;
				monster.Height = raceInfo.Size;
				monster.LifeMaxBase = raceInfo.Life;
				monster.Life = raceInfo.Life;
				monster.Race = raceInfo.Id;
				monster.BattleExp = raceInfo.Exp;
				monster.Direction = (byte)rand.Next(256);
				monster.State &= ~CreatureStates.GoodNpc; // Use race default?
				monster.GoldMin = raceInfo.GoldMin;
				monster.GoldMax = raceInfo.GoldMax;
				monster.Drops = raceInfo.Drops;

				foreach (var skill in raceInfo.Skills)
				{
					monster.Skills.Add(new MabiSkill(skill.SkillId, skill.Rank, monster.Race));
				}

				monster.LoadDefault();

				if (aiFilePath != null)
				{
					monster.AIScript = this.GetScript(aiFilePath).CreateObject("*") as AIScript;
					monster.AIScript.Creature = monster;
					monster.AIScript.OnLoad();
					monster.AIScript.Activate(0); // AI is intially active
				}

				WorldManager.Instance.AddCreature(monster);
				result++;
			}

			return result;
		}
	}
}