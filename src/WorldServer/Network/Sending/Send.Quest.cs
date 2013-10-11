// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see licence.txt in the main folder

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aura.Shared.Network;
using Aura.World.World;
using Aura.Shared.Const;
using Aura.Shared.Util;
using Aura.World.Player;
using System.Collections;
using Aura.Data;

namespace Aura.World.Network
{
	public static partial class Send
	{
		public static void QuestNew(MabiPC character, MabiQuest quest)
		{
			var packet = new MabiPacket(Op.QuestNew, character.Id);
			packet.AddQuest(quest);

			character.Client.Send(packet);
		}

		public static void QuestUpdate(MabiCreature creature, MabiQuest quest)
		{
			var packet = new MabiPacket(Op.QuestUpdate, creature.Id);
			packet.AddQuestProgress(quest);

			creature.Client.Send(packet);
		}

		private static void AddQuest(this MabiPacket packet, MabiQuest quest)
		{
			if (quest.Info == null)
				return;

			packet.PutLong(quest.Id);
			packet.PutByte(0);

			packet.PutLong(quest.ItemId);

			//packet.PutByte(2);
            // Quest type, changes how quest is displayed
            packet.PutByte(quest.Info.Type); // 0 = blue icon, 7 (shadow? changes structure slightly)

			packet.PutInt(quest.Info.Id); // 200076, range important for the tabs.

			packet.PutString(quest.Info.Name);
			packet.PutString(quest.Info.Description);
			packet.PutString(quest.Info.AdditionalInfo);

            /*
			packet.PutInt(1);
			packet.PutInt(70024); // Item "Hunting Quest" ?
			packet.PutByte(quest.Info.Cancelable);
			packet.PutByte(0);
			packet.PutByte(0); // 1 = blue icon
			packet.PutByte(0);
            */
            
            packet.PutInt(1);
            packet.PutInt(quest.Info.Unknown1); // 70024, Item "Hunting Quest" ?
            packet.PutByte(quest.Info.Cancelable);
            packet.PutByte(0);
            packet.PutByte(quest.Info.Unknown2); // 1 = blue icon
            packet.PutByte(0);

            // [180300, NA166 (18.09.2013)] ?
            {
                packet.PutByte(0);
                packet.PutByte(0);
                packet.PutByte(0);
            }

			packet.PutString(""); // data\gfx\image\gui_temporary_quest.dds
			packet.PutInt(0);     // 4, x y ?
			packet.PutInt(0);
			packet.PutString(""); // <xml soundset="4" npc="GUI_NPCportrait_Lanier"/>
			//packet.PutString("QMBEXP:f:1.000000;QMBGLD:f:1.000000;QMSMEXP:f:1.000000;QMSMGLD:f:1.000000;QMAMEXP:f:1.000000;QMAMGLD:f:1.000000;QMBHDCTADD:4:0;QMGNRB:f:1.000000;QMGNRB:f:1.000000;");
            packet.PutString((quest.Tags == null ? MabiQuest.DefaultTags().ToString() : quest.Tags.ToString()));

			// Alternative, PTJ
			//020 [........00000002] Int    : 2
			//021 [........0000000C] Int    : 12
			//022 [........00000010] Int    : 16
			//023 [........00000015] Int    : 21
			//024 [000039BF89671150] Long   : 63494806770000 // Timestamp

            // These aren't in (shadow?) missions, so.. simple check for now
            if (quest.Info.Type != 7)
            {
                packet.PutInt(0);
                packet.PutInt(0);

                packet.PutSInt(quest.Info.Objectives.Count);
                foreach (DictionaryEntry de in quest.Info.Objectives)
                {
                    var key = de.Key as string;
                    var objective = de.Value as QuestObjectiveInfo;
                    var progress = quest.Progresses[key] as MabiQuestProgress;

                    packet.PutByte((byte)objective.Type);
                    packet.PutString(objective.Description);
                    packet.PutString(objective.ToString());

                    // 3  - TARGECHAR:s:shamala;TARGETCOUNT:4:1; - Ask Shamala about collecting transformations
                    // 14 - TARGETITEM:4:40183;TARGETCOUNT:4:1; - Break a nearby tree
                    // 1  - TGTSID:s:/brownphysisfoxkid/;TARGETCOUNT:4:10;TGTCLS:2:0; - Hunt 10 Young Brown Physis Foxes
                    // 9  - TGTSKL:2:23002;TGTLVL:2:1;TARGETCOUNT:4:1; - Combat Mastery rank F reached
                    // 19 - TGTCLS:2:3906;TARGETCOUNT:4:1; - Win at least one match in the preliminaries or the finals of the Jousting Tournament.
                    // 18 - TGTCLS:2:3502;TARGETCOUNT:4:1; - Read the Author's Notebook.
                    // 4  - TARGECHAR:s:duncan;TARGETITEM:4:75473;TARGETCOUNT:4:1; - Deliver the Author's Notebook to Duncan.
                    // 15 - TGTLVL:2:15;TARGETCOUNT:4:1; - Reach Lv. 15
                    // 2  - TARGETITEM:4:52027;TARGETCOUNT:4:10;QO_FLAG:4:1; - Harvest 10 Bundles of Wheat
                    // 22 - TGTSID:s:/ski/start/;TARGETITEM:4:0;EXCNT:4:0;TGITM2:4:0;EXCNT2:4:0;TARGETCOUNT:4:1; - Click on the Start Flag.
                    // 47 - TARGETCOUNT:4:1;TGTCLS:4:730205; - Clear the Monkey Mash Mission.
                    // 52 - QO_FLAG:b:true;TARGETCOUNT:4:1; - Collect for the Transformation Diary
                    // 50 - TARGETRACE:4:9;TARGETCOUNT:4:1; - Transform into a Kiwi.
                    // 54 - TARGETRACE:4:9;TARGETCOUNT:4:1; - Collect Frail Green Kiwi perfectly.

                    // Type theory:
                    // 1  : Kill x of y
                    // 2  : Collect x of y
                    // 3  : Talk to x
                    // 4  : Bring x to y
                    // 9  : Reach rank x on skill y
                    // 14 : ?
                    // 15 : Reach lvl x
                    // 18 : Do something with item x ?
                    // 19 : Clear something, like jousting or a dungeon?

                    // Progress
                    packet.PutInt(progress.Count);
                    packet.PutByte(progress.Done);
                    packet.PutByte(progress.Unlocked);

                    // Target location
                    if (objective.Region > 0)
                    {
                        packet.PutByte(1);
                        packet.PutInt(objective.Region);
                        packet.PutInt(objective.X);
                        packet.PutInt(objective.Y);
                    }
                    else
                    {
                        packet.PutByte(0);
                    }
                }
            }
            else // (Shadow) Mission, possible different types of quests
                 // are serialized completely differently
            {
                //var d = this.Info.Objectives;
                //if (d == null || d.Count == 0) throw new System.Exception(); // Or something
                //
                //var objective = d[0] as QuestObjectiveInfo;
                packet.PutString(quest.Info.TooltipDescription);

                packet.PutByte(quest.ShowExitButton ? (byte)1 : (byte)0);
                packet.PutLong(0); // Set when entering mission, ex. 0x000039C22DDD8151
                packet.PutInt(quest.Info.TimeLimit); // Some time limit
                packet.PutInt(quest.Info.PartyCountMin); // Min party
                packet.PutInt(quest.Info.PartyCountMax); // Max party
                packet.PutByte(0); // ?
                packet.PutString(""); // ?
                packet.PutByte(0); // ?
                packet.PutInt(0); // ?
            }

			packet.PutByte(1);
			packet.PutByte(0);
			packet.PutByte(0);
			packet.PutByte(1);

			// Rewards
			packet.PutByte((byte)quest.Info.Rewards.Count);
			foreach (var reward in quest.Info.Rewards)
			{
				packet.PutByte((byte)reward.Type);
				packet.PutString(reward.ToString());
				packet.PutByte(reward.Group);
				packet.PutByte(1);
				packet.PutByte(1);
			}

			packet.PutByte(0);
		}

		private static void AddQuestProgress(this MabiPacket packet, MabiQuest quest)
		{
			packet.PutLong(quest.Id);
			packet.PutByte(1);

			packet.PutSInt(quest.Progresses.Count);
			foreach (MabiQuestProgress p in quest.Progresses.Values)
			{
				packet.PutInt(p.Count);
				packet.PutByte(p.Done);
				packet.PutByte(p.Unlocked);
			}

			packet.PutByte(0);
			packet.PutByte(0);
		}
	}
}
