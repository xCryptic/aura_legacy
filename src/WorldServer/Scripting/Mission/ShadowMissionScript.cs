// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see licence.txt in the main folder

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aura.Data;
using Aura.World.World;

namespace Aura.World.Scripting
{
    /// <summary>
    /// 
    /// </summary>
    public class ShadowMissionScript : MissionScript
    {
        public MissionInfo MissionInfo = new MissionInfo();

        public void AddToQuest()
        {
            // Manually add to the QuestDb
            var e = Aura.Data.MabiData.QuestDb.Entries;
            if (!e.ContainsKey(this.MissionInfo.Class))
                e.Add(this.MissionInfo.Class, this.MissionInfo.QuestInfo);
            else e[this.MissionInfo.Class] = this.MissionInfo.QuestInfo;
        }

        public MissionInfo.Region AddRegion(uint mapId)
        {
            return this.MissionInfo.AddRegion(mapId);
        }

        /// <summary>
        /// Add this Shadow Mission to a Shadow Mission board.
        /// </summary>
        /// <param name="classId">Class Id of board to add to</param>
        public void AddToBoard(uint classId)
        {
            MissionManager.Instance.AddShadowMission(classId, this.MissionInfo);
        }

        public void AddToAltar(ulong propId)
        {
            this.MissionInfo.AddAltar(propId);
        }

        public void Complete(MabiMission mission, bool receiveRewards = true)
        {
            mission.Complete(receiveRewards);
        }

        /// <summary>
        /// Perform the finishing touches after a mission's info has been initialized.
        /// </summary>
        public void FinishMissionInit()
        {
            // Add to QuestInfo database
            this.AddToQuest();
        }

        protected void HookMissionStart(MabiMission.Callback callback)
        {
            this.MissionInfo.OnMissionStart = callback;
        }

        public void SetClass(uint classId)
        {
            this.MissionInfo.Class = classId;
        }

        public void SetDefaultExit(ulong altarId, uint regionId, uint x, uint y)
        {
            this.MissionInfo.ExitRegion = regionId;
            this.MissionInfo.ExitSpawnX = x;
            this.MissionInfo.ExitSpawnY = y;
        }

        public void SetDirectoryName(MissionInfo.Region region, string directory)
        {
            region.DirectoryName = directory;
        }

        public void SetName(string name)
        {
            this.MissionInfo.Name = name;
        }

        public void SetDescription(string desc)
        {
            this.MissionInfo.Description = desc;
        }

        public void SetPartyRange(byte lower, byte upper)
        {
            this.MissionInfo.PartyCountMin = lower;
            this.MissionInfo.PartyCountMax = upper;
        }

        public void SetTimeLimit(uint limit)
        {
            this.MissionInfo.TimeLimit = limit;
        }

        public void SetMapRegion(uint mapRegion)
        {
            this.MissionInfo.MapRegion = mapRegion;
        }

        public void SetMapCrop(ushort x1, ushort y1, ushort x2, ushort y2)
        {
            this.MissionInfo.SetMapCrop(x1, y1, x2, y2);
        }

        public void AddMapMarking(uint x, uint y)
        {
            this.MissionInfo.AddMapMarking(x, y);
        }

        public void SetReward(Difficulty difficulty, uint exp, uint gold)
        {
            this.SetReward((byte)difficulty, exp, gold);
        }

        public void SetReward(byte difficulty, uint exp, uint gold)
        {
            this.MissionInfo.SetReward(difficulty, exp, gold);
        }

        public void SetSpawn(uint index, uint x, uint y)
        {
            this.MissionInfo.SetSpawn(index, x, y);
        }

        public void SetUnknown7(uint unk)
        {
            this.MissionInfo.Unknown7 = unk;
        }

        public void SetUnknown11(uint unk)
        {
            this.MissionInfo.Unknown11 = unk;
        }

        public void SetUnknown12(uint unk)
        {
            this.MissionInfo.Unknown12 = unk;
        }

        public void SetUnknown13(uint unk)
        {
            this.MissionInfo.Unknown13 = unk;
        }

        public void SetUnknown14(uint unk)
        {
            this.MissionInfo.Unknown14 = unk;
        }

        public void SetSpecial(byte unk)
        {
            // Special
            this.MissionInfo.Special = unk;
        }

        public void SetRegionUnknown1(MissionInfo.Region region, uint unk)
        {
            region.Unknown1 = unk;
        }

        public void SetRegionUnknown2(MissionInfo.Region region, byte unk)
        {
            region.Unknown2 = unk;
        }

        public void SetVariationFile(MissionInfo.Region region, string file)
        {
            region.VariationFile = file;
        }

        public void SupportDifficulty(Difficulty d)
        {
            this.MissionInfo.SupportDifficulty(d);
        }

        public void SupportDifficulty(byte d)
        {
            this.MissionInfo.SupportDifficulty(d);
        }

        public void SupportDifficulties(params Difficulty[] d)
        {
            this.MissionInfo.SupportDifficulties(d);
        }

        public void SupportDifficulties(params byte[] d)
        {
            this.MissionInfo.SupportDifficulties(d);
        }

        public void SupportDifficultyUpTo(Difficulty d)
        {
            this.SupportDifficultyUpTo((byte)d);
        }

        public void SupportDifficultyUpTo(byte d)
        {
            for (byte i = 0; i <= d; i++) this.SupportDifficulty(i);
        }

        public void SupportDifficultyRange(Difficulty a, Difficulty b)
        {
            this.SupportDifficultyRange((byte)a, (byte)b);
        }

        public void SupportDifficultyRange(byte a, byte b)
        {
            byte s = 0, e = 0;
            if (a < b) { s = a; e = b; }
            else { s = b; e = a; }

            for (byte i = s; i <= e; i++) this.SupportDifficulty(i);
        }

        /// <summary>
        /// Shortcut function for setting up a Tara shadow mission. Will set the
        /// default exit, add the altar hook, and add to the Tara board.
        /// </summary>
        protected virtual void InitTaraMission()
        {
            SetDefaultExit(0, 401, 81255, 126210); // Tara exit
            AddToBoard(41699); // Shadow Mission board prop it's tied to (Tara)
            AddToAltar(0x00A00191000D001E); // Altar prop it's tied to (Tara)
        }

        /// <summary>
        /// Shortcut function for setting up a Tail shadow mission. Will set the
        /// default exit, add the altar hook, and add to the Tail board.
        /// </summary>
        protected virtual void InitTailMission()
        {
            // TODO
        }

        public uint GetRegionInstanceId(MabiMission mission, uint regionIndex = 0)
        {
            if (regionIndex >= mission.Regions.Length)
                throw new Exception("Tried to get a RegionHandler for a region that does not exist");
            return mission.Regions[regionIndex].Id;
        }
    }
}