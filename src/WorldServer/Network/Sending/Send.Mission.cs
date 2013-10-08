using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aura.Shared.Network;
using Aura.World.World;
using Aura.Data;

namespace Aura.World.Network
{
    public static partial class Send
    {
        public static void AddMissionList(this MabiPacket packet, MissionBoard board, byte difficulty)
        {
            // Even if difficulty not supported, add difficulty to packet
            // Otherwise client will rapid request, this way it thinks empty list
            packet.PutByte(difficulty);

            // Throw an exception? Should never happen unless
            // custom crafted request or modified client
            if (!board.IsDifficultySupported(difficulty))
                return;

            foreach (MissionInfo info in board.Missions.Values)
            {
                if (info == null || !info.SupportsDifficulty(difficulty)) continue;
                info.AddToPacket(packet, difficulty);
            }
        }

        public static void AddMissionInfo(this MabiPacket packet, MissionInfo info, byte difficulty)
        {
            if (!info.SupportsDifficulty(difficulty)) throw new Exception(
                String.Format("Difficulty {1} unsupported for mission {0}", info.Class, difficulty));

            List<QuestRewardInfo> rewards = null;
            try
            {
                rewards = info.Rewards[difficulty];
            }
            catch { }

            if (rewards == null) rewards = new List<QuestRewardInfo>(); // Empty

            // Just QuestInfo's rewards
            info.QuestInfo.Rewards = rewards;

            packet.PutInt(info.Class);
            packet.PutByte(difficulty);
            packet.PutString(info.Name);
            packet.PutByte(info.PartyCountMin);
            packet.PutByte(info.PartyCountMax);
            packet.PutInt(info.TimeLimit);
            packet.PutInt(info.Unknown7);
            packet.PutString(info.Description);
            packet.PutString(""); // Always blank? Might be used somewhere
            packet.PutString(info.QuestInfo.GetRewardsString());
            packet.PutInt(info.Unknown11); // These last 5 are not consts, they have meaning, but no idea what
            packet.PutInt(info.Unknown12);
            packet.PutInt(info.Unknown13);
            packet.PutInt(info.Unknown14);
            packet.PutByte(info.Special); // Almost always 0, but have seen as 3? (The Other Alchemists = 3, Their Method = 1)
        }

        public static void AddMissionMapData(this MabiPacket packet, MissionInfo info)
        {
            // Op: 0x8D6C
            // First argument is a String which echos the String sent in request, so
            // no need to handle that here

            packet.PutInt(info.MapRegion);
            for (int i = 0; i < 4; i++)
            {
                if (info.MapCrop.Length > i)
                    packet.PutShort(info.MapCrop[i]);
                else packet.PutShort(0);
            }

            packet.PutInt((uint)info.MapMarkCoords.Count);
            foreach (Tuple<uint, uint> coords in info.MapMarkCoords)
            {
                packet.PutInt(coords.Item1);
                packet.PutInt(coords.Item2);
            }
        }

        public static void ShadowMissionAcceptR(MabiCreature creature, bool success, string msg = null)
        {
            var packet = new MabiPacket(Op.ShadowMissionAcceptR, creature.Id); // Might not use creature.Id, assumption
            packet.PutByte(success);
            if (!success && msg != null)
                packet.PutString(msg);
            creature.Client.Send(packet);
        }

    }
}
