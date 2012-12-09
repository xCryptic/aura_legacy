﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see licence.txt in the main folder

// Uncomment to get test packets when dropping an item and clicking on the
// following downstairs portal. Current test packet is based on Alby normal,
// props and entities (stairs, stautes, doors, ...) aren't sent.
//#define DUNGEON_TEST

using System;
using System.Linq;
using System.Text.RegularExpressions;
using Common.Constants;
using Common.Data;
using Common.Database;
using Common.Events;
using Common.Network;
using Common.Tools;
using Common.World;
using World.Tools;
using World.World;
using World.Skills;

namespace World.Network
{
	public partial class WorldServer : Server<WorldClient>
	{
		protected override void InitPacketHandlers()
		{
			this.RegisterPacketHandler(Op.LoginW, HandleLogin);
			this.RegisterPacketHandler(Op.DisconnectW, HandleDisconnect);
			this.RegisterPacketHandler(Op.CharInfoRequestW, HandleCharacterInfoRequest);

			this.RegisterPacketHandler(Op.Walk, HandleMove);
			this.RegisterPacketHandler(Op.Run, HandleMove);
			this.RegisterPacketHandler(Op.Chat, HandleChat);
			this.RegisterPacketHandler(Op.WhisperChat, HandleWhisperChat);
			this.RegisterPacketHandler(Op.VisualChat, HandleVisualChat);

			this.RegisterPacketHandler(Op.ItemMove, HandleItemMove);
			this.RegisterPacketHandler(Op.ItemPickUp, HandleItemPickUp);
			this.RegisterPacketHandler(Op.ItemDrop, HandleItemDrop);
			this.RegisterPacketHandler(Op.ItemDestroy, HandleItemDestroy);
			this.RegisterPacketHandler(Op.ItemSplit, HandleItemSplit);
			this.RegisterPacketHandler(Op.SwitchSet, HandleSwitchSet);
			this.RegisterPacketHandler(Op.ItemStateChange, HandleItemStateChange);
			this.RegisterPacketHandler(Op.ItemUse, HandleItemUse);
			this.RegisterPacketHandler(Op.ViewEquipment, HandleViewEquipment);

			this.RegisterPacketHandler(Op.NPCTalkStart, HandleNPCTalkStart);
			this.RegisterPacketHandler(Op.NPCTalkEnd, HandleNPCTalkEnd);
			this.RegisterPacketHandler(Op.NPCTalkPartner, HandleNPCTalkPartner);
			this.RegisterPacketHandler(Op.NPCTalkKeyword, HandleNPCTalkKeyword);
			this.RegisterPacketHandler(Op.NPCTalkSelect, HandleNPCTalkSelect);
			this.RegisterPacketHandler(Op.ShopBuyItem, HandleShopBuyItem);
			this.RegisterPacketHandler(Op.ShopSellItem, HandleShopSellItem);

			this.RegisterPacketHandler(Op.ChangeStance, HandleChangeStance);
			this.RegisterPacketHandler(Op.CombatSetTarget, HandleCombatSetTarget);
			this.RegisterPacketHandler(Op.CombatAttack, HandleCombatAttack);
			this.RegisterPacketHandler(Op.StunMeter, HandleStunMeterDummy);
			this.RegisterPacketHandler(Op.SubsribeStun, HandleStunMeterRequest); // subscription ?
			this.RegisterPacketHandler(Op.Revive, HandleRevive);
			this.RegisterPacketHandler(Op.DeadMenu, HandleDeadMenu);

			this.RegisterPacketHandler(Op.SkillPrepare, HandleSkillPrepare);
			this.RegisterPacketHandler(Op.SkillReady, HandleSkillReady);
			this.RegisterPacketHandler(Op.SkillUse, HandleSkillUse);
			this.RegisterPacketHandler(Op.SkillComplete, HandleSkillComplete);
			this.RegisterPacketHandler(Op.SkillCancel, HandleSkillCancel);
			this.RegisterPacketHandler(Op.SkillStart, HandleSkillStart);
			this.RegisterPacketHandler(Op.SkillStop, HandleSkillStop);
			this.RegisterPacketHandler(Op.SkillAdvance, HandleSkillAdvance);

			this.RegisterPacketHandler(Op.PetSummon, HandlePetSummon);
			this.RegisterPacketHandler(Op.PetUnsummon, HandlePetUnsummon);
			this.RegisterPacketHandler(Op.PetMount, HandlePetMount);
			this.RegisterPacketHandler(Op.PetUnmount, HandlePetUnmount);

			this.RegisterPacketHandler(Op.TouchProp, HandleTouchProp);
			this.RegisterPacketHandler(Op.HitProp, HandleHitProp);

			this.RegisterPacketHandler(Op.EnterRegion, HandleEnterRegion);
			this.RegisterPacketHandler(Op.AreaChange, HandleAreaChange);

			this.RegisterPacketHandler(Op.ChangeTitle, HandleTitleChange);
			this.RegisterPacketHandler(Op.MailsRequest, HandleMailsRequest);
			this.RegisterPacketHandler(Op.SosButton, HandleSosButton);
			this.RegisterPacketHandler(Op.MoonGateRequest, HandleMoonGateRequest);
			this.RegisterPacketHandler(Op.UseGesture, HandleGesture);
			this.RegisterPacketHandler(Op.HomesteadInfoRequest, HandleHomesteadInfo);
			this.RegisterPacketHandler(Op.OpenItemShop, HandleOpenItemShop);

			this.RegisterPacketHandler(Op.GMCPSummon, HandleGMCPSummon);
			this.RegisterPacketHandler(Op.GMCPMoveToChar, HandleGMCPMoveToChar);
			this.RegisterPacketHandler(Op.GMCPMove, HandleGMCPMove);
			this.RegisterPacketHandler(Op.GMCPRevive, HandleGMCPRevive);
			this.RegisterPacketHandler(Op.GMCPInvisibility, HandleGMCPInvisibility);
			this.RegisterPacketHandler(Op.GMCPExpel, HandleGMCPExpel);
			this.RegisterPacketHandler(Op.GMCPBan, HandleGMCPBan);
			this.RegisterPacketHandler(Op.GMCPClose, (c, p) => { }); // TODO: Maybe add this to a gm log.
			this.RegisterPacketHandler(Op.GMCPNPCList, HandleGMCPListNPCs);

			// Temp/Unknown
			// --------------------------------------------------------------

			this.RegisterPacketHandler(0xA43D, (client, packet) =>
			{
				client.Send(new MabiPacket(0xA43E, client.Character.Id).PutByte(1));

				// Requested/Sent on login?
				// Alternative structure: (Conti and Nao warps)
				// 001 [..............00]  Byte   : 0
				// 002 [000039BA86EA43C0]  Long   : 000039BA86EA43C0 // 2012-May-22 15:30:00
				// 003 [000039BA86FABE80]  Long   : 000039BA86FABE80 // 2012-May-22 15:48:00
			});

			this.RegisterPacketHandler(0x61A8, (client, packet) =>
			{
				// TODO: Send entities?
				// NOTE: No idea what this does anymore...
			});

			this.RegisterPacketHandler(0xAAE6, (client, packet) =>
			{
				// Sent when ESC is pressed? (Noticed in G17)
			});

			this.RegisterPacketHandler(0xA897, (client, packet) =>
			{
				// Sent while logging in.

				client.Send(new MabiPacket(0xA898, client.Character.Id));

				// Reply example:
				// 001 [00000000000A0F4D]  Long   : 00000000000A0F4D
				// 002 [000000000000000F]  Long   : 000000000000000F
				// 003 [........00000004]  Int    : 4
				// 004 [............0008]  Short  : 8
				// 005 [........00000001]  Int    : 1
				// 006 [........00014599]  Int    : 83353
				// 007 [........00000002]  Int    : 2
				// 008 [........0002C836]  Int    : 182326
				// 009 [........00000003]  Int    : 3
				// 010 [........000158C4]  Int    : 88260
				// 011 [........00000004]  Int    : 4
				// 012 [........0001BAA8]  Int    : 113320
				// 013 [........00000005]  Int    : 5
				// 014 [........0003B3E3]  Int    : 242659
				// 015 [........00000006]  Int    : 6
				// 016 [........00007FC0]  Int    : 32704
				// 017 [........00000007]  Int    : 7
				// 018 [........00008BBA]  Int    : 35770
				// 019 [........00000008]  Int    : 8
				// 020 [........0008D5FF]  Int    : 579071
			});
		}

#pragma warning disable 0162
		private void HandleLogin(WorldClient client, MabiPacket packet)
		{
			if (client.State != SessionState.Login)
				return;

			var userName = packet.GetString();
			if (Op.Version > 160000)
				packet.GetString(); // double acc name
			var seedKey = packet.GetLong();
			var charID = packet.GetLong();
			//byte unk1 = packet.GetByte();

			MabiPacket p;

			if (client.Account == null)
			{
				if (!MabiDb.Instance.IsSessionKey(userName, seedKey))
				{
					Logger.Warning("Invalid session key.");
					client.Kill();
					return;
				}

				client.Account = MabiDb.Instance.GetAccount(userName);
			}

			MabiPC creature = client.Account.Characters.FirstOrDefault(a => a.Id == charID);
			if (creature == null && (creature = client.Account.Pets.FirstOrDefault(a => a.Id == charID)) == null)
			{
				Logger.Warning("Creature not in account.");
				return;
			}

			creature.Save = true;

			client.Creatures.Add(creature);
			client.Character = creature;
			client.Character.Client = client;

			//p = new MabiPacket(0x90A1, creature.Id);
			//p.PutByte(0);
			//p.PutByte(1);
			//p.PutInt(0);
			//p.PutInt(0);
			//client.Send(p);

			p = new MabiPacket(Op.LoginWR, 0x1000000000000001);
			p.PutByte(1);
			p.PutLong(creature.Id);
			p.PutLong(DateTime.Now);
			p.PutInt(1);
			p.PutString("");
			client.Send(p);

			p = new MabiPacket(Op.CharacterLock, creature.Id);
			p.PutInt(0xEFFFFFFE);
			p.PutInt(0);
			client.Send(p);

			client.Send(PacketCreator.EnterRegionPermission(creature));

			client.State = SessionState.LoggedIn;

			ServerEvents.Instance.OnPlayerLogsIn(creature);
		}
#pragma warning restore 0162

		private void HandleDisconnect(WorldClient client, MabiPacket packet)
		{
			// TODO: Some check or move the unsafe stuff!

			Logger.Info("'" + client.Account.Username + "' is closing the connection. Saving...");

			MabiDb.Instance.SaveAccount(client.Account);

			foreach (var pc in client.Creatures)
			{
				WorldManager.Instance.RemoveCreature(pc);
			}

			client.Creatures.Clear();
			client.Character = null;
			client.Account = null;

			var p = new MabiPacket(Op.DisconnectWR, 0x1000000000000001);
			p.PutByte(0);
			client.Send(p);
		}

		private void HandleCharacterInfoRequest(WorldClient client, MabiPacket packet)
		{
			var p = new MabiPacket(Op.CharInfoRequestWR, 0x1000000000000001);

			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
			{
				p.PutByte(0);
				client.Send(p);
				return;
			}

			p.PutByte(1);
			(creature as MabiPC).AddPrivateEntityData(p);
			client.Send(p);

			if (creature.Owner != null)
			{
				if (creature.RaceInfo.VehicleType > 0)
				{
					WorldManager.Instance.VehicleUnbind(null, creature, true);
				}

				if (creature.IsDead())
				{
					WorldManager.Instance.Broadcast(new MabiPacket(Op.DeadFeather, creature.Id).PutShort(1).PutInt(10).PutByte(0), SendTargets.Range, creature);
				}
			}

			if (creature == client.Character)
			{
				if (WorldConf.EnableItemShop)
				{
					// Button will be disabled if we don't send this packet.
					client.Send(new MabiPacket(Op.ItemShopInfo, creature.Id).PutByte(0));
				}

				if (WorldConf.AutoSendGMCP && client.Account.Authority >= WorldConf.MinimumGMCP)
				{
					client.Send(new MabiPacket(Op.GMCPOpen, creature.Id));
				}
			}
		}

		private void HandleChat(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			var type = packet.GetByte();
			var message = packet.GetString();

			if (message[0] == WorldConf.CommandPrefix)
			{
				message = message.ToLower();

				var args = message.Substring(1).Split(' ');

				if (CommandHandler.Instance.Handle(client, creature, args, message))
					return;
			}

			WorldManager.Instance.CreatureTalk(creature, message, type);
		}

		private void HandleWhisperChat(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			var target = WorldManager.Instance.GetCharacterByName(packet.GetString());
			if (target == null)
			{
				client.Send(PacketCreator.SystemMessage(creature, "The target character does not exist."));
				return;
			}

			var msg = packet.GetString();
			client.Send(PacketCreator.Whisper(creature, creature.Name, msg));
			target.Client.Send(PacketCreator.Whisper(target, creature.Name, msg));
		}

		private void HandleGesture(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			creature.StopMove();

			byte result = 0;

			var info = MabiData.MotionDb.Find(packet.GetString());
			if (info != null)
			{
				result = 1;

				// TODO: Temp fix, add information to motion list.
				var loop = false;
				if (info.Name == "listen_music")
					loop = true;

				WorldManager.Instance.CreatureUseMotion(creature, info.Category, info.Type, loop);
			}

			var p = new MabiPacket(Op.UseGestureR, creature.Id);
			p.PutByte(result);
			client.Send(p);
		}

		public void HandleItemUse(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null || creature.IsDead())
				return;

			var itemId = packet.GetLong();

			var item = creature.GetItem(itemId);
			if (item == null || item.DataInfo == null || item.Type != ItemType.Usable)
			{
				client.Send(new MabiPacket(Op.UseItemR, creature.Id).PutByte(0));
				return;
			}

			// Doing this with ifs might be better.
			switch ((UsableType)item.DataInfo.UsableType)
			{
				// TODO: Remaining positive and negative effects.
				case UsableType.Life:
					creature.Life += item.DataInfo.UsableVar;
					break;
				case UsableType.Mana:
					creature.Mana += item.DataInfo.UsableVar;
					break;
				case UsableType.Stamina:
					creature.Stamina += item.DataInfo.UsableVar;
					break;
				case UsableType.Injury:
					creature.Injuries -= item.DataInfo.UsableVar;
					break;
				case UsableType.LifeMana:
					creature.Life += item.DataInfo.UsableVar;
					creature.Mana += item.DataInfo.UsableVar;
					break;
				case UsableType.LifeStamina:
					creature.Life += item.DataInfo.UsableVar;
					creature.Stamina += item.DataInfo.UsableVar;
					break;
				case UsableType.Food:
					creature.Hunger -= item.DataInfo.UsableVar;
					break;
				case UsableType.Recovery:
					// Full Recovery
					if (item.DataInfo.UsableVar == 100)
					{
						// Manually, to handle multiple recovery items at
						// once later, and we don't need 2 update packets.
						creature.Injuries = 0;
						creature.Hunger = 0;
						creature.Life = creature.LifeMax;
						creature.Mana = creature.ManaMax;
						creature.Stamina = creature.StaminaMax;
					}
					// Various chocolates? Full recovery as well?
					//else if (item.DataInfo.UsableVar == 300)
					//{
					//}
					// Recovery booster
					//else if (item.DataInfo.UsableVar == 1)
					//{
					//}
					else goto default;
					break;
				case UsableType.Antidote:
				case UsableType.Elixir:
				case UsableType.Others:
				default:
					client.Send(new MabiPacket(Op.UseItemR, creature.Id).PutByte(0));
					Logger.Unimplemented("This usable type is not supported yet.");
					return;
			}

			creature.DecItem(item);

			WorldManager.Instance.CreatureStatsUpdate(creature);

			client.Send(new MabiPacket(Op.UseItemR, creature.Id).PutByte(1).PutInt(item.Info.Class));
		}

		public void HandleGMCPMove(WorldClient client, MabiPacket packet)
		{
			if (client.Account.Authority < WorldConf.MinimumGMCP)
			{
				client.Send(PacketCreator.SystemMessage(client.Character, "You're not authorized to use the GMCP."));
				return;
			}

			var region = packet.GetInt();
			var x = packet.GetInt();
			var y = packet.GetInt();

			client.Warp(region, x, y);
		}

		private void HandleGMCPMoveToChar(WorldClient client, MabiPacket packet)
		{
			if (client.Account.Authority < WorldConf.MinimumGMCP)
			{
				client.Send(PacketCreator.SystemMessage(client.Character, "You're not authorized to use the GMCP."));
				return;
			}

			var targetName = packet.GetString();
			var target = WorldManager.Instance.GetCharacterByName(targetName, false);
			if (target == null)
			{
				client.Send(PacketCreator.MsgBox(client.Character, "Character '" + targetName + "' couldn't be found."));
				return;
			}

			var targetPos = target.GetPosition();
			client.Warp(target.Region, targetPos.X, targetPos.Y);
		}

		private void HandleGMCPRevive(WorldClient client, MabiPacket packet)
		{
			if (client.Account.Authority < WorldConf.MinimumGMCP)
			{
				client.Send(PacketCreator.SystemMessage(client.Character, "You're not authorized to use the GMCP."));
				return;
			}

			var creature = WorldManager.Instance.GetCreatureById(packet.Id);
			if (creature == null || !creature.IsDead())
				return;

			var pos = creature.GetPosition();
			var region = creature.Region;

			var response = new MabiPacket(Op.Revived, creature.Id);
			response.PutInt(1);
			response.PutInt(region);
			response.PutInt(pos.X);
			response.PutInt(pos.Y);
			client.Send(response);

			WorldManager.Instance.CreatureRevive(creature);

			creature.FullHeal();
		}

		private void HandleGMCPSummon(WorldClient client, MabiPacket packet)
		{
			if (client.Account.Authority < WorldConf.MinimumGMCP)
			{
				client.Send(PacketCreator.SystemMessage(client.Character, "You're not authorized to use the GMCP."));
				return;
			}

			var targetName = packet.GetString();
			var target = WorldManager.Instance.GetCharacterByName(targetName);
			if (target == null || target.Client == null)
			{
				client.Send(PacketCreator.MsgBox(client.Character, "Character '" + targetName + "' couldn't be found."));
				return;
			}

			var targetClient = (target.Client as WorldClient);
			var pos = client.Character.GetPosition();

			targetClient.Send(PacketCreator.ServerMessage(target, "You've been summoned by '" + client.Character.Name + "'."));
			targetClient.Warp(client.Character.Region, pos.X, pos.Y);
		}

		private void HandleGMCPListNPCs(WorldClient client, MabiPacket packet)
		{
			if (client.Account.Authority < WorldConf.MinimumGMCP)
			{
				client.Send(PacketCreator.SystemMessage(client.Character, "You're not authorized to use the GMCP."));
				return;
			}

			client.Send(PacketCreator.SystemMessage(client.Character, "Unimplimented."));
		}

		private void HandleGMCPInvisibility(WorldClient client, MabiPacket packet)
		{
			if (client.Account.Authority < WorldConf.MinimumGMCP)
			{
				client.Send(PacketCreator.SystemMessage(client.Character, "You're not authorized to use the GMCP."));
				return;
			}

			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			var toggle = packet.GetByte();
			creature.Conditions.A = (toggle == 1 ? (creature.Conditions.A | CreatureConditionA.Invisible) : (creature.Conditions.A & ~CreatureConditionA.Invisible));
			WorldManager.Instance.CreatureStatusEffectsChange(creature, new EntityEventArgs(creature));

			var p = new MabiPacket(Op.GMCPInvisibilityR, creature.Id);
			p.PutByte(1);
			client.Send(p);
		}

		private void HandleGMCPExpel(WorldClient client, MabiPacket packet)
		{
			if (client.Account.Authority < WorldConf.MinimumGMCP)
			{
				client.Send(PacketCreator.SystemMessage(client.Character, "You're not authorized to use the GMCP."));
				return;
			}

			var targetName = packet.GetString();
			var target = WorldManager.Instance.GetCharacterByName(targetName);
			if (target == null || target.Client == null)
			{
				client.Send(PacketCreator.MsgBox(client.Character, "Character '" + targetName + "' couldn't be found."));
				return;
			}

			client.Send(PacketCreator.MsgBox(client.Character, "'" + targetName + "' has been kicked."));

			// Better kill the connection, modders could bypass a dc request.
			target.Client.Kill();
		}

		private void HandleGMCPBan(WorldClient client, MabiPacket packet)
		{
			if (client.Account.Authority < WorldConf.MinimumGMCP)
			{
				client.Send(PacketCreator.SystemMessage(client.Character, "You're not authorized to use the GMCP."));
				return;
			}

			var targetName = packet.GetString();
			var target = WorldManager.Instance.GetCharacterByName(targetName);
			if (target == null || target.Client == null)
			{
				client.Send(PacketCreator.MsgBox(client.Character, "Character '" + targetName + "' couldn't be found."));
				return;
			}

			var end = DateTime.Now.AddMinutes(packet.GetInt());
			target.Client.Account.BannedExpiration = end;
			target.Client.Account.BannedReason = packet.GetString();

			client.Send(PacketCreator.MsgBox(client.Character, "'" + targetName + "' has been banned till '" + end.ToString() + "'."));

			// Better kill the connection, modders could bypass a dc request.
			target.Client.Kill();
		}

		private void HandleNPCTalkStart(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			var npcId = packet.GetLong();

			var target = WorldManager.Instance.GetCreatureById(npcId) as MabiNPC;
			if (target == null)
			{
				Logger.Warning("Unknown NPC: " + npcId.ToString());
			}
			else if (target.Script == null)
			{
				Logger.Warning("Script for '" + target.Name + "' is null.");
				target = null;
			}
			else if (!WorldManager.InRange(creature, target, 1000))
			{
				client.Send(PacketCreator.MsgBox(creature, "You're too far away."));
				target = null;
			}

			var p = new MabiPacket(Op.NPCTalkStartR, creature.Id);

			if (target == null)
			{
				p.PutByte(0);
				client.Send(p);
				return;
			}

			p.PutByte(1);
			p.PutLong(npcId);

			client.Send(p);

			if (target == null)
			{
				return;
			}

			client.NPCSession.Start(target);

			target.Script.OnTalk(client);
		}

		private void HandleNPCTalkPartner(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			var id = packet.GetLong();

			var target = client.Creatures.FirstOrDefault(a => a.Id == id);
			if (target == null)
			{
				Logger.Warning("Talk to unspawned partner: " + id.ToString());
			}

			var npc = WorldManager.Instance.GetCreatureByName("_partnerdummy") as MabiNPC;
			if (npc == null || npc.Script == null)
			{
				Logger.Warning("NPC or script of '_partnerdummy' is null.");
				npc = null;
			}

			var p = new MabiPacket(Op.NPCTalkPartnerR, creature.Id);

			if (target == null || npc == null)
			{
				p.PutByte(0);
				client.Send(p);
				return;
			}

			p.PutByte(1);
			p.PutLong(id);
			p.PutString(creature.Name + "'s " + target.Name);
			p.PutString(creature.Name + "'s " + target.Name);
			client.Send(p);

			client.NPCSession.Start(npc);

			npc.Script.OnTalk(client);
		}

		private void HandleNPCTalkEnd(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			var npcId = packet.GetLong();
			var target = client.NPCSession.Target;

			var p = new MabiPacket(Op.NPCTalkEndR, creature.Id);
			p.PutByte(1);
			p.PutLong(target.Id);
			p.PutString("");
			client.Send(p);

			// XXX: Should we check if the target is the same as at the start of the convo?
			if (target == null || target.Script == null)
				Logger.Warning("Ending empty NPC session.");
			else
				target.Script.OnEnd(client);

			client.NPCSession.Clear();
		}

		private void HandleNPCTalkKeyword(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null || client.NPCSession.SessionId == -1)
				return;

			var keyword = packet.GetString();

			var target = client.NPCSession.Target;
			if (target == null || client.NPCSession.SessionId == -1)
			{
				Logger.Debug("No target or no session.");
				return;
			}

			var p = new MabiPacket(Op.NPCTalkKeywordR, creature.Id);
			p.PutByte(1);
			p.PutString(keyword);
			client.Send(p);
		}

		private void HandleNPCTalkSelect(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null || client.NPCSession.SessionId == -1)
				return;

			var response = packet.GetString();
			var sessionId = packet.GetInt();
			var target = client.NPCSession.Target;

			if (target == null || sessionId != client.NPCSession.SessionId)
			{
				Logger.Debug("No target or sessionId incorrect (" + sessionId.ToString() + " : " + (client.NPCSession.SessionId) + ")");
				return;
			}

			int pos = -1;
			if ((pos = response.IndexOf("<return type=\"string\">")) < 1)
			{
				Logger.Debug("No return value found.");
				return;
			}

			pos += 22;

			response = response.Substring(pos, response.IndexOf('<', pos) - pos);

			if (response == "@end")
			{
				client.Send(new MabiPacket(Op.NPCTalkSelectEnd, creature.Id));

				target.Script.OnEnd(client);
				return;
			}
			else if (response.StartsWith("@input"))
			{
				var splitted = response.Split(':');
				target.Script.OnSelect(client, splitted[0], splitted[1]);
			}
			else
			{
				target.Script.OnSelect(client, response);
			}
		}

		private void HandleItemMove(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			var itemId = packet.GetLong();
			var source = (Pocket)packet.GetByte();
			var target = (Pocket)packet.GetByte();
			var unk = packet.GetByte();
			var targetX = packet.GetByte();
			var targetY = packet.GetByte();

			var item = creature.GetItem(itemId);
			if (item == null || item.Type == ItemType.Hair || item.Type == ItemType.Face)
				return;

			// Stop moving when changing weapons
			if ((target >= Pocket.RightHand1 && target <= Pocket.Arrow2) || (source >= Pocket.RightHand1 && source <= Pocket.Arrow2))
				creature.StopMove();

			// Inv -> Cursor
			// --------------------------------------------------------------
			if (target == Pocket.Cursor)
			{
				// Move
				client.Send(
					new MabiPacket(Op.ItemMoveInfo, creature.Id)
					.PutLong(item.Id).PutBytes((byte)source, (byte)target)
					.PutByte(unk).PutBytes(0, 0)
				);

				item.Move(target, targetX, targetY);
				this.CheckItemMove(creature, item, source);

				// Update euip
				//if (source >= Pocket.Armor && source <= Pocket.Accessory2)
				//    WorldManager.Instance.CreatureUnequip(creature, source, target);

				// Okay
				client.Send(new MabiPacket(Op.ItemMoveR, creature.Id).PutByte(1));
				return;
			}

			// Cursor -> Inv
			// --------------------------------------------------------------

			// Check for item at the target space
			var collidingItem = creature.GetItemColliding(target, targetX, targetY, item);

			// Is there a collision?
			if (collidingItem != null && ((collidingItem.StackType == BundleType.Sac && (collidingItem.StackItem == item.Info.Class || collidingItem.StackItem == item.StackItem)) || (item.StackType == BundleType.Stackable && item.Info.Class == collidingItem.Info.Class)))
			{
				if (collidingItem.Info.Amount < collidingItem.StackMax)
				{
					var diff = (ushort)(collidingItem.StackMax - collidingItem.Info.Amount);

					collidingItem.Info.Amount += Math.Min(diff, item.Info.Amount);
					client.Send(PacketCreator.ItemAmount(creature, collidingItem));

					item.Info.Amount -= Math.Min(diff, item.Info.Amount);
					if (item.Info.Amount > 0)
					{
						client.Send(PacketCreator.ItemAmount(creature, item));
					}
					else
					{
						creature.Items.Remove(item);
						client.Send(PacketCreator.ItemRemove(creature, item));
					}

					client.Send(new MabiPacket(Op.ItemMoveR, creature.Id).PutByte(1));

					return;
				}
			}

			var p = new MabiPacket((uint)(collidingItem == null ? Op.ItemMoveInfo : Op.ItemSwitchInfo), creature.Id);
			p.PutLong(item.Id);
			p.PutByte((byte)source);
			p.PutByte((byte)target);
			p.PutByte(unk);
			p.PutByte(targetX);
			p.PutByte(targetY);
			if (collidingItem != null)
			{
				p.PutLong(collidingItem.Id);
				p.PutByte(collidingItem.Info.Pocket);
				p.PutByte(1);
				p.PutByte(unk);
				p.PutByte(0);
				p.PutByte(0);

				collidingItem.Move(item.Info.Pocket, item.Info.X, item.Info.Y);
			}
			client.Send(p);

			item.Move(target, targetX, targetY);
			this.CheckItemMove(creature, item, target);

			// Update Equip
			if (target.IsEquip())
				WorldManager.Instance.CreatureEquip(creature, item);

			client.Send(new MabiPacket(Op.ItemMoveR, creature.Id).PutByte(1));
		}

		/// <summary>
		/// Checks for moving second hand equipment and unequiping,
		/// and sends the needed packets.
		/// </summary>
		/// <param name="creature"></param>
		/// <param name="item"></param>
		/// <param name="pocket"></param>
		// TODO: Move this to MabiCreature.
		private void CheckItemMove(MabiCreature creature, MabiItem item, Pocket pocket)
		{
			// Check for moving second hand
			if (pocket == Pocket.RightHand1 || pocket == Pocket.RightHand2)
			{
				var secSource = pocket + 2; // RightHand1/2
				var secItem = creature.GetItemInPocket(secSource);
				if (secItem != null || (secItem = creature.GetItemInPocket(secSource += 2)) != null)
				{
					var secTarget = Pocket.Inventory;
					var free = creature.GetFreeItemSpace(secItem, secTarget);
					if (free == null)
					{
						secTarget = Pocket.Temporary;
						free = new MabiVertex(0, 0);
					}
					creature.Client.Send(
						new MabiPacket(Op.ItemMoveInfo, creature.Id)
						.PutLong(secItem.Id).PutBytes((byte)secSource, (byte)secTarget)
						.PutByte(2).PutBytes((byte)free.X, (byte)free.Y)
					);
					secItem.Move(secTarget, free.X, free.Y);
					WorldManager.Instance.CreatureUnequip(creature, secSource);
				}
			}

			// Notify clients of equip change if equipment is being dropped
			if (pocket.IsEquip())
				WorldManager.Instance.CreatureUnequip(creature, pocket);
		}

		private void HandleItemDrop(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			var itemId = packet.GetLong();
			var unk = packet.GetByte();

			var item = creature.Items.FirstOrDefault(a => a.Id == itemId);
			if (item == null || item.Type == ItemType.Hair || item.Type == ItemType.Face)
				return;

			var source = (Pocket)item.Info.Pocket;

			creature.Items.Remove(item);
			this.CheckItemMove(creature, item, source);
			client.Send(PacketCreator.ItemRemove(creature, item));

#if !DUNGEON_TEST
			// Drop it
			WorldManager.Instance.CreatureDropItem(creature, new ItemEventArgs(item));

			// Done
			var p = new MabiPacket(Op.ItemDropR, creature.Id);
			p.PutByte(1);
			client.Send(p);
#else
			client.Send(PacketCreator.Lock(creature));

			// Done
			var p = new MabiPacket(Op.ItemDropR, creature.Id);
			p.PutByte(1);
			client.Send(p);

			WorldManager.Instance.CreatureLeaveRegion(creature);
			creature.SetLocation(10022, 3262, 3139);

			var dunp = new MabiPacket(0x9470, 0x3000000000000000);
			dunp.PutLong(creature.Id);
			dunp.PutLong(0x01000000000005CD);
			dunp.PutByte(1);
			dunp.PutString("tircho_alby_dungeon"); // dungeon name (dungeondb.xml)
			dunp.PutInt(item.Info.Class);
			dunp.PutInt(938735421);
			dunp.PutInt(0);
			dunp.PutInt(2); // count?
			dunp.PutInt(10022); // imaginary entrance region id?
			dunp.PutInt(10032); // imaginary dungeon region id?
			dunp.PutString("");
			dunp.PutInt(1);
			dunp.PutInt(4); // 0 = client crash
			dunp.PutByte(4); // 0 adds another room
			dunp.PutByte(0);
			dunp.PutByte(1);
			dunp.PutByte(1);
			dunp.PutByte(3);
			dunp.PutByte(2);
			dunp.PutByte(1);
			dunp.PutByte(4);
			dunp.PutInt(0);
			dunp.PutInt(1);
			dunp.PutSInt(-1212925688);
			dunp.PutInt(0);
			client.Send(dunp);
#endif
		}

		public void HandleItemDestroy(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			var itemId = packet.GetLong();
			var item = creature.GetItem(itemId);
			if (item == null || item.Type == ItemType.Hair || item.Type == ItemType.Face)
				return;

			creature.Items.Remove(item);
			this.CheckItemMove(creature, item, (Pocket)item.Info.Pocket);

			client.Send(PacketCreator.ItemRemove(creature, item));
			client.Send(new MabiPacket(Op.ItemDestroyR, creature.Id).PutByte(1));
		}

		private void HandleItemPickUp(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			var itemId = packet.GetLong();

			byte result = 2;

			var item = WorldManager.Instance.GetItemById(itemId);
			if (item != null)
			{
				if (item.StackType == BundleType.Stackable)// && item.Type == ItemType.Sac)
				{
					foreach (var invItem in creature.Items)
					{
						if (item.Info.Class == invItem.Info.Class || item.Info.Class == invItem.StackItem)
						{
							if (invItem.Info.Amount + item.Info.Amount > invItem.StackMax)
							{
								if (invItem.Info.Amount < invItem.StackMax)
								{
									item.Info.Amount -= (ushort)(invItem.StackMax - invItem.Info.Amount);
									invItem.Info.Amount = invItem.StackMax;

									client.Send(PacketCreator.ItemAmount(creature, invItem));

									result = 1;
								}
							}
							else
							{
								invItem.Info.Amount += item.Info.Amount;
								item.Info.Amount = 0;

								WorldManager.Instance.RemoveItem(item);
								client.Send(PacketCreator.ItemAmount(creature, invItem));

								result = 1;
							}
						}
					}
				}

				if (item.Info.Amount > 0 || (item.Type == ItemType.Sac && item.StackType == BundleType.Sac))
				{
					var pos = creature.GetFreeItemSpace(item, Pocket.Inventory);
					if (pos != null)
					{
						WorldManager.Instance.RemoveItem(item);

						item.Move(Pocket.Inventory, pos.X, pos.Y);
						creature.Items.Add(item);

						client.Send(PacketCreator.ItemInfo(creature, item));

						result = 1;
					}
					else
					{
						client.Send(PacketCreator.SystemMessage(creature, "Not enough space."));
					}
				}
			}

			var response = new MabiPacket(Op.ItemPickUpR, creature.Id);
			response.PutByte(result);
			client.Send(response);
		}

		private void HandleItemSplit(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			var itemId = packet.GetLong();
			var amount = packet.GetShort();
			var unk1 = packet.GetByte();

			packet = new MabiPacket(Op.ItemSplitR, creature.Id);

			var item = creature.GetItem(itemId);
			if (item != null && item.StackType != BundleType.None)
			{
				if (item.Info.Amount < amount)
					amount = item.Info.Amount;

				item.Info.Amount -= amount;

				MabiItem splitItem;
				if (item.StackItem == 0)
					splitItem = new MabiItem(item);
				else
					splitItem = new MabiItem(item.StackItem);
				splitItem.Info.Amount = amount;
				splitItem.Move(Pocket.Cursor, 0, 0);
				creature.Items.Add(splitItem);

				// New item on cursor
				client.Send(PacketCreator.ItemInfo(creature, splitItem));

				// Update amount or remove
				if (item.Info.Amount > 0 || item.StackItem != 0)
				{
					client.Send(PacketCreator.ItemAmount(creature, item));
				}
				else
				{
					creature.Items.Remove(item);
					client.Send(PacketCreator.ItemRemove(creature, item));
				}

				packet.PutByte(1);
			}
			else
			{
				packet.PutByte(0);
			}

			client.Send(packet);
		}

		private void HandleSwitchSet(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			var set = packet.GetByte();

			creature.StopMove();

			// TODO: Check this, GetItemInPocket doesn't return the correct stuff.
			creature.WeaponSet = set;
			WorldManager.Instance.CreatureSwitchSet(creature);

			var response = new MabiPacket(Op.SwitchSetR, creature.Id);
			response.PutByte(1);
			client.Send(response);
		}

		private void HandleItemStateChange(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			// This might not be entirely correct, but works fine.
			// Robe is opened first, Helm secondly, then Robe and Helm are both closed.

			var firstTarget = packet.GetByte();
			var secondTarget = packet.GetByte();
			var unk = packet.GetByte();

			foreach (var target in new byte[] { firstTarget, secondTarget })
			{
				if (target > 0)
				{
					var item = creature.GetItemInPocket((Pocket)target);
					if (item != null)
					{
						item.Info.FigureA = (byte)(item.Info.FigureA == 1 ? 0 : 1);
						WorldManager.Instance.CreatureEquip(creature, item);
					}
				}
			}

			MabiPacket response = new MabiPacket(Op.ItemStateChangeR, creature.Id);
			client.Send(response);
		}

		private void HandleEnterRegion(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
			{
				Logger.Warning("Creature not in account.");
				return;
			}

			// TODO: Maybe check if this action is valid.

			MabiPacket p;

			p = new MabiPacket(Op.CharacterUnlock, creature.Id);
			p.PutInt(0xEFFFFFFE);
			client.Send(p);

			p = new MabiPacket(Op.EnterRegionR, 0x1000000000000001);
			p.PutByte(1);
			p.PutLong(creature.Id);
			p.PutLong(DateTime.Now);
			client.Send(p);

			WorldManager.Instance.AddCreature(creature);

			// Player logs in
			if (creature == client.Character)
			{
				// List of all entities in range
				var entities = WorldManager.Instance.GetEntitiesInRange(creature);
				if (entities.Count > 0)
				{
					client.Send(PacketCreator.EntitiesAppear(entities));
				}

				p = new MabiPacket(Op.WarpRegion, creature.Id);
				var pos = creature.GetPosition();
				p.PutByte(1);
				p.PutInt(creature.Region);
				p.PutInt(pos.X);
				p.PutInt(pos.Y);
				client.Send(p);
			}
		}

		private void HandleSkillPrepare(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			var skillId = packet.GetShort();
			var parameters = packet.GetStringOrEmpty();

			if (parameters.Length > 0)
			{
				var match = Regex.Match(parameters, "ITEMID:[0-9]+:([0-9]+);");
				if (match.Success)
				{
					var itemId = Convert.ToUInt64(match.Groups[1].Value);
					var item = creature.GetItem(itemId);
					if (item == null)
						return;

					creature.ActiveSkillItem = item;
				}
			}

			MabiSkill skill; SkillHandler handler;
			SkillManager.CheckOutSkill(creature, skillId, out skill, out handler);
			if (skill == null || handler == null)
			{
				client.Send(new MabiPacket(Op.SkillPrepare, creature.Id).PutShort(0));
				return;
			}

			var castTime = skill.RankInfo.LoadTime;
			creature.ActiveSkillPrepareEnd = DateTime.Now.AddMilliseconds(castTime);

			// Check Mana
			if (skill.RankInfo.ManaCost > 0)
			{
				if (creature.Mana < skill.RankInfo.ManaCost)
				{
					client.Send(PacketCreator.SystemMessage(creature, "Insufficient Mana"));
					client.Send(new MabiPacket(Op.SkillPrepare, creature.Id).PutShort(0));
					return;
				}

				creature.Mana -= skill.RankInfo.ManaCost;
				WorldManager.Instance.CreatureStatsUpdate(creature);
			}

			// Check Stamina
			if (skill.RankInfo.StaminaCost > 0)
			{
				if (creature.Stamina < skill.RankInfo.StaminaCost)
				{
					client.Send(PacketCreator.SystemMessage(creature, "Insufficient Stamina"));
					client.Send(new MabiPacket(Op.SkillPrepare, creature.Id).PutShort(0));
					return;
				}

				creature.Stamina -= skill.RankInfo.StaminaCost;
				WorldManager.Instance.CreatureStatsUpdate(creature);
			}

			var result = handler.Prepare(creature, skill);

			if ((result & SkillResults.Okay) == 0 || (result & SkillResults.NoReply) != 0)
				return;

			var r = new MabiPacket(Op.SkillPrepare, creature.Id);
			r.PutShort(skillId);
			if (parameters.Length > 0)
				r.PutString(parameters);
			else
				r.PutInt(castTime);
			client.Send(r);
		}

		private void HandleSkillReady(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			var skillId = packet.GetShort();
			var parameters = packet.GetStringOrEmpty();

			MabiSkill skill; SkillHandler handler;
			SkillManager.CheckOutSkill(creature, skillId, out skill, out handler);
			if (skill == null || handler == null)
				return;

			var result = handler.Ready(creature, skill);

			if ((result & SkillResults.Okay) == 0)
				return;

			var r = new MabiPacket(Op.SkillReady, creature.Id);
			r.PutShort(creature.ActiveSkillId);
			if (parameters.Length > 0)
				r.PutString(parameters);
			client.Send(r);
		}

		private void HandleSkillUse(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			var skillId = packet.GetShort();
			var targetId = packet.GetLong();
			var unk1 = packet.GetInt();
			var unk2 = packet.GetInt();

			MabiCreature target = null;
			// Windmill sends a huge nr as target id... a sign!? O___O
			if (targetId < 0x3000000000000000)
			{
				if (targetId != creature.Id)
					target = WorldManager.Instance.GetCreatureById(targetId);
				else
					target = creature;

				if (target == null)
				{
					client.Send(PacketCreator.SystemMessage(creature, "Invalid target"));
					client.Send(new MabiPacket(Op.SkillUse, creature.Id).PutShort(0));
					return;
				}
			}

			creature.ActiveSkillTarget = target;

			MabiSkill skill; SkillHandler handler;
			SkillManager.CheckOutSkill(creature, skillId, out skill, out handler);
			if (skill == null || handler == null)
				return;

			var result = handler.Use(creature, target, skill);

			if ((result & SkillResults.InsufficientStamina) != 0)
				client.Send(PacketCreator.SystemMessage(creature, "Insufficient Stamina"));

			if ((result & SkillResults.InvalidTarget) != 0)
				client.Send(PacketCreator.SystemMessage(creature, "Invalid target"));

			if ((result & SkillResults.NoReply) != 0)
				return;

			if ((result & SkillResults.Okay) == 0)
			{
				client.Send(new MabiPacket(Op.SkillUse, creature.Id).PutShort(0));
				return;
			}

			var r = new MabiPacket(Op.SkillUse, creature.Id);
			r.PutShort(skillId);
			r.PutLong(targetId);
			r.PutInt(unk1);
			r.PutInt(unk2);
			client.Send(r);

			r = new MabiPacket(0x6992, creature.Id);
			r.PutBytes(0, 1, 0);
			r.PutShort(skillId);
			client.Send(r);
		}

		private void HandleSkillComplete(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			var skillId = packet.GetShort();

			MabiSkill skill; SkillHandler handler;
			SkillManager.CheckOutSkill(creature, skillId, out skill, out handler);
			if (skill == null || handler == null)
				return;

			var result = handler.Complete(creature, skill);

			if ((result & SkillResults.Okay) == 0)
				return;

			var r = new MabiPacket(Op.SkillComplete, creature.Id);
			r.PutShort(skillId);
			client.Send(r);

			if (creature.ActiveSkillStacks > 0 && skill.RankInfo.Stack > 1)
			{
				// Send new ready packet if there are stacks left.
				client.Send(new MabiPacket(Op.SkillReady, creature.Id).PutShort(creature.ActiveSkillId));
			}
			else
			{
				creature.ActiveSkillId = 0;
				creature.ActiveSkillTarget = null;
			}
		}

		private void HandleSkillCancel(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			WorldManager.Instance.CreatureSkillCancel(creature);
		}

		private void HandleSkillStart(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			var skillId = packet.GetShort();
			var parameters = packet.GetStringOrEmpty();

			MabiSkill skill; SkillHandler handler;
			SkillManager.CheckOutSkill(creature, skillId, out skill, out handler);
			if (skill == null || handler == null)
			{
				client.Send(new MabiPacket(Op.SkillStart, creature.Id).PutShort(skillId));
				return;
			}

			var result = handler.Start(creature, skill);

			if ((result & SkillResults.Okay) == 0)
				return;

			var r = new MabiPacket(Op.SkillStart, creature.Id);
			r.PutShort(skillId);
			r.PutString("");
			client.Send(r);
		}

		private void HandleSkillStop(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			var skillId = packet.GetShort();
			var parameters = packet.GetStringOrEmpty();

			MabiSkill skill; SkillHandler handler;
			SkillManager.CheckOutSkill(creature, skillId, out skill, out handler);
			if (skill == null || handler == null)
				return;

			var result = handler.Stop(creature, skill);

			//if ((result & SkillResults.Okay) == 0)
			//    return;

			var r = new MabiPacket(Op.SkillStop, creature.Id);
			r.PutShort(skillId);
			r.PutByte(1);
			client.Send(r);
		}

		private void HandleChangeStance(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			var mode = packet.GetByte();

			// Clear target?
			if (mode == 0)
				WorldManager.Instance.CreatureSetTarget(creature, null);

			// Send info
			creature.BattleState = mode;
			WorldManager.Instance.CreatureChangeStance(creature);

			// Unlock
			client.Send(new MabiPacket(Op.ChangeStanceR, creature.Id));
		}

		private void HandleShopBuyItem(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			if (!client.NPCSession.IsValid())
				return;

			var itemId = packet.GetLong();
			var targetPocket = packet.GetByte(); // 0:cursor, 1:inv
			var unk2 = packet.GetByte(); // storage gold?

			var newItem = client.NPCSession.Target.Script.Shop.GetItem(itemId);
			if (newItem == null)
				return;

			if (targetPocket == 0)
			{
				newItem.Move(Pocket.Cursor, 0, 0);
			}
			else if (targetPocket == 1)
			{
				var pos = creature.GetFreeItemSpace(newItem, Pocket.Inventory);
				if (pos != null)
				{
					newItem.Move(Pocket.Inventory, pos.X, pos.Y);
				}
				else
				{
					newItem = null;
				}
			}

			// The client expects the price for a full stack to be sent,
			// so we have to calculate the actual price here.
			var price = newItem.OptionInfo.Price;
			if (newItem.StackType == BundleType.Stackable)
				price = (uint)(price / newItem.StackMax * newItem.Count);

			var p = new MabiPacket(Op.ShopBuyItemR, creature.Id);
			if (creature.HasGold(price) && newItem != null)
			{
				creature.RemoveGold(price);

				creature.Items.Add(newItem);
				client.Send(PacketCreator.ItemInfo(creature, newItem));
			}
			else
			{
				client.Send(PacketCreator.MsgBox(creature, "Insufficient amount of gold."));

				p.PutByte(0);
			}
			client.Send(p);
		}

		private void HandleShopSellItem(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			if (!client.NPCSession.IsValid())
				return;

			var itemId = packet.GetLong();
			var unk1 = packet.GetByte();

			var item = creature.GetItem(itemId);
			if (item == null)
				return;

			var p = new MabiPacket(Op.ShopSellItemR, creature.Id);

			creature.Items.Remove(item);
			client.Send(PacketCreator.ItemRemove(creature, item));

			var sellingPrice = item.OptionInfo.SellingPrice;

			if (item.StackType == BundleType.Sac)
			{
				var stackItem = MabiData.ItemDb.Find(item.StackItem);
				if (stackItem != null)
				{
					sellingPrice += (stackItem.SellingPrice / stackItem.StackMax) * item.Info.Amount;
				}
			}
			else if (item.StackType == BundleType.Stackable)
			{
				sellingPrice = (uint)(sellingPrice / item.StackMax * item.Count);
			}

			creature.GiveGold(sellingPrice);

			// TODO: There could be an optional tab for rebuying sold things.

			client.Send(p);
		}

		private void HandleTitleChange(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			var title = packet.GetShort();

			// Make sure the character has this title enabled
			var character = creature as MabiPC;
			if (character.Titles.ContainsKey(title) && character.Titles[title])
			{
				creature.Title = title;
				WorldManager.Instance.CreatureChangeTitle(creature);
			}

			var answer = new MabiPacket(Op.ChangeTitleR, creature.Id);
			answer.PutByte(1);
			answer.PutByte(0);
			client.Send(answer);
		}

		private void HandlePetSummon(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			var petId = packet.GetLong();
			var unk1 = packet.GetByte();

			MabiPacket p;

			var pet = client.Account.Pets.FirstOrDefault(a => a.Id == petId);
			if (pet == null)
			{
				p = new MabiPacket(Op.PetSummonR, creature.Id);
				p.PutByte(0);
				p.PutLong(petId);
				client.Send(p);
				return;
			}

			// Set pet position near the summoner
			var pos = creature.GetPosition();
			var rand = RandomProvider.Get();
			pos.X = (uint)(pos.X + rand.Next(-350, 351));
			pos.Y = (uint)(pos.Y + rand.Next(-350, 351));
			pet.SetLocation(creature.Region, pos.X, pos.Y);
			pet.Direction = (byte)rand.Next(255);

			pet.Owner = creature;
			client.Creatures.Add(pet);

			pet.Save = true;

			p = new MabiPacket(Op.PetRegister, creature.Id);
			p.PutLong(pet.Id);
			p.PutByte(2);
			client.Send(p);

			p = new MabiPacket(Op.PetSummonR, creature.Id);
			p.PutByte(1);
			p.PutLong(petId);
			client.Send(p);

			p = new MabiPacket(Op.CharacterLock, petId);
			p.PutInt(0xEFFFFFFE);
			p.PutInt(0);
			client.Send(p);

			client.Send(PacketCreator.EnterRegionPermission(pet));

			WorldManager.Instance.Effect(creature, 29, creature.Region, pos.X, pos.Y);
		}

		private void HandlePetUnsummon(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			var petId = packet.GetLong();

			MabiPacket p;

			var pet = client.Creatures.FirstOrDefault(a => a.Id == petId);
			if (pet == null)
			{
				p = new MabiPacket(Op.PetUnsummonR, creature.Id);
				p.PutByte(0);
				p.PutLong(petId);
				client.Send(p);
				return;
			}

			client.Creatures.Remove(pet);

			var pos = pet.GetPosition();
			WorldManager.Instance.Effect(pet, 29, pet.Region, pos.X, pos.Y);
			WorldManager.Instance.RemoveCreature(pet);

			if (pet.Owner.Vehicle == pet)
			{
				WorldManager.Instance.VehicleUnbind(pet.Owner, pet);
				pet.Owner.Vehicle = null;
			}

			// ?
			p = new MabiPacket(Op.PetUnRegister, creature.Id);
			p.PutLong(pet.Id);
			WorldManager.Instance.Broadcast(p, SendTargets.Range, creature);

			// Disappear
			p = new MabiPacket(Op.Disappear, 0x1000000000000001);
			p.PutLong(pet.Id);
			client.Send(p);

			// Result
			p = new MabiPacket(Op.PetUnsummonR, creature.Id);
			p.PutByte(1);
			p.PutLong(petId);
			client.Send(p);
		}

		private void HandlePetMount(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			var petId = packet.GetLong();

			MabiPacket p;

			var pet = client.Account.Pets.FirstOrDefault(a => a.Id == petId);
			if (pet == null || pet.IsDead() || pet.RaceInfo.VehicleType == 0 || pet.RaceInfo.VehicleType == 17)
			{
				p = new MabiPacket(Op.PetMountR, creature.Id);
				p.PutByte(0);
				client.Send(p);
				return;
			}

			creature.Vehicle = pet;

			WorldManager.Instance.VehicleBind(creature, pet);

			p = new MabiPacket(Op.PetMountR, creature.Id);
			p.PutByte(1);
			client.Send(p);
		}

		private void HandlePetUnmount(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			MabiPacket p;

			if (creature.Vehicle == null)
			{
				p = new MabiPacket(Op.PetUnmountR, creature.Id);
				p.PutByte(0);
				client.Send(p);
				return;
			}

			WorldManager.Instance.VehicleUnbind(creature, creature.Vehicle);

			creature.Vehicle = null;

			p = new MabiPacket(Op.PetUnmountR, creature.Id);
			p.PutByte(1);
			client.Send(p);
		}

		private void HandleTouchProp(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			byte success = 0;

			// TODO: If all portals are props, and not all props are portals, what's this?

			var portalId = packet.GetLong();
			var portalInfo = MabiData.PortalDb.Find(portalId);
			if (portalInfo != null)
			{
				if (creature.Region == portalInfo.Region && WorldManager.InRange(creature, portalInfo.X, portalInfo.Y, 1500))
				{
					success = 1;
					client.Warp(portalInfo.ToRegion, portalInfo.ToX, portalInfo.ToY);
				}
			}
			else
			{
				Logger.Unimplemented("Unknown prop: " + portalId.ToString());
#if DUNGEON_TEST
				var pos = creature.GetPosition();
				//client.Send(new MabiPacket(Op.WARP_ENTER, creature.Id).PutByte(1).PutInts(creature.Region, pos.X, pos.Y));
				//
				//
				WorldManager.Instance.CreatureLeaveRegion(creature);
				client.Send(new MabiPacket(Op.CharacterLock, creature.Id).PutInts(0xEFFFFFFE, 0));

				creature.SetLocation(10032, 5992, 5614);
				client.Send(PacketCreator.EnterRegionPermission(creature));

				success = 1;
#endif
			}

			var p = new MabiPacket(Op.TouchPropR, creature.Id);
			p.PutByte(success);
			client.Send(p);
		}

		private void HandleMove(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			var x = packet.GetInt();
			var y = packet.GetInt();

			var pos = creature.GetPosition();
			var dest = new MabiVertex(x, y);

			// TODO: Collision

			var walking = (packet.Op == 0x0FF23431);

			// TODO: Update creature position on unmount?
			creature.StartMove(dest, walking);
			if (creature.Vehicle != null)
				creature.Vehicle.StartMove(dest, walking);

			WorldManager.Instance.CreatureMove(creature, pos, dest, walking);
		}

		private void HandleCombatSetTarget(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			var targetId = packet.GetLong();
			var unk1 = packet.GetByte();
			var unk2 = packet.GetString();

			MabiCreature target = null;
			if (targetId > 0)
			{
				target = WorldManager.Instance.GetCreatureById(targetId);
				if (target == null)
				{
					Logger.Warning("Target '" + targetId + "' doesn't exist.");
					return;
				}
			}

			creature.Target = target;
			WorldManager.Instance.CreatureSetTarget(creature, target);
		}

		private void HandleCombatAttack(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			if (creature.Vehicle != null)
				creature = creature.Vehicle;

			// TODO: Check if mount is able to attack anything? (this is done with a status)

			var attackResult = SkillResults.Failure;
			var targetId = packet.GetLong();

			var handler = SkillManager.GetHandler(SkillConst.MeleeCombatMastery);
			if (handler == null)
				return;

			var target = WorldManager.Instance.GetCreatureById(targetId);
			if (target != null)
			{
				attackResult = handler.Use(creature, target, null); // MabiCombat.MeleeAttack(creature, target);
			}

			var answer = new MabiPacket(Op.CombatAttackR, creature.Id);

			if (attackResult == SkillResults.Okay)
			{
				answer.PutByte(1);
			}
			else if (attackResult == SkillResults.OutOfRange)
			{
				// Let the creature run to the target.
				var creaturePos = creature.GetPosition();
				var targetPos = target.GetPosition();

				answer.PutByte(100);
				answer.PutLong(target.Id);
				answer.PutByte(0);
				answer.PutByte(0);
				answer.PutInt(creaturePos.X);
				answer.PutInt(creaturePos.Y);
				answer.PutByte(0);
				answer.PutInt(targetPos.X);
				answer.PutInt(targetPos.Y);
				answer.PutString("");
			}
			else if (attackResult == SkillResults.Failure)
			{
				// No target, no skill, message should be more clear.
				client.Send(PacketCreator.SystemMessage(creature, "Something went wrong here, sry =/"));
			}
			else
			{
				// Stunned, missing handler, failure, should be more clear.
				answer.PutByte(0);
			}

			client.Send(answer);

			if (target != null)
				client.Send(new MabiPacket(Op.StunMeter, creature.Id).PutLong(target.Id).PutByte(1).PutFloat(target.Stun));
		}

		public void HandleDeadMenu(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null && creature.IsDead())
				return;

			var response = new MabiPacket(Op.DeadMenuR, creature.Id);
			response.PutByte(1);
			response.PutString("town;here;;stay");
			response.PutInt(0);
			response.PutInt(0);
			client.Send(response);
		}

		public void HandleRevive(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null && creature.IsDead())
				return;

			// 1 = Town, 2 = Here, 9 = Wait
			var option = packet.GetInt();

			uint region = 0, x = 0, y = 0;

			// TODO: Town etc support.
			if (option == 9)
			{
				var feather = new MabiPacket(Op.DeadFeather, creature.Id);
				feather.PutShort((ushort)(creature.WaitingForRes ? 4 : 5));
				feather.PutInt(0);
				feather.PutInt(1);
				feather.PutInt(2);
				feather.PutInt(9);
				if (!creature.WaitingForRes)
					feather.PutInt(10);
				feather.PutByte(0);
				WorldManager.Instance.Broadcast(feather, SendTargets.Range, creature);

				creature.WaitingForRes = !creature.WaitingForRes;
			}
			else
			{
				WorldManager.Instance.CreatureRevive(creature);

				var pos = creature.GetPosition();
				region = creature.Region;
				x = pos.X;
				y = pos.Y;
			}

			var response = new MabiPacket(Op.Revived, creature.Id);
			response.PutInt(1);
			response.PutInt(region);
			response.PutInt(x);
			response.PutInt(y);
			client.Send(response);
		}

		public void HandleMailsRequest(WorldClient client, MabiPacket packet)
		{
			client.Send(new MabiPacket(Op.MailsRequestR, client.Character.Id));

			// Mails
			// client.Send(new MabiPacket(0x7255, client.Character.Id).PutInt(3));
		}

		public void HandleSosButton(WorldClient client, MabiPacket packet)
		{
			// Enable = 1, Disable = 0
			client.Send(new MabiPacket(Op.SosButtonR, client.Character.Id).PutByte(false));
		}

		public void HandleAreaChange(WorldClient client, MabiPacket packet)
		{
			var unk1 = packet.GetLong(); // 00B000010003007C ?
			var unk2 = packet.GetInt(); // Area?
			var unk3 = packet.GetString();

			// TODO: Do something with this?
		}

		public void HandleStunMeterRequest(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null && creature.IsDead())
				return;

			var targetId = packet.GetLong();

			// TODO: Check target, get target, send back actual value

			var response = new MabiPacket(Op.StunMeter, creature.Id);
			response.PutLong(targetId);
			if (targetId > 0)
			{
				response.PutByte(1);
				response.PutFloat(50);
			}
			else
				response.PutByte(0);

			client.Send(response);
		}

		public void HandleStunMeterDummy(WorldClient client, MabiPacket packet)
		{
			// Something about the stun meter I guess.
		}

		public void HandleHomesteadInfo(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null || creature.IsDead())
				return;

			client.Send(new MabiPacket(Op.HomesteadInfoRequestR, creature.Id).PutBytes(0, 0, 1));

			// Seems to be only called on login, good place for the MOTD.
			if (WorldConf.Motd != string.Empty)
				client.Send(PacketCreator.ServerMessage(client.Character, WorldConf.Motd));
		}

		public void HandleHitProp(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null || creature.IsDead())
				return;

			var targetId = packet.GetLong();

			// TODO: Check for prop to exist?
			// TODO: Drops. Hard, we'll have to create a prop database...

			var pos = creature.GetPosition();
			WorldManager.Instance.Broadcast(new MabiPacket(Op.HittingProp, creature.Id).PutLong(targetId).PutInt(2000).PutFloat(pos.X).PutFloat(pos.Y), SendTargets.Region, creature);

			client.Send(new MabiPacket(Op.HitPropR, creature.Id).PutByte(1));

			Logger.Unimplemented("Unknown prop ID: " + targetId);
		}

		public void HandleMoonGateRequest(WorldClient client, MabiPacket packet)
		{
			//001 [................]  String : _moongate_tara_west
			//002 [................]  String : _moongate_tirchonaill

			// Send no gates for now.
			client.Send(new MabiPacket(Op.MoonGateRequestR, 0x3000000000000000));
		}

		public void HandleOpenItemShop(WorldClient client, MabiPacket packet)
		{
			// 1 = succes?, test = key passed to the url
			client.Send(new MabiPacket(0xA44E, client.Character.Id).PutByte(1).PutString(client.Account.Username));
		}

		public void HandleVisualChat(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null || !WorldConf.EnableVisual)
				return;

			var url = packet.GetString();
			var width = packet.GetShort();
			var height = packet.GetShort();

			var p = new MabiPacket(Op.VisualChat, creature.Id);
			p.PutString(creature.Name);
			p.PutString(url);
			p.PutShorts(width, height);
			p.PutByte(0);
			WorldManager.Instance.Broadcast(p, SendTargets.Range, creature);
		}

		public void HandleViewEquipment(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			var p = new MabiPacket(Op.ViewEquipmentR, client.Character.Id);

			var targetID = packet.GetLong();
			var target = WorldManager.Instance.GetCreatureById(targetID);
			if (target == null /* || TODO: Check visibility. */)
			{
				p.PutByte(0);
				client.Send(p);
				return;
			}

			var items = target.Items.FindAll(a => a.IsEquipped() && a.Type != ItemType.Hair && a.Type != ItemType.Face);

			p.PutByte(1);
			p.PutLong(targetID);
			p.PutInt((ushort)items.Count);
			foreach (var item in items)
				item.AddPrivateEntityData(p);

			client.Send(p);
		}

		public void HandleSkillAdvance(WorldClient client, MabiPacket packet)
		{
			var creature = client.Creatures.FirstOrDefault(a => a.Id == packet.Id);
			if (creature == null)
				return;

			var skillId = packet.GetShort();
			var skill = creature.GetSkill(skillId);
			if (skill == null || !skill.IsRankable)
				return;

			creature.GiveSkill(skill.Id, skill.Rank + 1);
		}
	}
}
