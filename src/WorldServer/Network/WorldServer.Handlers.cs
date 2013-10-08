// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see licence.txt in the main folder

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aura.Data;
using Aura.Shared.Const;
using Aura.Shared.Database;
using Aura.Shared.Network;
using Aura.Shared.Util;
using Aura.World.Database;
using Aura.World.Events;
using Aura.World.Player;
using Aura.World.Scripting;
using Aura.World.Skills;
using Aura.World.Util;
using Aura.World.World;
using Aura.World.World.Guilds;
using Aura.Shared.World;

namespace Aura.World.Network
{
	public partial class WorldServer : BaseServer<WorldClient>
	{
		protected override void OnServerStartUp()
		{
			this.RegisterPacketHandler(Op.WorldLogin, HandleLogin);
			this.RegisterPacketHandler(Op.WorldDisconnect, HandleDisconnect);
			this.RegisterPacketHandler(Op.WorldCharInfoRequest, HandleCharacterInfoRequest);

			this.RegisterPacketHandler(Op.Walk, HandleMove);
			this.RegisterPacketHandler(Op.Run, HandleMove);
			this.RegisterPacketHandler(Op.Chat, HandleChat);
			this.RegisterPacketHandler(Op.TakeOff, HandleTakeOff);
			this.RegisterPacketHandler(Op.FlyTo, HandleFlyTo);
			this.RegisterPacketHandler(Op.Land, HandleLand);
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
			this.RegisterPacketHandler(Op.UmbrellaJump, HandleUmbrellaJump);
			this.RegisterPacketHandler(Op.UmbrellaLand, HandleUmbrellaLand);
			this.RegisterPacketHandler(Op.DyePaletteReq, HandleDyePaletteReq);
			this.RegisterPacketHandler(Op.DyePickColor, HandleDyePickColor);

			this.RegisterPacketHandler(Op.QuestComplete, HandleQuestComplete);
			this.RegisterPacketHandler(Op.QuestGiveUp, HandleQuestGiveUp);

			this.RegisterPacketHandler(Op.NPCTalkStart, HandleNPCTalkStart);
			this.RegisterPacketHandler(Op.NPCTalkEnd, HandleNPCTalkEnd);
			this.RegisterPacketHandler(Op.NPCTalkPartner, HandleNPCTalkPartner);
			this.RegisterPacketHandler(Op.NPCTalkKeyword, HandleNPCTalkKeyword);
			this.RegisterPacketHandler(Op.NPCTalkSelect, HandleNPCTalkSelect);
			this.RegisterPacketHandler(Op.ShopBuyItem, HandleShopBuyItem);
			this.RegisterPacketHandler(Op.ShopSellItem, HandleShopSellItem);

			this.RegisterPacketHandler(Op.LeaveSoulStream, HandleLeaveSoulStream);

			this.RegisterPacketHandler(Op.CancelBeautyShop, HandleCancelBeautyShop);

			this.RegisterPacketHandler(Op.CutsceneFinished, HandleCutsceneFinished);

			this.RegisterPacketHandler(Op.PartyCreate, HandlePartyCreate);
			this.RegisterPacketHandler(Op.PartyJoin, HandlePartyJoin);
			this.RegisterPacketHandler(Op.PartyLeave, HandlePartyLeave);
			this.RegisterPacketHandler(Op.PartyRemove, HandlePartyRemove);
			this.RegisterPacketHandler(Op.PartyChangeSetting, HandlePartyChangeSettings);
			this.RegisterPacketHandler(Op.PartyChangePassword, HandlePartyChangePassword);
			this.RegisterPacketHandler(Op.PartyChangeLeader, HandlePartyChangeLeader);
			this.RegisterPacketHandler(Op.PartyWantedShow, HandlePartyWantedShow);
			this.RegisterPacketHandler(Op.PartyWantedHide, HandlePartyWantedHide);
			this.RegisterPacketHandler(Op.PartyChangeFinish, HandlePartyChangeFinish);
			this.RegisterPacketHandler(Op.PartyChangeExp, HandlePartyChangeExp);

			this.RegisterPacketHandler(Op.GetMails, HandleGetMails);
			this.RegisterPacketHandler(Op.ConfirmMailRecipent, HandleConfirmMailRecipient);
			this.RegisterPacketHandler(Op.SendMail, HandleSendMail);
			this.RegisterPacketHandler(Op.MarkMailRead, HandleMarkMailRead);
			this.RegisterPacketHandler(Op.ReturnMail, HandleReturnMail);
			this.RegisterPacketHandler(Op.RecallMail, HandleRecallMail);
			this.RegisterPacketHandler(Op.RecieveMailItem, HandleRecieveMailItem);
			this.RegisterPacketHandler(Op.DeleteMail, HandleDeleteMail);

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

			this.RegisterPacketHandler(Op.CombatSetAim, HandleCombatSetAim);
			this.RegisterPacketHandler(0x791F, HandleCombatSetAim);

			this.RegisterPacketHandler(Op.PetSummon, HandlePetSummon);
			this.RegisterPacketHandler(Op.PetUnsummon, HandlePetUnsummon);
			this.RegisterPacketHandler(Op.PetMount, HandlePetMount);
			this.RegisterPacketHandler(Op.PetUnmount, HandlePetUnmount);

			this.RegisterPacketHandler(Op.TouchProp, HandleTouchProp);
			this.RegisterPacketHandler(Op.HitProp, HandleHitProp);

			this.RegisterPacketHandler(Op.EnterRegion, HandleEnterRegion);
			this.RegisterPacketHandler(Op.AreaChange, HandleAreaChange);

			this.RegisterPacketHandler(Op.OptionSet, HandleOptionSet);

			this.RegisterPacketHandler(Op.ChangeTitle, HandleChangeTitle);
			this.RegisterPacketHandler(Op.TalentTitleChange, HandleTalentTitleChange);
			this.RegisterPacketHandler(Op.MailsRequest, HandleMailsRequest);
			this.RegisterPacketHandler(Op.SosButton, HandleSosButton);
			this.RegisterPacketHandler(Op.MoonGateRequest, HandleMoonGateRequest);
			this.RegisterPacketHandler(Op.UseGesture, HandleGesture);
			this.RegisterPacketHandler(Op.HomesteadInfoRequest, HandleHomesteadInfo);
			this.RegisterPacketHandler(Op.OpenItemShop, HandleOpenItemShop);

			this.RegisterPacketHandler(Op.ConvertGp, HandleConvertGp);
			this.RegisterPacketHandler(Op.ConvertGpConfirm, HandleConvertGpConfirm);
			this.RegisterPacketHandler(Op.GuildDonate, HandleGuildDonate);
			this.RegisterPacketHandler(Op.GuildApply, HandleGuildApply);

			this.RegisterPacketHandler(Op.CollectionRequest, HandleCollectionRequest);
			this.RegisterPacketHandler(Op.ShamalaTransformationUse, HandleShamalaTransformation);
			this.RegisterPacketHandler(Op.ShamalaTransformationEnd, HandleShamalaTransformationEnd);

			this.RegisterPacketHandler(Op.GMCPSummon, HandleGMCPSummon);
			this.RegisterPacketHandler(Op.GMCPMoveToChar, HandleGMCPMoveToChar);
			this.RegisterPacketHandler(Op.GMCPMove, HandleGMCPMove);
			this.RegisterPacketHandler(Op.GMCPRevive, HandleGMCPRevive);
			this.RegisterPacketHandler(Op.GMCPInvisibility, HandleGMCPInvisibility);
			this.RegisterPacketHandler(Op.GMCPExpel, HandleGMCPExpel);
			this.RegisterPacketHandler(Op.GMCPBan, HandleGMCPBan);
			this.RegisterPacketHandler(Op.GMCPClose, (c, p) => { }); // TODO: Maybe add this to a gm log.
			this.RegisterPacketHandler(Op.GMCPNPCList, HandleGMCPListNPCs);

			this.RegisterPacketHandler(Op.Internal.ServerIdentify, HandleServerIdentify);
			this.RegisterPacketHandler(Op.ChannelStatus, HandleChannelStatus);

            this.RegisterPacketHandler(Op.ShadowMissionList, HandleShadowMissionList);
            this.RegisterPacketHandler(Op.ShadowMissionMap, HandleShadowMissionMap);
            this.RegisterPacketHandler(Op.ShadowMissionAccept, HandleShadowMissionAccept);
            this.RegisterPacketHandler(Op.ShadowMissionExit, HandleShadowMissionExit);

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
				// XXX: No idea what this does anymore...
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

			this.RegisterPacketHandler(0xAAEC, (client, packet) =>
			{
				// Apperantly sent when switching weapon sets, but only if
				// there are items equipped on that set.
				// Also sent when pressing ESC?
			});
		}

		private void HandleLogin(WorldClient client, MabiPacket packet)
		{
			var userName = packet.GetString();
			// [160XXX] Double account name
			{
				packet.GetString();
			}
			var seedKey = packet.GetLong();
			var charID = packet.GetLong();
			//byte unk1 = packet.GetByte();

			if (client.State != ClientState.LoggingIn)
				return;

			if (client.Account == null)
			{
				if (!MabiDb.Instance.IsSessionKey(userName, seedKey))
				{
					Logger.Warning("Invalid session key.");
					client.Kill();
					return;
				}

				client.Account = WorldDb.Instance.GetAccount(userName);
			}

			MabiPC character = client.Account.Characters.FirstOrDefault(a => a.Id == charID);
			if (character == null && (character = client.Account.Pets.FirstOrDefault(a => a.Id == charID)) == null)
			{
				Logger.Warning("Creature not in account.");
				return;
			}

			character.Save = true;

			client.Creatures.Add(character);
			client.Character = character;
			client.Character.Client = client;

			// Purpose not clear, doesn't seem necessary.
			//foreach (var skill in creature.Skills)
			//{
			//    client.Send(new MabiPacket(Op.SkillInfo, creature.Id).PutBin(skill.Value.Info));
			//    client.Send(new MabiPacket(0x699E, creature.Id).PutShort(skill.Key).PutByte(1));
			//}

			Send.LoginResponse(client, character);

			if (character.Has(CreatureStates.EverEnteredWorld))
			{
				Send.CharacterLock(client, character);
				Send.EnterRegionPermission(client, character);
			}
			else
			{
				//var pp = new MabiPacket(0x000065C2, 0x1000000000000001);
				//pp.PutByte(1);
				//pp.PutByte(0);
				//client.Send(pp);

				// Set location, so character can talk to Nao,
				// and doesn't appear in the world.
				character.SetLocation(1000, 0, 0);

				// TODO: return_to location.

				// Update state, so we don't get here again automatically.
				character.State |= CreatureStates.EverEnteredWorld;

				var introNpcId = (character is MabiCharacter ? Id.Nao : Id.Tin);

				if (WorldManager.Instance.GetCreatureById(introNpcId) == null)
					Logger.Warning("Intro NPC not found.");

				// With this packet many buttons and stuff are disabled,
				// until you're really logged in.
				Send.SpecialLogin(client, character, 1000, 3200, 3200, introNpcId);
			}

			client.State = ClientState.LoggedIn;
		}

		private void HandleDisconnect(WorldClient client, MabiPacket packet)
		{
			var unk1 = packet.GetByte(); // 1 | 2 (maybe login vs exit?)

			Logger.Info("'{0}' is closing the connection. Saving...", client.Account.Name);

			WorldDb.Instance.SaveAccount(client.Account);

			foreach (var pc in client.Creatures.Where(cr => cr is MabiPC))
				WorldManager.Instance.RemoveCreature(pc);

			client.Creatures.Clear();
			client.Character = null;
			client.Account = null;

			Send.DisconnectResponse(client);
		}

		private void HandleCharacterInfoRequest(WorldClient client, MabiPacket packet)
		{
			var character = client.GetCharacterOrNull(packet.Id);
			if (character == null)
			{
				Send.CharacterInfo(client, null);
				return;
			}

			// Spawn effect
			if (character.Owner != null)
				WorldManager.Instance.Broadcast(PacketCreator.SpawnEffect(character.Owner, SpawnEffect.Pet, character.GetPosition()), SendTargets.Range, character);

			// Infamous 5209, aka char info
			Send.CharacterInfo(client, character);

			if (character.Owner != null)
			{
				if (character.RaceInfo.VehicleType > 0)
					WorldManager.Instance.VehicleUnbind(null, character, true);

				if (character.IsDead)
					Send.DeadFeather(character, DeadMenuOptions.Here | DeadMenuOptions.FeatherUp);
			}

			// If initial login.
			if (character == client.Character)
			{
				if (character.Guild != null)
					Send.GuildstoneLocation(client, character);

				Send.UnreadMailCount(client, character, (uint)MabiMail.GetUnreadCount(character));

				// Button will be disabled if we don't send this packet.
				// Update: Doesn't seem to be the case anymore (noticed in NA166).
				if (WorldConf.EnableItemShop)
					Send.ItemShopInfo(client);

				if (WorldConf.AutoSendGMCP && client.Account.Authority >= WorldConf.MinimumGMCP)
					Send.GMCPOpen(client);
			}
		}

		private void HandleChat(WorldClient client, MabiPacket packet)
		{
			var type = packet.GetByte();
			var message = packet.GetString();

			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			if (message[0] == WorldConf.CommandPrefix)
			{
				var args = message.Substring(1).Split(' ');
				args[0] = args[0].ToLower();

				if (CommandHandler.Instance.Handle(client, creature, args, message))
					return;
			}

			Send.Chat(creature, message);
			EventManager.PlayerEvents.OnPlayerTalks(creature, message);
		}

		private void HandleWhisperChat(WorldClient client, MabiPacket packet)
		{
			var targetName = packet.GetString();
			var msg = packet.GetString();

			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			var target = WorldManager.Instance.GetCharacterByName(targetName);
			if (target == null)
			{
				Send.SystemMessage(client, creature, Localization.Get("world.whisper_no_target")); // The target character does not exist.
				return;
			}

			Send.Whisper(client, creature, creature.Name, msg);
			Send.Whisper(target.Client, target, creature.Name, msg);
		}

		private void HandleGesture(WorldClient client, MabiPacket packet)
		{
			var motion = packet.GetString();

			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			creature.StopMove();

			var result = false;

			var info = MabiData.MotionDb.Find(motion);
			if (info != null)
			{
				Send.UseMotion(creature, info.Category, info.Type, info.Loop);

				result = true;
			}

			Send.GestureResponse(client, creature, result);
		}

		public void HandleItemUse(WorldClient client, MabiPacket packet)
		{
			var itemOId = packet.GetLong();

			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			// Check item and alive
			var item = creature.Inventory.GetItem(itemOId);
			if (item == null || creature.IsDead)
			{
				Send.UseItemResponse(client, creature, false, 0);
				return;
			}

			// Check for use script
			var script = ScriptManager.Instance.GetItemScript(item.Info.Class);
			if (script == null)
			{
				Logger.Unimplemented("Missing script for item '{0}' ({1}).", item.DataInfo.Name, item.Info.Class);
				Send.UseItemResponse(client, creature, false, 0);
				return;
			}

			// Use item
			script.OnUse(creature, item);
			creature.Inventory.Decrement(item);

			// Mandatory stat update
			WorldManager.Instance.CreatureStatsUpdate(creature);

			Send.UseItemResponse(client, creature, true, item.Info.Class);
		}

		public void HandleGMCPMove(WorldClient client, MabiPacket packet)
		{
			var region = packet.GetInt();
			var x = packet.GetInt();
			var y = packet.GetInt();

			if (client.Account.Authority < WorldConf.MinimumGMCP || client.Account.Authority < WorldConf.MinimumGMCPMove)
			{
				Send.SystemMessage(client, client.Character, Localization.Get("gm.gmcp_auth")); // You're not authorized to use the GMCP.
				return;
			}

			client.Warp(region, x, y);
		}

		private void HandleGMCPMoveToChar(WorldClient client, MabiPacket packet)
		{
			var targetName = packet.GetString();

			if (client.Account.Authority < WorldConf.MinimumGMCP || client.Account.Authority < WorldConf.MinimumGMCPCharWarp)
			{
				Send.SystemMessage(client, client.Character, Localization.Get("gm.gmcp_auth")); // You're not authorized to use the GMCP.
				return;
			}

			var target = WorldManager.Instance.GetCharacterOrNpcByName(targetName);
			if (target == null)
			{
				Send.MsgBox(client, client.Character, Localization.Get("gm.gmcp_nochar"), targetName); // Character '{0}' couldn't be found.
				return;
			}

			var targetPos = target.GetPosition();
			client.Warp(target.Region, targetPos.X, targetPos.Y);
		}

		private void HandleGMCPRevive(WorldClient client, MabiPacket packet)
		{
			if (client.Account.Authority < WorldConf.MinimumGMCP || client.Account.Authority < WorldConf.MinimumGMCPRevive)
			{
				Send.SystemMessage(client, client.Character, Localization.Get("gm.gmcp_auth")); // You're not authorized to use the GMCP.
				return;
			}

			var creature = WorldManager.Instance.GetCreatureById(packet.Id);
			if (creature == null || !creature.IsDead)
				return;

			creature.FullHeal();
			creature.Revive();

			Send.Revived(creature);
		}

		private void HandleGMCPSummon(WorldClient client, MabiPacket packet)
		{
			var targetName = packet.GetString();

			if (client.Account.Authority < WorldConf.MinimumGMCP || client.Account.Authority < WorldConf.MinimumGMCPSummon)
			{
				Send.SystemMessage(client, client.Character, Localization.Get("gm.gmcp_auth")); // You're not authorized to use the GMCP.
				return;
			}

			var target = WorldManager.Instance.GetCharacterByName(targetName);
			if (target == null)
			{
				Send.MsgBox(client, client.Character, Localization.Get("gm.gmcp_nochar"), targetName); // Character '{0}' couldn't be found.
				return;
			}

			var targetClient = (target.Client as WorldClient);
			var pos = client.Character.GetPosition();

			Send.ServerMessage(target.Client, target, Localization.Get("gm.gmcp_summon"), client.Character.Name); // You've been summoned by '{0}'.
			targetClient.Warp(client.Character.Region, pos.X, pos.Y);
		}

		private void HandleGMCPListNPCs(WorldClient client, MabiPacket packet)
		{
			if (client.Account.Authority < WorldConf.MinimumGMCP)
			{
				Send.SystemMessage(client, client.Character, Localization.Get("gm.gmcp_auth")); // You're not authorized to use the GMCP.
				return;
			}

			Send.SystemMessage(client, client.Character, Localization.Get("aura.unimplemented")); // Unimplemented.
		}

		private void HandleGMCPInvisibility(WorldClient client, MabiPacket packet)
		{
			var activate = packet.GetBool();

			if (client.Account.Authority < WorldConf.MinimumGMCP || client.Account.Authority < WorldConf.MinimumGMCPInvisible)
			{
				Send.SystemMessage(client, client.Character, Localization.Get("gm.gmcp_auth")); // You're not authorized to use the GMCP.
				return;
			}

			if (client.Character.Id != packet.Id)
				return;

			if (activate)
				client.Character.Activate(CreatureConditionA.Invisible);
			else
				client.Character.Deactivate(CreatureConditionA.Invisible);

			Send.StatusEffectUpdate(client.Character);
			Send.GMCPInvisibilityResponse(client, true);
		}

		private void HandleGMCPExpel(WorldClient client, MabiPacket packet)
		{
			var targetName = packet.GetString();

			if (client.Account.Authority < WorldConf.MinimumGMCP || client.Account.Authority < WorldConf.MinimumGMCPExpel)
			{
				Send.SystemMessage(client, client.Character, Localization.Get("gm.gmcp_auth")); // You're not authorized to use the GMCP.
				return;
			}

			var target = WorldManager.Instance.GetCharacterByName(targetName);
			if (target == null)
			{
				Send.MsgBox(client, client.Character, Localization.Get("gm.gmcp_nochar"), targetName); // Character '{0}' couldn't be found.
				return;
			}

			// Better kill the connection, modders could bypass a dc request.
			target.Client.Kill();

			Send.MsgBox(client, client.Character, Localization.Get("gm.gmcp_kicked"), targetName); // '{0}' has been kicked.
		}

		private void HandleGMCPBan(WorldClient client, MabiPacket packet)
		{
			var targetName = packet.GetString();
			var duration = packet.GetInt();
			var reason = packet.GetString();

			if (client.Account.Authority < WorldConf.MinimumGMCP || client.Account.Authority < WorldConf.MinimumGMCPBan)
			{
				Send.SystemMessage(client, client.Character, Localization.Get("gm.gmcp_auth")); // You're not authorized to use the GMCP.
				return;
			}

			var target = WorldManager.Instance.GetCharacterByName(targetName);
			if (target == null)
			{
				Send.MsgBox(client, client.Character, Localization.Get("gm.gmcp_nochar"), targetName); // Character '{0}' couldn't be found.
				return;
			}

			var end = DateTime.Now.AddMinutes(duration);
			var targetClient = target.Client as WorldClient;
			targetClient.Account.BannedExpiration = end;
			targetClient.Account.BannedReason = reason;

			// Better kill the connection, modders could bypass a dc request.
			targetClient.Kill();

			Send.MsgBox(client, client.Character, Localization.Get("gm.gmcp_banned"), targetName, end); // '{0}' has been banned till '{1}'.
		}

		private void HandleNPCTalkStart(WorldClient client, MabiPacket packet)
		{
			var npcId = packet.GetLong();

			if (client.Character.Id != packet.Id)
				return;

			var target = WorldManager.Instance.GetCreatureById(npcId) as MabiNPC;
			if (target == null)
			{
				Logger.Warning("Unknown NPC: " + npcId.ToString("X"));
			}
			else if (target.Script == null)
			{
				Logger.Warning("Script for '" + target.Name + "' is null.");
				target = null;
			}
			else if (client.Character.Region != target.Region || !WorldManager.InRange(client.Character, target, 1000))
			{
				Send.MsgBox(client, client.Character, Localization.Get("world.too_far")); // You're too far away.
				target = null;
			}

			if (target == null)
			{
				Send.NPCTalkStartResponse(client, false, 0);
				return;
			}

			Send.NPCTalkStartResponse(client, true, npcId);

			client.NPCSession.Start(target);

			// Get enumerator and start first run.
			client.NPCSession.State = target.Script.OnTalk(client).GetEnumerator();
			if (client.NPCSession.State.MoveNext())
				client.NPCSession.Response = client.NPCSession.State.Current as Response;
		}

		private void HandleNPCTalkPartner(WorldClient client, MabiPacket packet)
		{
			var partnerId = packet.GetLong();

			if (client.Character.Id != packet.Id)
				return;

			var target = client.Creatures.FirstOrDefault(a => a.Id == partnerId);
			if (target == null)
			{
				Logger.Warning("Talk to unspawned partner: " + partnerId.ToString());
			}

			var npc = WorldManager.Instance.GetCreatureByName("_partnerdummy") as MabiNPC;
			if (npc == null || npc.Script == null)
			{
				Logger.Warning("NPC or script of '_partnerdummy' is null.");
				npc = null;
			}

			if (npc == null)
				Send.NPCTalkPartnerStartResponse(client, false, 0, string.Empty);
			else
				Send.NPCTalkPartnerStartResponse(client, true, partnerId, target.Name);

			client.NPCSession.Start(npc);

			client.NPCSession.State = npc.Script.OnTalk(client).GetEnumerator();
			if (client.NPCSession.State.MoveNext())
				client.NPCSession.Response = client.NPCSession.State.Current as Response;
		}

		private void HandleNPCTalkEnd(WorldClient client, MabiPacket packet)
		{
			var npcId = packet.GetLong();

			if (client.Character.Id != packet.Id)
				return;

			var target = client.NPCSession.Target;

			//var p = new MabiPacket(Op.NPCTalkEndR, creature.Id);
			//p.PutByte(1);
			//p.PutLong(target.Id);
			//p.PutString("");
			//client.Send(p);

			if (target == null || target.Script == null)
			{
				Logger.Warning("Ending empty NPC session.");
				return;
			}

			target.Script.OnEnd(client);

			client.NPCSession.Clear();
		}

		/// <summary>
		/// Parameters:
		///		string  Keyword
		///	Description:
		///		Sent when selecting a keyword. Exact purpose unknown,
		///		NPCTalkSelect is sent as well. Maybe a check.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="packet"></param>
		private void HandleNPCTalkKeyword(WorldClient client, MabiPacket packet)
		{
			var keyword = packet.GetString();

			if (client.Character.Id != packet.Id)
				return;

			if (client.NPCSession.IsValid)
			{
				Logger.Debug("No target or no session.");
				return;
			}

			Send.NPCTalkKeywordResponse(client, true, keyword);
		}

		private void HandleNPCTalkSelect(WorldClient client, MabiPacket packet)
		{
			var response = packet.GetString();
			var sessionId = packet.GetInt();

			if (client.Character.Id != packet.Id)
				return;

			if (!client.NPCSession.IsValid)
			{
				Logger.Warning("Invalid NPC session for '{0}', talking to '{1}'.", client.Character.Name, (client.NPCSession.Target != null ? client.NPCSession.Target.Script.ScriptName : "<unknown>"));
				return;
			}

			if (sessionId != client.NPCSession.Id)
			{
				Logger.Debug("No target or session id incorrect ({0}:{1})", sessionId, client.NPCSession.Id);
				return;
			}

			int pos = -1;
			if ((pos = response.IndexOf("<return type=\"string\">")) < 1)
			{
				Logger.Debug("No return value found.");
				return;
			}
			response = response.Substring(pos += 22, response.IndexOf('<', pos) - pos);

			if (response == "@end")
			{
				Send.NPCTalkSelectEnd(client);

				client.NPCSession.Target.Script.OnEnd(client);
			}
			else if (response.StartsWith("@input"))
			{
				var splitted = response.Split(':');
				if (client.NPCSession.Response != null)
					client.NPCSession.Response.Value = splitted[1];
				if (client.NPCSession.State.MoveNext())
					client.NPCSession.Response = client.NPCSession.State.Current as Response;
			}
			else
			{
				if (client.NPCSession.Response != null)
					client.NPCSession.Response.Value = response;
				if (client.NPCSession.State.MoveNext())
					client.NPCSession.Response = client.NPCSession.State.Current as Response;
			}
		}

		private void HandleGetMails(WorldClient client, MabiPacket packet)
		{
			if (client.Character.Id != packet.Id)
				return;

			var allMails = new List<MabiMail>();
			allMails.AddRange(WorldDb.Instance.GetRecievedMail(client.Character.Id));
			allMails.AddRange(WorldDb.Instance.GetSentMail(client.Character.Id));

			var toReturn = new List<MabiMail>();
			var validMails = new List<MabiMail>();
			foreach (var mail in allMails)
			{
				if (WorldConf.MailExpires > 0 && (DateTime.Today - mail.Sent).Days > WorldConf.MailExpires)
				{
					toReturn.Add(mail);
					continue;
				}

				validMails.Add(mail);
			}

			Send.GetMailsResponse(client, validMails);

			foreach (var mail in toReturn)
				mail.Return("Mail is valid for {0} days, and the mail's valid period has expired. The mail has been returned to its sender.", WorldConf.MailExpires);
		}

		private void HandleConfirmMailRecipient(WorldClient client, MabiPacket packet)
		{
			var recipient = packet.GetString();

			if (client.Character.Id != packet.Id)
				return;

			ulong recipientId;
			Send.ConfirmMailRecipentResponse(client, WorldDb.Instance.IsValidMailRecpient(recipient, out recipientId), recipientId);
		}

		private void HandleSendMail(WorldClient client, MabiPacket packet)
		{
			var recipientName = packet.GetString();
			var text = packet.GetString();
			var itemId = packet.GetLong();
			var cod = packet.GetInt();

			if (client.Character.Id != packet.Id)
				return;

			ulong recipientId;
			if (!WorldDb.Instance.IsValidMailRecpient(recipientName, out recipientId))
			{
				Send.MsgBox(client, client.Character, Localization.Get("world.mail_invalid")); // Invaild recipient
				Send.SendMailFail(client);
				return;
			}

			var mail = new MabiMail();
			mail.Type = (byte)MailTypes.Normal;
			mail.RecipientName = recipientName;
			mail.SenderName = client.Character.Name;
			mail.SenderId = client.Character.Id;
			mail.Text = text;
			mail.ItemId = itemId;
			mail.COD = cod;

			if (mail.ItemId != 0)
			{
				mail.Type = (byte)MailTypes.Item;

				var item = client.Character.Inventory.GetItem(itemId);
				if (item == null)
				{
					Send.MsgBox(client, client.Character, Localization.Get("world.mail_item")); // You can't send an item you don't have!
					Send.SendMailFail(client);
					return;
				}

				client.Character.Inventory.Remove(item);
				WorldDb.Instance.SaveMailItem(item, null);
			}

			mail.Save(true);

			Send.SendMailResponse(client, mail);
		}

		private void HandleMarkMailRead(WorldClient client, MabiPacket packet)
		{
			var mailId = packet.GetLong();

			if (client.Character.Id != packet.Id)
				return;

			var mail = WorldDb.Instance.GetMail(mailId);
			if (mail == null)
			{
				Send.MarkMailReadResponse(client, false, 0);
				return;
			}

			mail.Read = 2;
			mail.Save(false);

			Send.MarkMailReadResponse(client, true, mail.MessageId);
		}

		private void HandleReturnMail(WorldClient client, MabiPacket packet)
		{
			var mailId = packet.GetLong();
			var message = packet.GetString();

			if (client.Character.Id != packet.Id)
				return;

			var mail = WorldDb.Instance.GetMail(mailId);
			if (mail == null)
			{
				Send.ReturnMailResponse(client, false, 0);
				return;
			}

			mail.Return(message);

			Send.ReturnMailResponse(client, true, mail.MessageId);
		}

		private void HandleRecallMail(WorldClient client, MabiPacket packet)
		{
			var mailId = packet.GetLong();

			if (client.Character.Id != packet.Id)
				return;

			var mail = WorldDb.Instance.GetMail(mailId);
			if (mail == null || mail.ItemId == 0)
			{
				Send.RecallMailResponse(client, false, 0);
				return;
			}

			var item = WorldDb.Instance.GetItem(mail.ItemId);
			client.Character.Inventory.Add(item, Pocket.Temporary); // TODO: Inv
			WorldDb.Instance.SaveMailItem(item, client.Character);

			mail.Delete();

			Send.ItemInfo(client, client.Character, item);
			Send.RecallMailResponse(client, true, mail.MessageId);
		}

		private void HandleRecieveMailItem(WorldClient client, MabiPacket packet)
		{
			var mailId = packet.GetLong();

			if (client.Character.Id != packet.Id)
				return;

			var mail = WorldDb.Instance.GetMail(mailId);

			if (mail == null || mail.ItemId == 0)
			{
				Send.ReceiveMailItemResponse(client, false, 0);
				return;
			}

			//TODO: COD

			var item = WorldDb.Instance.GetItem(mail.ItemId);
			client.Character.Inventory.Add(item, Pocket.Temporary); // TODO: Inv
			WorldDb.Instance.SaveMailItem(item, client.Character);

			mail.Delete();

			Send.ItemInfo(client, client.Character, item);
			Send.ReceiveMailItemResponse(client, true, mail.ItemId);
		}

		private void HandleDeleteMail(WorldClient client, MabiPacket packet)
		{
			var mailId = packet.GetLong();

			if (client.Character.Id != packet.Id)
				return;

			var m = WorldDb.Instance.GetMail(mailId);
			if (m == null)
			{
				Send.DeleteMailResponse(client, false, 0);
				return;
			}

			m.Delete();

			Send.DeleteMailResponse(client, false, m.MessageId);
		}

		private void HandleItemMove(WorldClient client, MabiPacket packet)
		{
			var itemId = packet.GetLong();
			var source = (Pocket)packet.GetByte();
			var target = (Pocket)packet.GetByte();
			var unk = packet.GetByte();
			var targetX = packet.GetByte();
			var targetY = packet.GetByte();

			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			var item = creature.Inventory.GetItem(itemId);
			if (item == null || item.Type == ItemType.Hair || item.Type == ItemType.Face)
			{
				Send.ItemMoveR(creature, false);
				return;
			}

			// Stop moving when changing weapons
			if ((target >= Pocket.RightHand1 && target <= Pocket.Magazine2) || (source >= Pocket.RightHand1 && source <= Pocket.Magazine2))
				creature.StopMove();

			// Try to move item
			if (!creature.Inventory.Move(item, target, targetX, targetY))
			{
				Send.ItemMoveR(creature, false);
				return;
			}

			Send.ItemMoveR(creature, true);
		}

		private void HandleItemDrop(WorldClient client, MabiPacket packet)
		{
			var itemId = packet.GetLong();
			var unk = packet.GetByte();

			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			var item = creature.Inventory.GetItem(itemId);
			if (item == null || item.Type == ItemType.Hair || item.Type == ItemType.Face)
			{
				Send.ItemDropR(creature, false);
				return;
			}

			if (!creature.Inventory.Remove(item))
			{
				Send.ItemDropR(creature, false);
				return;
			}

			if (HandleDungeonDrop(client, creature, item))
				return;

			// Drop it
			var rnd = RandomProvider.Get();
			var pos = creature.GetPosition();

			item.Id = MabiItem.NewItemId;
			WorldManager.Instance.DropItem(item, creature.Region, (uint)(pos.X + rnd.Next(-50, 51)), (uint)(pos.Y + rnd.Next(-50, 51)));
			EventManager.CreatureEvents.OnCreatureItemAction(creature, item.Info.Class);

			Send.ItemDropR(creature, true);
		}

		/// <summary>
		/// Temp function to separate this stuff from HandleItemDrop.
		/// </summary>
		uint DGID1 = 10001;
		uint DGID2 = 10002;
		ulong INSTANCEID = Id.Instances;
		string DGDESIGN = "TirCho_Alby_Low_Dungeon";
		private bool HandleDungeonDrop(WorldClient client, MabiCreature creature, MabiItem item)
		{
			// TODO: Go through the list of dungeons (scriptable?), check the
			//   altars there, return true if dungeon drop, etc etc.

			if (creature.OnAltar != DungeonAltar.Alby)
				return false;

			//DGID1 += 2;
			//DGID2 += 2;
			//ITID2++;
			//INSTANCEID++;

			Send.CharacterLock(client);

			// Done
			var p = new MabiPacket(Op.ItemDropR, creature.Id);
			p.PutByte(1);
			client.Send(p);

			WorldManager.Instance.CreatureLeaveRegion(creature);
			creature.SetLocation(DGID1, 3262, 3139);

			// Dungeon info
			// --------------------------------------------------------------
			var dunp = new MabiPacket(Op.DungeonInfo, Id.Broadcast);

			dunp.PutLong(creature.Id);
			dunp.PutLong(INSTANCEID);            // Instance id?

			dunp.PutByte(1);
			dunp.PutString(DGDESIGN);            // Dungeon name (dungeondb.xml)

			dunp.PutInt(1234567890);
			dunp.PutInt(0987654321);
			dunp.PutInt(0);

			dunp.PutInt(2);                      // ? Count, Entry + Floors?
			dunp.PutInt(DGID1);                  // Imaginary entrance region id?
			dunp.PutInt(DGID2);                  // Imaginary dungeon region id?

			dunp.PutString("<option/>");

			dunp.PutInt(1);			             // Floor Count
			{
				dunp.PutInt(4);                  // Room Count
				{
					dunp.PutByte(0);
					dunp.PutByte(0);

					dunp.PutByte(0);
					dunp.PutByte(0);

					dunp.PutByte(0);
					dunp.PutByte(0);

					dunp.PutByte(0);
					dunp.PutByte(0);
				}
			}

			dunp.PutInt(0);
			dunp.PutInt(1);					     // Floor Count?
			{
				dunp.PutInt(543210987);
				dunp.PutInt(0);
			}

			client.Send(dunp);

			return true;
		}

		public void HandleItemDestroy(WorldClient client, MabiPacket packet)
		{
			var itemId = packet.GetLong();

			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			var item = creature.Inventory.GetItem(itemId);
			if (item == null || item.Type == ItemType.Hair || item.Type == ItemType.Face)
			{
				Send.ItemDestroyR(creature, false);
				return;
			}

			if (!creature.Inventory.Remove(item))
			{
				Send.ItemDestroyR(creature, false);
				return;
			}

			Send.ItemDestroyR(creature, true);

			EventManager.CreatureEvents.OnCreatureItemAction(creature, item.Info.Class);
		}

		private void HandleItemPickUp(WorldClient client, MabiPacket packet)
		{
			var itemId = packet.GetLong();

			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			var item = WorldManager.Instance.GetItemById(itemId);
			if (item == null)
			{
				Send.ItemPickUpR(creature, false);
				return;
			}

			var success = creature.Inventory.Insert(item, false, false);
			if (success)
			{
				// Order is important here, remove from world first =/
				WorldManager.Instance.RemoveItem(item);
				if (item.Info.Amount > 0)
					Send.ItemInfo(client, creature, item);
			}
			else
				Send.SystemMessage(client, creature, Localization.Get("world.insufficient_space")); // Not enough space.

			Send.ItemPickUpR(creature, success);
		}

		private void HandleItemSplit(WorldClient client, MabiPacket packet)
		{
			var itemId = packet.GetLong();
			var amount = packet.GetShort();
			var unk1 = packet.GetByte();

			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			// Check item
			var item = creature.Inventory.GetItem(itemId);
			if (item == null || item.StackType == BundleType.None)
			{
				Send.ItemSplitR(creature, false);
				return;
			}

			// Check requested amount
			if (item.Info.Amount < amount)
				amount = item.Info.Amount;

			// Clone item or create new one based on stack item
			MabiItem splitItem;
			if (item.StackItem == 0)
				splitItem = new MabiItem(item);
			else
				splitItem = new MabiItem(item.StackItem);
			splitItem.Info.Amount = amount;

			// New item on cursor
			if (!creature.Inventory.Add(splitItem, Pocket.Cursor))
			{
				Send.ItemSplitR(creature, false);
				return;
			}

			// Update amount or remove
			item.Info.Amount -= amount;

			if (item.Info.Amount > 0 || item.StackItem != 0)
			{
				Send.ItemAmount(creature, item);
			}
			else
			{
				creature.Inventory.Remove(item);
			}

			Send.ItemSplitR(creature, true);
		}

		private void HandleSwitchSet(WorldClient client, MabiPacket packet)
		{
			var set = (WeaponSet)packet.GetByte();

			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			creature.StopMove();

			creature.Inventory.WeaponSet = set;

			Send.UpdateWeaponSet(creature);
			WorldManager.Instance.CreatureStatsUpdate(creature);

			Send.SwitchSetR(creature, true);
		}

		private void HandleItemStateChange(WorldClient client, MabiPacket packet)
		{
			var firstTarget = (Pocket)packet.GetByte();
			var secondTarget = (Pocket)packet.GetByte();
			var unk = packet.GetByte();

			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			// This might not be entirely correct, but works fine.
			// Robe is opened first, Helm secondly, then Robe and Helm are both closed.

			foreach (var target in new Pocket[] { firstTarget, secondTarget })
			{
				if (target > 0)
				{
					var item = creature.Inventory.GetItemAt(target, 0, 0);
					if (item != null)
					{
						item.Info.FigureA = (byte)(item.Info.FigureA == 1 ? 0 : 1);
						Send.EquipmentChanged(creature, item);
					}
				}
			}

			Send.ItemStateChangeR(creature);
		}

		private void HandleEnterRegion(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCharacterOrNull(packet.Id);
			if (creature == null)
				return;

			// TODO: Maybe check if this action is valid.

			Send.CharacterUnlock(client, creature);

			// Sent on log in, but not when switching regions?
			client.Send(new MabiPacket(Op.EnterRegionR, Id.World).PutByte(1).PutLongs(creature.Id).PutLong(MabiTime.Now.DateTime));
			WorldManager.Instance.AddCreature(creature);

			if (creature == client.Character)
			{
				var entities = WorldManager.Instance.GetEntitiesInRange(creature);

				if (entities.Count > 0)
					Send.EntitiesAppear(client, entities);
			}

			var pos = creature.GetPosition();

			client.Send(new MabiPacket(Op.WarpRegion, creature.Id).PutByte(1).PutInts(creature.Region, pos.X, pos.Y));

			// Send Conformation?
			client.Send(new MabiPacket(0xA925, Id.Broadcast).PutInts(creature.Region, 0));

			if (creature == client.Character)
			{
				foreach (var rider in creature.Riders.Where(c => c.Client != client))
					((WorldClient)rider.Client).Warp(creature.Region, pos.X, pos.Y);
			}

			if (creature.Pet != null)
			{
				creature.Pet.SetLocation(creature.Region, pos.X, pos.Y);
				Send.EnterRegionPermission(client, creature.Pet);

				foreach (var rider in creature.Pet.Riders.Where(c => c.Client != client))
					((WorldClient)rider.Client).Warp(creature.Region, pos.X, pos.Y);
			}

			WorldManager.Instance.CreatureEnterRegionPVPStuff(creature);

			EventManager.PlayerEvents.OnPlayerChangesRegion(creature as MabiPC);
		}

		/// <summary>
		/// Checks Stamina and Mana, sends fail response and stats update. No success response.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="packet"></param>
		private void HandleSkillPrepare(WorldClient client, MabiPacket packet)
		{
			var skillId = (SkillConst)packet.GetShort();

			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			MabiSkill skill; SkillHandler handler;
			SkillManager.CheckOutSkill(creature, skillId, out skill, out handler);
			if (skill == null || handler == null)
			{
				Send.SendSkillPrepareFail(client, creature);
				return;
			}

			// Check Mana
			if (creature.Mana < skill.RankInfo.ManaCost)
			{
				Send.SystemMessage(client, creature, Localization.Get("skills.insufficient_mana")); // Insufficient Mana
				Send.SendSkillPrepareFail(client, creature);
				return;
			}

			// Check Stamina
			if (creature.Stamina < skill.RankInfo.StaminaCost)
			{
				Send.SystemMessage(client, creature, Localization.Get("skills.insufficient_stamina")); // Insufficient Stamina
				Send.SendSkillPrepareFail(client, creature);
				return;
			}

			if (creature.ActiveSkillId != skill.Id && creature.ActiveSkillId != 0)
			{
				SkillManager.GetHandler(creature.ActiveSkillId).Cancel(creature, creature.Skills.Get(creature.ActiveSkillId));
			}

			creature.ActiveSkillId = skill.Id;

			// Save cast time when preparation is finished.
			var castTime = WorldConf.DynamicCombat ? skill.RankInfo.NewLoadTime : skill.RankInfo.LoadTime;
			creature.ActiveSkillPrepareEnd = DateTime.Now.AddMilliseconds(castTime);

			var result = handler.Prepare(creature, skill, packet, castTime);

			if ((result & SkillResults.Failure) != 0)
			{
				Send.SendSkillPrepareFail(client, creature);
				return;
			}

			if (skill.RankInfo.ManaCost > 0)
				creature.Mana -= skill.RankInfo.ManaCost;
			if (skill.RankInfo.StaminaCost > 0)
				creature.Stamina -= skill.RankInfo.StaminaCost;

			WorldManager.Instance.CreatureStatsUpdate(creature);

			// Not Okay or NoReply
			//if ((result & SkillResults.Okay) == 0 || (result & SkillResults.NoReply) != 0)
			//    return;

			//client.SendSkillPrepare(creature, skill.Id, castTime);
		}

		/// <summary>
		/// No success response.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="packet"></param>
		private void HandleSkillReady(WorldClient client, MabiPacket packet)
		{
			var skillId = (SkillConst)packet.GetShort();
			//var parameters = packet.GetStringOrEmpty();

			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			MabiSkill skill; SkillHandler handler;
			SkillManager.CheckOutSkill(creature, skillId, out skill, out handler);
			if (skill == null || handler == null)
				return;

			var result = handler.Ready(creature, skill);

			if ((result & SkillResults.Okay) == 0)
				return;

			//client.SendSkillReady(creature, skill.Id, parameters);
		}

		/// <summary>
		/// Sends insufficient Stamina msg. No success response.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="packet"></param>
		private void HandleSkillUse(WorldClient client, MabiPacket packet)
		{
			var skillId = (SkillConst)packet.GetShort();

			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			MabiSkill skill; SkillHandler handler;
			SkillManager.CheckOutSkill(creature, skillId, out skill, out handler);
			if (skill == null || handler == null)
				return;

			var result = handler.Use(creature, skill, packet);

			if ((result & SkillResults.InsufficientStamina) != 0)
				Send.SystemMessage(client, creature, Localization.Get("skills.insufficient_stamina")); // Insufficient Stamina
		}

		/// <summary>
		/// No success response.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="packet"></param>
		private void HandleSkillComplete(WorldClient client, MabiPacket packet)
		{
			var skillId = (SkillConst)packet.GetShort();

			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			MabiSkill skill; SkillHandler handler;
			SkillManager.CheckOutSkill(creature, skillId, out skill, out handler);
			if (skill == null || handler == null)
				return;

			var result = handler.Complete(creature, skill, packet);

			if (creature.ActiveSkillStacks < 1)
				creature.ActiveSkillId = SkillConst.None;

			//if ((result & SkillResults.Okay) == 0 || (result & SkillResults.NoReply) != 0)
			//{
			//    creature.ActiveSkillId = 0;
			//    return;
			//}

			//client.SendSkillComplete(creature, skill.Id);

			//if (creature.ActiveSkillStacks > 0 && skill.RankInfo.Stack > 1)
			//{
			//    // Send new ready packet if there are stacks left.
			//    client.SendSkillReady(creature, skill.Id);
			//}
			//else
			//{
			//    creature.ActiveSkillId = 0;
			//    creature.ActiveSkillTarget = null;
			//}
		}

		/// <summary>
		/// Properly cancels skill, calls skill's cancel handler,
		/// but doesn't require manual success.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="packet"></param>
		private void HandleSkillCancel(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			creature.CancelSkill();
		}

		/// <summary>
		/// Full handling.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="packet"></param>
		private void HandleSkillStart(WorldClient client, MabiPacket packet)
		{
			var skillId = (SkillConst)packet.GetShort();

			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			MabiSkill skill; SkillHandler handler;
			SkillManager.CheckOutSkill(creature, skillId, out skill, out handler);
			if (skill == null || handler == null)
			{
				client.Send(new MabiPacket(Op.SkillStart, creature.Id).PutShort((ushort)skillId));
				return;
			}

			handler.Start(creature, skill, packet);
		}

		/// <summary>
		/// Full handling.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="packet"></param>
		private void HandleSkillStop(WorldClient client, MabiPacket packet)
		{
			var skillId = (SkillConst)packet.GetShort();

			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			MabiSkill skill; SkillHandler handler;
			SkillManager.CheckOutSkill(creature, skillId, out skill, out handler);
			if (skill == null || handler == null)
				return;

			handler.Stop(creature, skill, packet);
		}

		private void HandleChangeStance(WorldClient client, MabiPacket packet)
		{
			var mode = packet.GetByte();

			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			// Clear target?
			if (mode == 0)
				Send.CombatTargetSet(creature, null);

			// Send info
			creature.BattleState = mode;
			Send.ChangesStance(creature);

			if (creature.Vehicle != null && creature == creature.Vehicle.Owner)
			{
				creature.Vehicle.BattleState = mode;
				Send.ChangesStance(creature.Vehicle);
			}
			if (creature.Owner != null && creature.Riders.Contains(creature.Owner))
			{
				creature.Owner.BattleState = mode;
				Send.ChangesStance(creature.Owner);
			}

			// Unlock
			client.Send(new MabiPacket(Op.ChangeStanceR, creature.Id));
		}

		private void HandleShopBuyItem(WorldClient client, MabiPacket packet)
		{
			var itemId = packet.GetLong();
			var targetPocket = packet.GetByte(); // 0:cursor, 1:inv
			var unk2 = packet.GetByte(); // storage gold?

			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			if (!client.NPCSession.IsValid)
				return;

			var newItem = client.NPCSession.Target.Script.Shop.GetItem(itemId);
			if (newItem == null)
				return;

			// The client expects the price for a full stack to be sent,
			// so we have to calculate the actual price here.
			var price = newItem.OptionInfo.Price;
			if (newItem.StackType == BundleType.Stackable)
				price = (uint)(price / newItem.StackMax * newItem.Count);

			// Check gold
			if (creature.Inventory.Gold < price)
			{
				Send.MsgBox(client, creature, Localization.Get("world.shop_gold")); // Insufficient amount of gold.
				Send.ShopBuyItemR(creature, false);
				return;
			}

			var success = false;

			// Cursor
			if (targetPocket == 0)
				success = creature.Inventory.Add(newItem, Pocket.Cursor);
			// Inventory
			else if (targetPocket == 1)
				success = creature.Inventory.Add(newItem, false);

			if (success)
				creature.Inventory.RemoveGold(price);

			Send.ShopBuyItemR(creature, success);
		}

		private void HandleShopSellItem(WorldClient client, MabiPacket packet)
		{
			var itemId = packet.GetLong();
			var unk1 = packet.GetByte();

			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			if (!client.NPCSession.IsValid)
				return;

			var item = creature.Inventory.GetItem(itemId);
			if (item == null)
				return;

			// Remove from inv
			if (!creature.Inventory.Remove(item))
				return;

			// Calculate selling price
			var sellingPrice = item.OptionInfo.SellingPrice;

			if (item.StackType == BundleType.Sac)
			{
				// Add costs of the items inside the sac
				var stackItem = MabiData.ItemDb.Find(item.StackItem);
				if (stackItem != null)
				{
					sellingPrice += (uint)((item.Info.Amount / (float)stackItem.StackMax) * stackItem.SellingPrice);
				}
			}
			else if (item.StackType == BundleType.Stackable)
			{
				// Individuel price for this stack
				sellingPrice = (uint)((item.Count / (float)item.StackMax) * sellingPrice);
			}

			creature.Inventory.AddGold(sellingPrice);

			Send.ShopSellItemR(creature);
		}

		private void HandleChangeTitle(WorldClient client, MabiPacket packet)
		{
			var title = packet.GetShort();
			var optionTitle = packet.GetShort();

			var character = client.GetCharacterOrNull(packet.Id);
			if (character == null)
				return;

			var titleSuccess = false;
			var optionSuccess = false;

			// Make sure the character has this title
			if (title == 0 || (character.Titles.ContainsKey(title)) && character.Titles[title])
			{
				character.Title = title;
				titleSuccess = true;
			}

			if (optionTitle == 0 || (character.Titles.ContainsKey(optionTitle)) && character.Titles[optionTitle])
			{
				character.OptionTitle = optionTitle;
				optionSuccess = true;
			}

			if (titleSuccess || optionSuccess)
				Send.TitleUpdate(character);

			Send.ChangeTitleResponse(client, character, titleSuccess, optionSuccess);
		}

		private void HandlePetSummon(WorldClient client, MabiPacket packet)
		{
			var petId = packet.GetLong();
			var unk1 = packet.GetByte();

			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			// No pets in soul stream.
			// TODO: implement something like map flags?
			if (creature.Region == 1000)
			{
				client.Send(new MabiPacket(Op.PetSummonR, creature.Id).PutByte(false));
				return;
			}

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

			pet.IsFlying = false;

			// Set pet position near the summoner
			var pos = creature.GetPosition();
			var rand = RandomProvider.Get();
			pos.X = (uint)(pos.X + rand.Next(-350, 351));
			pos.Y = (uint)(pos.Y + rand.Next(-350, 351));
			pet.SetLocation(creature.Region, pos.X, pos.Y);
			pet.Direction = (byte)rand.Next(255);

			pet.Owner = creature;
			creature.Pet = pet;
			client.Creatures.Add(pet);

			pet.Save = true;

			// Doesn't fix giant mount problems.
			if (creature.IsGiant)
				pet.StateEx |= CreatureStatesEx.SummonedByGiant;

			p = new MabiPacket(Op.PetRegister, creature.Id);
			p.PutLong(pet.Id);
			p.PutByte(2);
			client.Send(p);

			p = new MabiPacket(Op.PetSummonR, creature.Id);
			p.PutByte(1);
			p.PutLong(petId);
			client.Send(p);

			Send.CharacterLock(client, pet);

			Send.EnterRegionPermission(client, pet);
		}

		private void HandlePetUnsummon(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			var petId = packet.GetLong();

			var pet = client.Creatures.FirstOrDefault(a => a.Id == petId);
			if (pet == null)
			{
				var p = new MabiPacket(Op.PetUnsummonR, creature.Id);
				p.PutByte(0);
				p.PutLong(petId);
				client.Send(p);
				return;
			}

			client.Creatures.Remove(pet);

			pet.StopMove();
			var pos = pet.GetPosition();

			WorldManager.Instance.Broadcast(PacketCreator.SpawnEffect(creature, SpawnEffect.PetDespawn, pos), SendTargets.Range, pet);
			WorldManager.Instance.RemoveCreature(pet);

			if (pet.Riders.Count > 0)
			{
				if (pet.IsFlying)
				{
					Send.CharacterUnlock(client, pet, 0xFFFFBDFF);
					foreach (var rider in pet.Riders)
						Send.CharacterUnlock(client, rider, 0xFFFFBDFF);
					pet.IsFlying = false;
				}
				foreach (var rider in pet.Riders)
				{
					rider.Vehicle = null;
					WorldManager.Instance.VehicleUnbind(rider, pet);
					rider.StopMove();
				}
				pet.Riders.Clear();
			}

			if (pet.Owner != null)
			{
				pet.Owner.Pet = null;
				pet.Owner = null;
			}

			// ?
			WorldManager.Instance.Broadcast(new MabiPacket(Op.PetUnRegister, creature.Id).PutLong(pet.Id), SendTargets.Range, creature);

			// Disappear
			client.Send(new MabiPacket(Op.Disappear, Id.World).PutLong(pet.Id));

			// Result
			client.Send(new MabiPacket(Op.PetUnsummonR, creature.Id).PutByte(true).PutLong(petId));
		}

		private void HandlePetMount(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			var petId = packet.GetLong();

			var creatureIsSitting = creature.Has(CreatureStates.SitDown);

			var pet = client.Account.Pets.FirstOrDefault(a => a.Id == petId);
			if (pet == null || pet.IsDead || pet.RaceInfo.VehicleType == 0 || pet.RaceInfo.VehicleType == 17 || creatureIsSitting || !WorldManager.InRange(creature, pet, 200))
			{
				if (creatureIsSitting)
					Send.Notice(client, NoticeType.MiddleTop, Localization.Get("world.mount_sit")); // You cannot mount while resting.

				client.Send(new MabiPacket(Op.PetMountR, creature.Id).PutByte(false));
				return;
			}

			creature.Vehicle = pet;
			pet.Riders.Add(creature);

			WorldManager.Instance.VehicleBind(creature, pet);

			// Mount motion (horse)
			// TODO: Add to db.
			var p = new MabiPacket(Op.UseMotion, creature.Id);
			p.PutInt(21);
			p.PutInt(0);
			p.PutByte(0);
			p.PutShort(0);
			client.Send(p);

			client.Send(new MabiPacket(Op.PetMountR, creature.Id).PutByte(true));
		}

		private void HandlePetUnmount(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			if (creature.Vehicle == null || !creature.Vehicle.Riders.Contains(creature) || creature.Vehicle.IsFlying)
			{
				client.Send(new MabiPacket(Op.PetUnmountR, creature.Id).PutByte(false));
				return;
			}

			WorldManager.Instance.VehicleUnbind(creature, creature.Vehicle);

			// Unmount motion (horse)
			var p2 = new MabiPacket(Op.UseMotion, creature.Id);
			p2.PutInt(21);
			p2.PutInt(1);
			p2.PutByte(0);
			p2.PutShort(0);
			client.Send(p2);

			var pos = creature.Vehicle.GetPosition();

			creature.SetPosition(pos.X, pos.Y);

			creature.Vehicle.Riders.Remove(creature);
			creature.Vehicle = null;

			client.Send(new MabiPacket(Op.PetUnmountR, creature.Id).PutByte(true));
		}

		private void HandleTouchProp(WorldClient client, MabiPacket packet)
		{
			var propId = packet.GetLong();

			var character = client.GetCreatureOrNull(packet.Id) as MabiPC;
			if (character == null)
				return;

			var success = false;

			var pb = WorldManager.Instance.GetPropBehavior(propId);
			if (pb != null)
			{
				if (character.Region == pb.Prop.Region && WorldManager.InRange(character, (uint)pb.Prop.Info.X, (uint)pb.Prop.Info.Y, 1500))
				{
					success = true;
					pb.Func(client, character, pb.Prop);
				}
			}
			else
			{
				// Dungeon test stuff
				if (character.Region == DGID1)
				{
					var pos = character.GetPosition();
					//client.Send(new MabiPacket(Op.WARP_ENTER, creature.Id).PutByte(1).PutInts(creature.Region, pos.X, pos.Y));
					//
					//
					WorldManager.Instance.CreatureLeaveRegion(character);
					Send.CharacterLock(client, character);

					character.SetLocation(DGID2, 5992, 5614);
					Send.EnterRegionPermission(client, character);

					success = true;
				}
				else
				{
					Logger.Unimplemented("Unknown prop (touch): " + propId.ToString());
				}
			}

			var p = new MabiPacket(Op.TouchPropR, character.Id);
			p.PutByte(success);
			client.Send(p);
		}

		public void HandleHitProp(WorldClient client, MabiPacket packet)
		{
			var propId = packet.GetLong();

			var character = client.GetCreatureOrNull(packet.Id) as MabiPC;
			if (character == null || character.IsDead)
				return;

			// Hit prop animation
			var pos = character.GetPosition();
			WorldManager.Instance.Broadcast(new MabiPacket(Op.HittingProp, character.Id).PutLong(propId).PutInt(2000).PutFloat(pos.X).PutFloat(pos.Y), SendTargets.Region, character);

			// Check for behavior and run it.
			var pb = WorldManager.Instance.GetPropBehavior(propId);
			if (pb != null)
			{
				if (character.Region == pb.Prop.Region && WorldManager.InRange(character, (uint)pb.Prop.Info.X, (uint)pb.Prop.Info.Y, 1500))
				{
					pb.Func(client, character, pb.Prop);
				}
			}
			else
			{
				Logger.Unimplemented("Unknown prop (hit): " + propId.ToString());
			}

			// Send success in any case, just like hit ani.
			client.Send(new MabiPacket(Op.HitPropR, character.Id).PutByte(1));
		}

		private void HandleMove(WorldClient client, MabiPacket packet)
		{
			var x = packet.GetInt();
			var y = packet.GetInt();

			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			var from = creature.GetPosition();
			var to = new MabiVertex(x, y);

#if false
			// Collision
			MabiVertex intersection;
			if (WorldManager.Instance.FindCollisionInTree(creature.Region, pos, dest, out intersection))
			{
				//Logger.Debug("intersection " + intersection);
				// TODO: Uhm... do something.
			}
#endif

			WorldManager.Instance.ActivateMobs(creature, from, to);

			creature.Move(to, (packet.Op == Op.Walk));
		}

		private void HandleTakeOff(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			//Todo: Check if can fly? or use that --v

			if (creature.RaceInfo.FlightInfo == null)
			{
				Logger.Unimplemented("Missing flight info for race '{0}'.", creature.Race);
				client.Send(new MabiPacket(Op.TakeOffR, packet.Id).PutByte(false));
				return;
			}

			if (creature.IsFlying)
			{
				client.Send(new MabiPacket(Op.TakeOffR, packet.Id).PutByte(false));
				return;
			}

			var ascentTime = packet.GetFloat();

			Send.CharacterLock(client, creature, 0xFFFFBDDF);
			foreach (var rider in creature.Riders)
				Send.CharacterLock(client, rider, 0xFFFFBDDF);

			var pos = creature.GetPosition();
			creature.SetPosition(pos.X, pos.Y, 10000);
			creature.IsFlying = true;

			WorldManager.Instance.Broadcast(new MabiPacket(Op.TakingOff, packet.Id).PutFloat(ascentTime), SendTargets.Range, creature);
			client.Send(new MabiPacket(Op.TakeOffR, packet.Id).PutByte(true));
		}

		private void HandleFlyTo(WorldClient client, MabiPacket packet)
		{
			var toX = packet.GetFloat();
			var toH = packet.GetFloat();
			var toY = packet.GetFloat();
			var dir = packet.GetFloat();

			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null || !creature.IsFlying)
				return;

			var pos = creature.GetPosition();

			creature.Direction = (byte)dir;
			creature.Move(new MabiVertex(toX, toY, toH));
			foreach (var rider in creature.Riders)
			{
				rider.Direction = creature.Direction;
				rider.Move(new MabiVertex(toX, toY));
			}

			WorldManager.Instance.Broadcast(new MabiPacket(Op.FlyingTo, packet.Id).PutFloats(toX, toH, toY, dir, pos.X, pos.H, pos.Y), SendTargets.Range, creature);
		}

		private void HandleLand(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			var pos = creature.GetPosition();

			if (!creature.IsFlying /*|| pos.H > 16000*/)
			{
				client.Send(new MabiPacket(Op.CanLand, creature.Id).PutByte(false));
				return;
			}

			Send.CharacterUnlock(client, creature, 0xFFFFBDFF);
			foreach (var rider in creature.Riders)
				Send.CharacterUnlock(client, rider, 0xFFFFBDFF);

			// TODO: angled decent
			creature.SetPosition(pos.X, pos.Y, 0);
			creature.IsFlying = false;

			WorldManager.Instance.Broadcast(new MabiPacket(Op.Landing, creature.Id).PutFloats(pos.X, pos.Y).PutByte(0), SendTargets.Range, creature);
			client.Send(new MabiPacket(Op.CanLand, creature.Id).PutByte(true));
		}

		private void HandleCombatSetTarget(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
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
			WorldManager.Instance.Broadcast(new MabiPacket(Op.CombatSetTarget, creature.Id)
				.PutLong(targetId)
				.PutByte(unk1)
				.PutString(unk2)
			, SendTargets.Range, creature);

			// XXX: Should this better be placed in the skill handlers?
			Send.CombatTargetSet(creature, target);
		}

		private void HandleCombatAttack(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			var targetId = packet.GetLong();
			var target = WorldManager.Instance.GetCreatureById(targetId);

			if (target == null || !target.IsAttackableBy(creature))
			{
				client.Send(new MabiPacket(Op.CombatAttackR, creature.Id));
				return;
			}

			if (creature.Vehicle != null)
				creature = creature.Vehicle;

			// TODO: Check if mount is able to attack anything? (this is done with a status)

			var attackResult = SkillResults.Failure;

			var handler = SkillManager.GetHandler(SkillConst.MeleeCombatMastery) as CombatMasteryHandler;
			if (handler == null)
				return;

			attackResult = handler.Use(creature, targetId);

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
				Send.ServerMessage(client, creature, "Something went wrong here, sry =/");
			}
			else
			{
				// Stunned, missing handler, failure, should be more clear.
				answer.PutByte(0);
			}

			client.Send(answer);

			//if (target != null)
			//    client.Send(new MabiPacket(Op.StunMeter, creature.Id).PutLong(target.Id).PutByte(1).PutFloat(target.Stun));
		}

		public void HandleDeadMenu(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null || !creature.IsDead)
				return;

			var options = new List<DeadMenuOptions>(); // List of flags

			switch (creature.CauseOfDeath)
			{
				case DeathCauses.None:
					client.Send(new MabiPacket(Op.DeadMenuR, creature.Id).PutByte(0));
					return;

				case DeathCauses.Arena:
					if (creature.ArenaPvPManager != null)
						options.AddRange(creature.ArenaPvPManager.GetRevivalOptions(creature));
					break;

				case DeathCauses.TransPvP:
					options.Add(DeadMenuOptions.HereNoPenalty);
					break;

				case DeathCauses.EvG:
				case DeathCauses.PvP:
					options.Add(DeadMenuOptions.HerePvP);
					break;
			}

			if (options.Count == 0) //  They must be in the field
			{
				options.Add(DeadMenuOptions.Town);
				options.Add(DeadMenuOptions.Here);
				options.Add(DeadMenuOptions.NaoStone);
				options.Add(DeadMenuOptions.WaitForRescue);
			}

			if (client.Account.Authority >= 50)
				options.Add(DeadMenuOptions.HereNoPenalty);

			creature.RevivalOptions = options[options.Count - 1];

			for (int i = 0; i < options.Count - 1; i++)
			{
				creature.RevivalOptions |= options[i];
				for (int j = i + 1; j < options.Count; j++)
				{
					options[j] &= ~options[i]; // Eliminate duplicates
				}
			}

			var strOptions = new List<string>();
			foreach (var o in options)
				strOptions.AddRange(DeadMenuHelper.GetStrings(o));

			var response = new MabiPacket(Op.DeadMenuR, creature.Id);
			response.PutByte(1);
			response.PutString(DeadMenuHelper.ConvertToClientString(strOptions));
			response.PutInt(1); // Beginner Nao Stone count
			response.PutInt(1); // Nao Stone Count
			client.Send(response);
		}

		public void HandleRevive(WorldClient client, MabiPacket packet)
		{
			var clientOption = packet.GetInt();

			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null || !creature.IsDead)
				return;

			var option = DeadMenuHelper.ConvertFromClientOption(clientOption);

			if ((option & creature.RevivalOptions) == 0)
			{
				Send.ReviveFail(creature);
				return;
			}

			var pos = creature.GetPosition();

			switch (option)
			{
				case DeadMenuOptions.ArenaLobby:
					creature.ArenaPvPManager.ReviveInLobby(creature);
					break;

				case DeadMenuOptions.ArenaSide:
					creature.ArenaPvPManager.ReviveInArena(creature);
					break;

				case DeadMenuOptions.ArenaWaitingRoom:
					creature.ArenaPvPManager.ReviveInWaitingRoom(creature);
					break;

				case DeadMenuOptions.BarriLobby:
					Logger.Unimplemented("Barri revival (TNN)");
					goto case DeadMenuOptions.Here;

				case DeadMenuOptions.DungeonEntrance:
					Logger.Unimplemented("Dungeon Entrance revival");
					goto case DeadMenuOptions.Here;

				case DeadMenuOptions.Here:
					creature.Injuries = Math.Min(creature.Injuries + creature.LifeInjured * .5f, creature.LifeMax - 5);
					creature.Life = 5;
					//creature.Experience -= creature.Level * .4; //TODO: Look up multiplier
					creature.Revive();
					Send.Revived(creature);
					break;

				case DeadMenuOptions.HereNoPenalty:
					creature.FullHeal();
					creature.Revive();
					Send.Revived(creature);
					break;

				case DeadMenuOptions.HerePvP: // Different... somehow?
					creature.Injuries = Math.Min(creature.Injuries + creature.LifeInjured * .5f, creature.LifeMax - 5);
					creature.Life = 5;
					creature.Revive();
					Send.Revived(creature);
					break;

				case DeadMenuOptions.InCamp:
					Logger.Unimplemented("Camp revival");
					goto case DeadMenuOptions.Here;

				case DeadMenuOptions.NaoStone:
					Send.DeadFeather(creature, DeadMenuOptions.NaoRevival1);
					creature.Client.Send(new MabiPacket(Op.NaoRevivalEntrance, creature.Id));
					Send.ReviveFail(creature);
					creature.RevivalOptions = DeadMenuOptions.NaoRevival1;
					break;

				case DeadMenuOptions.NaoRevival1:
					// TODO: Add item groups, so we can remove on of the
					//   various available soul stones.

					if (!creature.Inventory.Has(75213))
					{
						Send.ReviveFail(creature);
						return;
					}
					creature.Inventory.Remove(75213, 1);

					creature.FullHeal();
					creature.Revive();
					WorldManager.Instance.Broadcast(new MabiPacket(Op.Effect, creature.Id).PutInt(Effect.Revive), SendTargets.Range, creature);
					creature.Client.Send(new MabiPacket(Op.NaoRevivalExit, creature.Id).PutByte(0));
					Send.DeadFeather(creature, DeadMenuOptions.None);
					Send.Revived(creature);
					break;

				case DeadMenuOptions.StatueOfGoddess:
					Logger.Unimplemented("Statue of Goddess revival");
					goto case DeadMenuOptions.Here;

				case DeadMenuOptions.TirChonaill:
				case DeadMenuOptions.Town:
					Logger.Unimplemented("In town revival");
					goto case DeadMenuOptions.Here;

				case DeadMenuOptions.WaitForRescue:
					creature.WaitingForRes = !creature.WaitingForRes;

					if (creature.WaitingForRes)
						Send.DeadFeather(creature, creature.RevivalOptions | DeadMenuOptions.FeatherUp);
					else
						Send.DeadFeather(creature, creature.RevivalOptions);

					Send.Revived(creature);
					break;
			}
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
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null || !creature.IsDead)
				return;

			var eventId = packet.GetLong();
			var type = packet.GetInt();
			var unk3 = packet.GetString();

			// Check if creature is in the same region as the event.
			var region = (eventId & 0x00FFFF00000000) >> 32;
			if (creature.Region != region)
				return;

			var evnt = MabiData.RegionInfoDb.FindEvent(eventId);
			if (evnt == null)
			{
				Logger.Warning("Unknown event: {0}", eventId.ToString("X"));
				return;
			}

			if (evnt.IsAltar)
			{
				if (type == 101)
				{
					creature.OnAltar = (DungeonAltar)eventId;
				}
				else
				{
					creature.OnAltar = DungeonAltar.None;
				}
			}

			creature.LastEventTriggered = eventId;
		}

		public void HandleStunMeterRequest(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null || !creature.IsDead)
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
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			client.Send(new MabiPacket(Op.HomesteadInfoRequestR, creature.Id).PutBytes(0, 0, 1));

			// Seems to be only called on login, good place for the MOTD.
			if (WorldConf.Motd != string.Empty)
				Send.ServerMessage(client, client.Character, WorldConf.Motd);

			EventManager.PlayerEvents.OnPlayerLoggedIn(creature as MabiPC);
		}

		public void HandleMoonGateRequest(WorldClient client, MabiPacket packet)
		{
			//001 [................]  String : _moongate_tara_west
			//002 [................]  String : _moongate_tirchonaill

			// Send no gates for now.
			client.Send(new MabiPacket(Op.MoonGateRequestR, Id.Broadcast));
		}

		public void HandleOpenItemShop(WorldClient client, MabiPacket packet)
		{
			// 1 = succes?, test = key passed to the url
			client.Send(new MabiPacket(0xA44E, client.Character.Id).PutByte(1).PutString(client.Account.Name));
		}

		public void HandleVisualChat(WorldClient client, MabiPacket packet)
		{
			var url = packet.GetString();
			var width = packet.GetShort();
			var height = packet.GetShort();

			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null || !WorldConf.EnableVisual)
				return;

			var p = new MabiPacket(Op.VisualChat, creature.Id);
			p.PutString(creature.Name);
			p.PutString(url);
			p.PutShorts(width, height);
			p.PutByte(0);
			WorldManager.Instance.Broadcast(p, SendTargets.Range, creature);
		}

		public void HandleViewEquipment(WorldClient client, MabiPacket packet)
		{
			var targetId = packet.GetLong();

			if (client.Character.Id != packet.Id)
				return;

			var target = WorldManager.Instance.GetCreatureById(targetId);
			if (target == null /* || TODO: Check visibility. */)
			{
				Send.ViewEquipmentResponse(client, 0, null);
				return;
			}

			Send.ViewEquipmentResponse(client, targetId, target.Inventory.ActualEquipment);
		}

		public void HandleSkillAdvance(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			var skillId = (SkillConst)packet.GetShort();
			var skill = creature.Skills.Get(skillId);
			if (skill == null || !skill.IsRankable)
				return;

			creature.Skills.Give(skill.Id, skill.Rank + 1);

			WorldManager.Instance.CreatureStatsUpdate(creature);
		}

		private void HandleUmbrellaJump(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			uint height = (uint)packet.GetFloat(), x = (uint)packet.GetFloat(), y = (uint)packet.GetFloat();

			creature.SetPosition(x, y);

			WorldManager.Instance.Broadcast(new MabiPacket(Op.UmbrellaJumpR, creature.Id).PutByte(2), SendTargets.Range, creature);
		}

		private void HandleUmbrellaLand(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			WorldManager.Instance.Broadcast(new MabiPacket(Op.MotionCancel2, creature.Id).PutByte(0), SendTargets.Range, creature);
		}

		protected void HandleCollectionRequest(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			var p = new MabiPacket(Op.CollectionRequestR, creature.Id);

			p.PutByte(0);
			p.PutInt(0);

			p.PutByte(1);
			p.PutInt(0);

			p.PutByte(2);
			p.PutInt(0);

			client.Send(p);

			//p.PutByte(0); // Type : 0-Fishing, 1-Cooking, 2-Taming
			//p.PutInt(7);  // Count
			//{
			//    p.PutInt(50253);           // Id (scrapbook)
			//    p.PutInt(4100);
			//    p.PutInt(3002);            // Region
			//    p.PutLong(63469614973173); // Timestamp
			//    p.PutByte(1);
			//    p.PutByte(0);
			//
			//    p.PutInt(50254);
			//    p.PutInt(2200);
			//    p.PutInt(3002);
			//    p.PutLong(63469593461570);
			//    p.PutByte(1);
			//    p.PutByte(0);
			//
			//    p.PutInt(50255);
			//    p.PutInt(6000);
			//    p.PutInt(3002);
			//    p.PutLong(63469450426497);
			//    p.PutByte(1);
			//    p.PutByte(0);
			//
			//    p.PutInt(50256);
			//    p.PutInt(10200);
			//    p.PutInt(3002);
			//    p.PutLong(63469610919920);
			//    p.PutByte(1);
			//    p.PutByte(0);
			//
			//    p.PutInt(50257);
			//    p.PutInt(900);
			//    p.PutInt(3002);
			//    p.PutLong(63469589348657);
			//    p.PutByte(1);
			//    p.PutByte(0);
			//
			//    p.PutInt(50258);
			//    p.PutInt(11000);
			//    p.PutInt(3002);
			//    p.PutLong(63469439821427);
			//    p.PutByte(1);
			//    p.PutByte(0);
			//
			//    p.PutInt(50260);
			//    p.PutInt(9800);
			//    p.PutInt(3002);
			//    p.PutLong(63469451258313);
			//    p.PutByte(1);
			//    p.PutByte(0);
			//}
			//p.PutByte(1);
			//p.PutInt(1);
			//{
			//    p.PutInt(50537);
			//    p.PutInt(0);
			//    p.PutInt(20);
			//    p.PutLong(63481333396833);
			//    p.PutByte(0);
			//}
			//p.PutByte(2);
			//p.PutInt(3);
			//{
			//    p.PutInt(1007);
			//    p.PutInt(420001);
			//    p.PutInt(3300);
			//    p.PutLong(63481310925307);
			//    p.PutFloat(790);
			//    p.PutByte(0);
			//
			//    p.PutInt(1013);
			//    p.PutInt(201701);
			//    p.PutInt(401);
			//    p.PutLong(63481386587503);
			//    p.PutFloat(600);
			//    p.PutByte(0);
			//
			//    p.PutInt(2010);
			//    p.PutInt(201801);
			//    p.PutInt(401);
			//    p.PutLong(63481386642277);
			//    p.PutFloat(600);
			//    p.PutByte(0);
			//}
		}

		protected void HandleShamalaTransformation(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			// Check skill
			if (creature.Skills.Has(SkillConst.TransformationMastery))
			{
				var transId = packet.GetInt();

				// Check transformation
				var strans = MabiData.ShamalaDb.Find(transId);
				if (strans != null)
				{
					// Check character's transformations
					if ((creature as MabiPC).Shamalas.Exists(a => a.Id == transId && a.State == ShamalaState.Available))
					{
						// Check race
						var race = MabiData.RaceDb.Find(strans.Race);
						if (race != null)
						{
							creature.ShamalaRace = race;
							creature.Shamala = strans;

							// Success
							WorldManager.Instance.Broadcast(new MabiPacket(Op.ShamalaTransformation, creature.Id)
								.PutByte(1)
								.PutInt(strans.Id)
								.PutByte(1)
								.PutInt(race.Id)
								.PutFloat(strans.Size)
								.PutInt(strans.Color1)
								.PutInt(strans.Color2)
								.PutInt(strans.Color3)
								.PutByte(0)
								.PutByte(0)
							, SendTargets.Range, creature);
						}
					}
				}
			}

			// Fail
			var p = new MabiPacket(Op.ShamalaTransformation, creature.Id);
			p.PutByte(0);
			client.Send(p);
			return;

			// for reference
			//client.Send(new MabiPacket(Op.ShamalaTransformationUpdate, creature.Id)
			//    .PutInt(9)
			//    .PutByte(1)
			//    .PutByte(2)
			//    .PutByte(2)
			//);
		}

		protected void HandleShamalaTransformationEnd(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			if (creature.Shamala == null)
			{
				client.Send(new MabiPacket(Op.ShamalaTransformationEndR).PutByte(0));
				return;
			}

			creature.Shamala = null;
			creature.ShamalaRace = null;

			// Broadcast end, success with showing ani.
			WorldManager.Instance.Broadcast(new MabiPacket(Op.ShamalaTransformationEndR, creature.Id).PutBytes(1, 1), SendTargets.Range, creature);
		}

		protected void HandleQuestComplete(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			var character = creature as MabiPC;
			var questId = packet.GetLong();

			var quest = character.GetQuestOrNull(questId);
			if (quest == null || !quest.IsDone)
			{
				client.Send(new MabiPacket(Op.QuestCompleteR, creature.Id).PutByte(0));
				return;
			}

			// Set quest complete and complete it with reward.
			quest.State = MabiQuestState.Complete;
			WorldManager.Instance.CreatureCompletesQuest(creature, quest, true);

			// Success
			client.Send(new MabiPacket(Op.QuestCompleteR, creature.Id).PutByte(1));
		}

		protected void HandleQuestGiveUp(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			var character = creature as MabiPC;
			var questId = packet.GetLong();
			var unk = packet.GetByte();

			var quest = character.GetQuestOrNull(questId);
			if (quest == null)
			{
				client.Send(new MabiPacket(Op.QuestGiveUpR, creature.Id).PutByte(0));
				return;
			}

			// Remove quest from char and log, without reward.
			character.Quests.Remove(quest.Class);
			WorldManager.Instance.CreatureCompletesQuest(creature, quest, false);

			// Success
			client.Send(new MabiPacket(Op.QuestGiveUpR, creature.Id).PutByte(1));
		}

		protected void HandleConvertGp(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			creature.Guild = WorldDb.Instance.GetGuildForChar(creature.Id);
			if (creature.Guild == null)
				return;

			Send.ConvertGpR(creature, true, (uint)creature.GuildMember.Gp);
		}

		protected void HandleConvertGpConfirm(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			creature.Guild = WorldDb.Instance.GetGuildForChar(creature.Id);
			if (creature.Guild == null)
				return;

			creature.Guild.Gp += (uint)creature.GuildMember.Gp;
			Send.GuildMessage(creature, Localization.Get("guild.points"), creature.GuildMember.Gp); // Added {0} Point(s)
			creature.GuildMember.Gp = 0;

			creature.Guild.Save();
			creature.GuildMember.Save();

			Send.ConvertGpConfirmR(creature, true);
		}

		protected void HandleGuildDonate(WorldClient client, MabiPacket packet)
		{
			var amount = packet.GetInt();

			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			creature.Guild = WorldDb.Instance.GetGuildForChar(creature.Id);
			if (creature.Guild == null)
				return;

			if (creature.Inventory.Gold < amount)
			{
				Send.GuildDonateR(creature, false);
				return;
			}

			creature.Guild.Gold += amount;
			creature.Inventory.RemoveGold(amount);

			creature.Guild.Save();

			Send.GuildMessage(creature, Localization.Get("guild.donated"), amount); // You have donated {0} Gold
			Send.GuildDonateR(creature, true);
		}

		protected void HandleGuildApply(WorldClient client, MabiPacket packet)
		{
			var guildId = packet.GetLong();
			var appText = packet.GetString();

			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			if (WorldDb.Instance.GetGuildForChar(creature.Id) != null)
			{
				Send.MsgBox(client, creature, Localization.Get("guild.already_you")); // You are already a member of a guild
				Send.GuildApplyR(creature, false);
				return;
			}

			var guild = WorldDb.Instance.GetGuild(guildId);
			if (guild == null)
			{
				Send.MsgBox(client, creature, Localization.Get("guild.not_found")); // Guild does not exist
				Send.GuildApplyR(creature, false);
				return;
			}

			var member = new MabiGuildMember();
			member.CharacterId = creature.Id;
			member.GuildId = guildId;
			member.JoinedDate = DateTime.Now;
			member.Gp = 0;
			member.MemberRank = GuildMemberRank.Applied;
			member.MessageFlags = GuildMessageFlags.None;
			member.ApplicationText = appText;
			member.Save();

			Send.GuildMessage(creature, guild, Localization.Get("guild.pls_wait")); // Your application has been accepted.\nPlease wait [...]
			Send.GuildApplyR(creature, true);
		}

		private void HandlePartyCreate(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			var newParty = new MabiParty(creature);
			newParty.LoadFromPacket(packet);
			WorldManager.Instance.AddParty(newParty);
			creature.Party = newParty;

			var p = new MabiPacket(Op.PartyCreateR, creature.Id).PutByte(1);
			creature.Party.AddPartyPacket(p);
			creature.Client.Send(p);

			WorldManager.Instance.PartyMemberWantedShow(newParty);
		}

		private void HandlePartyJoin(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			var partyLeader = WorldManager.Instance.GetCreatureById(packet.GetLong());
			if (partyLeader.Party == null)
				return;

			var party = partyLeader.Party;
			var password = packet.GetString();
			// var zero = packet.GetByte(); / ?

			if (party.Members.Count >= party.MaxSize)
			{
				creature.Client.Send(new MabiPacket(Op.PartyJoinR, creature.Id).PutByte(0));
				return;
			}

			// Password check
			if (password != party.Password)
			{
				creature.Client.Send(new MabiPacket(Op.PartyJoinR, creature.Id).PutByte(4));
				return;
			}

			// Add new member to party
			party.AddPartyMember(creature);
			creature.Party = party;

			foreach (var member in party.Members)
			{
				if (member != creature)
				{
					var p = new MabiPacket(Op.PartyJoinUpdate, member.Id);
					party.AddMemberPacket(p, creature);
					member.Client.Send(p);
				}
			}

			if (party.IsOpen)
				WorldManager.Instance.PartyMemberWantedRefresh(partyLeader.Party);

			var partyInfoPacket = new MabiPacket(Op.PartyJoinR, creature.Id).PutByte(1);
			partyLeader.Party.AddPartyPacket(partyInfoPacket);
			creature.Client.Send(partyInfoPacket);
		}

		private void HandlePartyLeave(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			if (creature.Party == null)
				return;

			// TODO: Check if allowed to leave party
			var canLeave = true;

			if (canLeave)
				WorldManager.Instance.CreatureLeaveParty(creature);

			creature.Client.Send(new MabiPacket(Op.PartyLeaveR, creature.Id).PutByte(canLeave));
		}

		private void HandlePartyRemove(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			if (creature.Party == null)
				return;

			// TODO: Check if allowed to leave party
			var canRemove = true;

			var target = WorldManager.Instance.GetCreatureById(packet.GetLong());
			if (canRemove)
			{
				WorldManager.Instance.CreatureLeaveParty(target);
				target.Client.Send(new MabiPacket(0xA43C, target.Id).PutLong(creature.Id).PutByte(1).PutByte(1).PutShort(0).PutInt(0));
				target.Client.Send(new MabiPacket(Op.PartyRemoved, target.Id));
			}

			creature.Client.Send(new MabiPacket(Op.PartyRemoveR, creature.Id).PutByte(canRemove));
		}

		private void HandlePartyChangeSettings(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			if (creature.Party == null)
				return;

			if (creature.Party.Leader != creature)
				return;

			var prevType = creature.Party.Type;
			creature.Party.LoadFromPacket(packet);

			foreach (var member in creature.Party.Members)
			{
				if (prevType != creature.Party.Type)
					member.Client.Send(new MabiPacket(Op.PartyTypeUpdate, member.Id).PutInt(creature.Party.Type));
				member.Client.Send(new MabiPacket(Op.PartySettingUpdate, member.Id).PutString(creature.Party.Name));
			}

			if (creature.Party.IsOpen)
				WorldManager.Instance.PartyMemberWantedRefresh(creature.Party);

			var p = new MabiPacket(Op.PartyChangeSettingR, creature.Id).PutByte(1);
			creature.Party.AddPartyPacket(p);
			creature.Client.Send(p);
		}

		private void HandlePartyChangePassword(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			if (creature.Party == null)
				return;

			if (creature.Party.Leader != creature)
				return;

			creature.Party.Password = packet.GetString();

			if (creature.Party.IsOpen)
				WorldManager.Instance.PartyMemberWantedRefresh(creature.Party);

			creature.Client.Send(new MabiPacket(Op.PartyChangePasswordR, creature.Id).PutLong(1));
		}

		private void HandlePartyChangeLeader(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			if (creature.Party == null)
				return;

			if (creature.Party.Leader != creature)
				return;

			// TODO: Check if able to change leader
			var leaderChangeAllow = true;

			if (leaderChangeAllow)
			{
				var newLeader = WorldManager.Instance.GetCreatureById(packet.GetLong());
				WorldManager.Instance.PartyChangeLeader(newLeader, creature.Party);
			}

			creature.Client.Send(new MabiPacket(Op.PartyChangeLeaderR, creature.Id).PutByte(leaderChangeAllow));
		}

		private void HandlePartyWantedHide(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			if (creature.Party == null)
				return;

			if (creature.Party.Leader != creature)
				return;

			WorldManager.Instance.PartyMemberWantedHide(creature.Party);
			creature.Client.Send(new MabiPacket(Op.PartyWantedHideR, creature.Id).PutByte(1));
		}

		private void HandlePartyWantedShow(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			if (creature.Party == null)
				return;

			if (creature.Party.Leader != creature)
				return;

			WorldManager.Instance.PartyMemberWantedShow(creature.Party);
			creature.Client.Send(new MabiPacket(Op.PartyWantedShowR, creature.Id).PutByte(1));
		}

		private void HandlePartyChangeFinish(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			if (creature.Party == null)
				return;

			if (creature.Party.Leader != creature)
				return;

			creature.Party.Finish = (PartyFinishRule)packet.GetInt();
			foreach (var member in creature.Party.Members)
				member.Client.Send(new MabiPacket(Op.PartyFinishUpdate, member.Id).PutInt((uint)creature.Party.Finish));
			creature.Client.Send(new MabiPacket(Op.PartyChangeFinishR, creature.Id).PutByte(1));
		}

		private void HandlePartyChangeExp(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			if (creature.Party == null)
				return;

			if (creature.Party.Leader != creature)
				return;

			creature.Party.ExpShare = (PartyExpSharing)packet.GetInt();
			foreach (var member in creature.Party.Members)
				member.Client.Send(new MabiPacket(Op.PartyExpUpdate, member.Id).PutInt((uint)creature.Party.ExpShare));
			creature.Client.Send(new MabiPacket(Op.PartyChangeExpR, creature.Id).PutByte(1));
		}

		private void HandleServerIdentify(WorldClient client, MabiPacket packet)
		{
			var success = packet.GetBool();

			if (!success)
			{
				Logger.Error("Login Server expected a different password.");
				return;
			}

			client.State = ClientState.LoggedIn;
			this.SendChannelStatus(MabiTime.Now);
		}

		private void HandleCutsceneFinished(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			if (creature.Temp.CurrentCutscene.Leader != creature)
				return;

			client.Send(new MabiPacket(Op.CutsceneEnd, Id.World).PutLong(creature.Id));

			Send.EntityAppearsOthers(creature);
			Send.EntitiesAppear(client, WorldManager.Instance.GetEntitiesInRange(creature));

			Send.CharacterUnlock(client, creature);

			//client.Send(new MabiPacket(Op.CutsceneEnd+1, Id.World).PutLong(creature.Id));

			if (creature.Temp.CurrentCutscene != null)
			{
				if (creature.Temp.CurrentCutscene.OnComplete != null)
					creature.Temp.CurrentCutscene.OnComplete(client);

				creature.Temp.CurrentCutscene = null;
			}
		}

		private void HandleTalentTitleChange(WorldClient client, MabiPacket packet)
		{
			var title = (TalentTitle)packet.GetShort();

			if (client.Character.Id != packet.Id)
				return;

			// TODO: Check if vaild

			// Delay fail
			//0000 [..............00] Byte  : 0
			//0001 [............2777] Short : 10103
			//0002 [..............02] Byte  : 2

			WorldManager.Instance.Broadcast(new MabiPacket(Op.TalentTitleUpdate, client.Character.Id).PutByte(1).PutShort((ushort)title), SendTargets.Range, client.Character);

			client.Character.Talents.SelectedTitle = title;
		}

		private void HandleCombatSetAim(WorldClient client, MabiPacket packet)
		{
			var targetId = packet.GetLong();

			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			creature.Temp.AimStart = DateTime.Now;

			client.Send(new MabiPacket(Op.CombatSetAimR, creature.Id)
			.PutByte(1)
			.PutLong(targetId)
			.PutShort((ushort)creature.ActiveSkillId)
			.PutByte(0));
		}

		/// <summary>
		/// Parameters: None
		/// Description: Sent after Dye skill was prepared.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="packet"></param>
		private void HandleDyePaletteReq(WorldClient client, MabiPacket packet)
		{
			if (client.Character.Id != packet.Id)
				return;

			// Wave parameters for the client's "color pattern change algo".
			var p = new MabiPacket(Op.DyePaletteReqR, client.Character.Id);
			p.PutByte(true);
			p.PutInt(0); //p.PutInt(62);
			p.PutInt(0); //p.PutInt(123);
			p.PutInt(0); //p.PutInt(6);
			p.PutInt(0); //p.PutInt(238);
			client.Send(p);
		}

		/// <summary>
		/// Parameters:
		///		ulong  Item Object Id
		/// Description: Sent when clicking "Pick Color".
		/// </summary>
		/// <param name="client"></param>
		/// <param name="packet"></param>
		private void HandleDyePickColor(WorldClient client, MabiPacket packet)
		{
			var itemId = packet.GetLong();

			if (client.Character.Id != packet.Id)
				return;

			var item = client.Character.Inventory.GetItem(itemId);
			if (item == null)
			{
				Send.DyePickColorR(client.Character, false);
				return;
			}

			if (WorldConf.SafeDye)
			{
				client.Character.Temp.DyeCursors = new byte[20];
			}
			else
			{
				// 5x x+y. First byte is +, second -?
				client.Character.Temp.DyeCursors = new byte[]
				{ 
					0x00, 0x00, 0x00, 0x00, // Color Picker 1
					0xF5, 0xFF, 0xF5, 0xFF, // Color Picker 2
					0x0A, 0x00, 0xF5, 0xFF, // Color Picker 3
					0xF5, 0xFF, 0x0A, 0x00,	// Color Picker 4
					0x0A, 0x00, 0x0A, 0x00,	// Color Picker 5
				};
			}

			Send.DyePickColorR(client.Character, true);
		}

		private void HandleChannelStatus(WorldClient client, MabiPacket packet)
		{
			// TODO: Update channel list... and TODO: Add channel list.
		}

		private void HandleCancelBeautyShop(WorldClient client, MabiPacket packet)
		{
			client.Send(new MabiPacket(Op.CancelBeautyShopR, client.Character.Id));
		}

		/// <summary>
		/// Parameters: None
		/// Description: Sent when closing the chat with Nao.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="packet"></param>
		private void HandleLeaveSoulStream(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			client.Send(new MabiPacket(Op.LeaveSoulStreamR, Id.World));

			Send.CharacterLock(client, creature);
			Send.EnterRegionPermission(client, creature);
		}

		protected void HandleOptionSet(WorldClient client, MabiPacket packet)
		{
			var creature = client.GetCreatureOrNull(packet.Id);
			if (creature == null)
				return;

			var response = new MabiPacket(Op.OptionSetR, creature.Id);

			var count = packet.GetByte();
			response.PutByte(count);
			for (byte i = 0; i < count; i++)
			{
				var id = packet.GetByte();
				response.PutByte(id);
				packet.GetByte(); // Padding byte?
				var requestedState = packet.GetBool();

				switch (id)
				{
					case 1: // Trans pvp
						creature.TransPvPEnabled = requestedState;
						response.PutByte(1);
						response.PutByte(requestedState);
						Send.PvPInformation(creature);
						break;

					case 2: // EvG
						creature.EvGEnabled = requestedState;
						response.PutByte(1);
						response.PutByte(requestedState);
						Send.PvPInformation(creature);
						break;

					case 5: // All pvp Enabled
						if (!requestedState)
							creature.EvGEnabled = creature.TransPvPEnabled = requestedState;
						response.PutByte(1);
						response.PutByte(requestedState);
						Send.PvPInformation(creature);
						break;

					case 3: // Equip View
					case 4: // Pet Finish
					case 6: // Journal Public
					case 7: // Cutscene skips
					default:
						response.PutByte(0);
						response.PutByte(!requestedState);
						break;
				}
			}

			client.Send(response);
		}

        private void HandleShadowMissionList(WorldClient client, MabiPacket packet)
        {
            byte difficulty = packet.GetByte();
            uint boardClassId = packet.GetInt(); // Class Id of SM board
            MissionBoard board = MissionManager.Instance.GetShadowMissionBoard(boardClassId);

            var response = new MabiPacket(Op.ShadowMissionListR, packet.Id);
            board.AddToPacket(response, difficulty);
            client.Send(response);
        }

        private void HandleShadowMissionMap(WorldClient client, MabiPacket packet)
        {
            String echo = packet.GetString();
            uint classId = packet.GetInt();

            var sm = MissionManager.Instance.GetShadowMissionInfo(classId);
            if (sm == null) return; // Do nothing

            MabiPacket response = new MabiPacket(Op.ShadowMissionMapR, packet.Id);
            response.PutString(echo);
            sm.AddMapDataToPacket(response);
            client.Send(response);
        }

        private void HandleShadowMissionAccept(WorldClient client, MabiPacket packet)
        {
            var creature = client.GetCreatureOrNull(packet.Id);
            if (creature == null)
                return;

            if (!(creature is MabiPC))
            {
                Send.ShadowMissionAcceptR(creature, false, "You don't exist!");
                return;
            }

            var player = creature as MabiPC;

            // Check if they already have a SM quest
            if (player.GetShadowMissionQuestOrNull() != null)
            {
                // TODO: Change text to official Mabi's text later
                Send.ShadowMissionAcceptR(creature, false, "You already have a Shadow Mission");
                return;
            }

            uint classId = packet.GetInt();
            byte difficulty = packet.GetByte();

            // Temp check for now, if it's really a SM
            if (classId < 700000 || classId >= 800000)
            {
                Send.ShadowMissionAcceptR(creature, false, "The quest requested is not a Shadow Mission");
                return;
            }

            var sm = MissionManager.Instance.GetShadowMissionInfo(classId);
            if (sm == null)
            {
                Send.ShadowMissionAcceptR(creature, false, "The Shadow Mission quest could not be found");
                return;
            }

            // Difficulty check
            if (!sm.SupportsDifficulty(difficulty))
            {
                Send.ShadowMissionAcceptR(creature, false,
                    "Selected Shadow Mission does not support selected difficulty");
                return;
            }

            var quest = sm.GenerateQuestOrNull(creature, difficulty);
            if (quest == null)
            {
                Send.ShadowMissionAcceptR(creature, false, "Error generating quest");
                return;
            }

            // Add the quest, should be thread safe?
            player.Quests.Add(classId, quest);

            // See BaseScript.Quests.cs for examples

            Send.ShadowMissionAcceptR(creature, true);

            Send.ItemInfo(player.Client, creature, quest.QuestItem);
            Send.QuestNew(player, quest);
            //WorldManager.Instance.CreatureReceivesQuest(creature, quest);

            // Should probably be a function for this.. manual send for now
            var questUpdate = new MabiPacket(Op.QuestUpdate, creature.Id)
                .PutLong(quest.Id)
                .PutByte(1) // Can quit this mission
                .PutByte(0)
                .PutInt(0)
                .PutByte(0)
                .PutByte(0);
            client.Send(questUpdate);

            // Placeholder function name:
            // if(CanPartakeInShadowMission(creature, classId, difficulty))
            {
                // Right now, just accept
                // var sm = WorldManager.Instance.GetShadowMissionInfo(classId);
            }
            //  else // Untested
            //  {
            //      
            //  }
        }

        private void HandleShadowMissionExit(WorldClient client, MabiPacket packet)
        {
            //var sm = client.ShadowMission;

            // New strat: check region instance Id (ex. 35,000) and try to get mission from it?

            //if (sm != null) sm.Exit(client.Character as MabiPC); // Might exception

            // Exit will need thread safety/locking..
            // Otherwise Exit request spamming could cause problems
        }
	}
}
