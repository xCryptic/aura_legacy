﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see licence.txt in the main folder

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aura.Data;
using Aura.Shared.Const;
using Aura.Shared.Network;
using Aura.World.Network;
using Aura.World.Scripting;
using Aura.Shared.Util;
using Aura.World.Player;

namespace Aura.World.World
{
    // [#SM]
    public class MissionBoard
    {
        // Altars:
        // Tail: 0x00A0012C00120099
        // Tara: 0x00A00191000D001E

        // Placeholder for now...
        //public static enum Prop : ulong
        //{
        //    Misc = 0,
        //    Tail = 0x00A0012C00120001,
        //    Tara = 0x00A00191000D01C4,
        //}

        // Key is Quest "classId"?
        public SortedDictionary<uint, MissionInfo> Missions = new SortedDictionary<uint, MissionInfo>();

        /// <summary>
        /// Check if a difficulty value is supported.
        /// </summary>
        /// <param name="dif">Difficulty</param>
        /// <returns>true if supported, false if not</returns>
        public Boolean IsDifficultySupported(byte dif)
        {
            // Basic - Lord
            return (dif >= (byte)Difficulty.Basic && dif <= (byte)Difficulty.Lord);
        }

        /// <summary>
        /// Add a mission to this board.
        /// </summary>
        /// <param name="info">Info on mission to add</param>
        public void AddMissionInfo(MissionInfo info)
        {
            this.Missions.Add(info.Class, info);
        }

        public void AddToPacket(MabiPacket packet, Difficulty difficulty)
        {
            this.AddToPacket(packet, (byte)difficulty);
        }

        // Difficulty is sent with 0x8EBF request, along with prop Id of mission board
        public void AddToPacket(MabiPacket packet, Byte difficulty)
        {
            // Even if difficulty not supported, add difficulty to packet
            // Otherwise client will rapid request, this way it thinks empty list
            packet.PutByte(difficulty);

            // Throw an exception? Should never happen unless
            // custom crafted request or modified client
            if (!this.IsDifficultySupported(difficulty))
                return;

            //bool first = true;
            foreach (MissionInfo info in this.Missions.Values)
            {
                if (info == null || !info.SupportsDifficulty(difficulty)) continue;

                // Null byte separator
                //if (!first) packet.PutByte(0);

                info.AddToPacket(packet, difficulty);

                //first = false;
            }
        }

        /// <summary>
        /// Get the ShadowMissionInfo of the specified class Id.
        /// </summary>
        /// <param name="classId">Class Id of mission (quest)</param>
        /// <returns>ShadowMissionInfo, or null if not found</returns>
        public MissionInfo GetMissionInfo(uint classId)
        {
            MissionInfo info = null;
            this.Missions.TryGetValue(classId, out info);
            return info;
        }
    }

    /***
     * Replaced with Quest reward
     ***
    public class ShadowMissionReward
    {
        public uint Gold = 0;
        public uint Experience = 0;

        public ShadowMissionReward(uint exp, uint gold)
        {
            this.Experience = exp;
            this.Gold = gold;
        }

        // Used in packet
        public override string ToString()
        {
 	         return String.Format("{1} Experience Point{0}G", this.Gold, this.Experience);
        }
    } 
    ***/
    
    /// <summary>
    /// Contains info (metadata) on a mission. (Move to Aura.Data eventually?)
    /// </summary>
    public class MissionInfo
    {
        /// <summary>
        /// QuestInfo associated with this mission.
        /// </summary>
        public QuestInfo QuestInfo = new QuestInfo();

        /// <summary>
        /// Function called when starting the mission. Usually set by
        /// script function HookMissionStart()
        /// </summary>
        public MabiMission.Callback OnMissionStart = null;

        // Arg 1, class Id
        // public uint Class = 0;

        // Arg 2, sent as difficulty in packet, but we don't want to
        // need four identical shadow missions with just different
        // difficulty, for now just set a max
        //public byte MaxDifficulty = (byte)Difficulty.Elite;
        public List<byte> Difficulties = new List<byte>(); // Could also be a flag..? Technically 256 possible difficulties

        // Arg 3
        //public String Name = "";

        // Args 4/5
        //public byte PartyCountMin = 1;
        //public byte PartyCountMax = 8;

        // Arg 6, in milliseconds (0 if no time limit)
        // public uint TimeLimit = 0;

        public byte PartyCountMin
        {
            get { return this.QuestInfo.PartyCountMin; }
            set { this.QuestInfo.PartyCountMin = value; }
        }

        public byte PartyCountMax
        {
            get { return this.QuestInfo.PartyCountMax; }
            set { this.QuestInfo.PartyCountMax = value; }
        }

        public uint TimeLimit
        {
            get { return this.QuestInfo.TimeLimit; }
            set { this.QuestInfo.TimeLimit = value; }
        }

        /// <summary>
        /// Current special flag for this mission.
        /// </summary>
        public byte Special
        {
            get { lock (_specialLock) return _special; }
            set { lock (_specialLock) _special = value; }
        }

        // Arg 8
        //public String Description = "";

        // Rewards (part of Arg 10)
        public uint Gold = 0;
        public uint Experience = 0;

        // Unknown stuff, will need to just be copy/pasted from
        // packets sent by Nexon's server for now (should be constant per mission?)
        public uint Unknown7 = 70501; // Also used in MabiQuest, set to QuestInfo if purpose ever discovered
        public uint Unknown11 = 0;
        public uint Unknown12 = 0;
        public uint Unknown13 = 0;
        public uint Unknown14 = 0;
        //public byte Unknown15 = 0;

        private byte _special = 0; // Previously Unknown15
        private Object _specialLock = new Object(); // Special value may be changing after init

        // Rewards sorted by difficulty
        public SortedDictionary<byte, List<QuestRewardInfo>> Rewards = new SortedDictionary<byte, List<QuestRewardInfo>>();

        /// <summary>
        /// Info on regions involved in this Shadow Mission.
        /// </summary>
        public List<MissionRegionInfo> Regions = new List<MissionRegionInfo>();

        // Data sent in 0x8D6C (map data)

        /// <summary>
        /// Region Id of the map to be used.
        /// </summary>
        public uint MapRegion = 0;

        /// <summary>
        /// Map crop data: X1, Y1, X2, Y2
        /// </summary>
        public ushort[] MapCrop = new ushort[4];

        /// <summary>
        /// List of coordinates for "!" marks on the map
        /// </summary>
        public List<Tuple<uint, uint>> MapMarkCoords = new List<Tuple<uint, uint>>(); // "!" marks on the map

        // Data sent in 0xA97E (region data, sent upon entering mission)
        // Might be sent for both character and pet if you have a pet out when entering?

        /// <summary>
        /// List of coordinates for user and pet spawns.
        /// </summary>
        public Dictionary<uint, Tuple<uint, uint>> UserSpawnCoords
            = new Dictionary<uint, Tuple<uint, uint>>();

        /// <summary>
        /// List of prop Ids which this Mission can be launched from, if any.
        /// Included to restrict starting the mission from anywhere?
        /// </summary>
        public List<ulong> Triggers = new List<ulong>();

        // Implement this better
        public uint ExitRegion = 0, ExitSpawnX = 0, ExitSpawnY = 0;

        public uint Class
        {
            get { return this.QuestInfo.Id; }
            set { this.QuestInfo.Id = value; }
        }

        public String Name
        {
            get { return this.QuestInfo.Name; }
            set { this.QuestInfo.Name = value; }
        }

        public String Description
        {
            get { return this.QuestInfo.Description; }
            set { this.QuestInfo.Description = value; }
        }

        public String AdditionalInfo
        {
            get { return this.QuestInfo.AdditionalInfo; }
            set { this.QuestInfo.AdditionalInfo = value; }
        }

        public String QuestDescription
        {
            get { return this.QuestInfo.TooltipDescription; }
            set { this.QuestInfo.TooltipDescription = value; }
        }

        public MissionInfo()
        {
            this.QuestInfo.Cancelable = true;
        }

        /// <summary>
        /// Add a supported trigger.
        /// </summary>
        /// <param name="propId">Prop Id of trigger</param>
        public void AddTrigger(ulong propId)
        {
            this.Triggers.Add(propId);
        }

        public void AddMapMarking(uint x, uint y)
        {
            this.MapMarkCoords.Add(new Tuple<uint, uint>(x, y));
        }

        public void AddToPacket(MabiPacket packet, Difficulty difficulty)
        {
            this.AddToPacket(packet, (byte)difficulty);
        }

        /// <summary>
        /// Add this mission data to a packet.
        /// TODO: Make full packet cached
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="difficulty"></param>
        public void AddToPacket(MabiPacket packet, byte difficulty)
        {
            if (!this.SupportsDifficulty(difficulty)) throw new Exception(
                String.Format("Difficulty {1} unsupported for mission {0}", this.Class, difficulty));

            List<QuestRewardInfo> rewards = null;
            try
            {
                rewards = this.Rewards[difficulty];
            }
            catch { }

            if (rewards == null) rewards = new List<QuestRewardInfo>(); // Empty

            // Just QuestInfo's rewards
            this.QuestInfo.Rewards = rewards;

            packet.PutInt(this.Class); // Quest class Id
            packet.PutByte(difficulty);
            packet.PutString(this.Name);
            packet.PutByte(this.PartyCountMin);
            packet.PutByte(this.PartyCountMax);
            packet.PutInt(this.TimeLimit);
            packet.PutInt(this.Unknown7);
            packet.PutString(this.Description);
            packet.PutString(""); // Always blank? Might be used somewhere
            packet.PutString(this.QuestInfo.GetRewardsString());
            packet.PutInt(this.Unknown11); // These last 5 are not consts, they have meaning, but no idea what
            packet.PutInt(this.Unknown12);
            packet.PutInt(this.Unknown13);
            packet.PutInt(this.Unknown14);
            packet.PutByte(this.Special); // Almost always 0, but have seen as 3? (The Other Alchemists = 3, Their Method = 1)
        }

        public void AddMapDataToPacket(MabiPacket packet)
        {
            // Op: 0x8D6C
            // First argument is a String which echos the String sent in request, so
            // no need to handle that here

            packet.PutInt(this.MapRegion);
            for (int i = 0; i < 4; i++)
            {
                if (this.MapCrop.Length > i)
                    packet.PutShort(this.MapCrop[i]);
                else packet.PutShort(0);
            }

            packet.PutInt((uint)this.MapMarkCoords.Count);
            foreach (Tuple<uint, uint> coords in this.MapMarkCoords)
            {
                packet.PutInt(coords.Item1);
                packet.PutInt(coords.Item2);
            }
        }

        public MissionRegionInfo AddRegion(uint mapId)
        {
            var region = new MissionRegionInfo();
            region.Id = mapId;
            this.Regions.Add(region);
            return region;
        }

        /// <summary>
        /// Generate a quest for a user.
        /// </summary>
        /// <param name="creature"></param>
        /// <returns>MabiQuest, or null if an error occurred</returns>
        public MabiQuest GenerateQuestOrNull(MabiCreature creature, byte difficulty)
        {
            // Should get QuestInfo from database, added by respective SM script
            var quest = new MabiQuest(this.Class);
            if (quest == null) return null;
            quest.MissionDifficulty = difficulty;

            quest.Tags = this.GetTags((Difficulty)difficulty);
            // Older class before I found Tags:
            //quest.QMValues = this.GetQMValues((Difficulty)difficulty);

            quest.ShowExitButton = true;
            quest.Info.Unknown2 = 1;
            quest.Info.Type = 7;

            return quest;
        }

        public MabiTags GetTags(Difficulty difficulty)
        {
            var tags = new MabiTags();

            // Send with exp/gold modifiers
            double expDiffMod = 1f, gldDiffMod = 1f;
            switch (difficulty)
            {
                case Difficulty.Int:
                    {
                        expDiffMod = 1.8f;
                        gldDiffMod = 1.4f;
                    } break;
                case Difficulty.Advanced:
                    {
                        expDiffMod = 3f;
                        gldDiffMod = 2f;
                    } break;
                case Difficulty.Hard:
                    {
                        expDiffMod = 5f;
                        gldDiffMod = 2.8f; // Don't remember, fix later
                    } break;
                // Elite and Lord unknown
            }

            // Add default Shadow Mission values
            tags.SetFloat("QMBEXP", (float)expDiffMod);
            tags.SetFloat("QMBGLD", (float)gldDiffMod);
            tags.SetByte("QMMLVL", (byte)difficulty); // Difficulty
            tags.SetByte("QMEXQE", 1);
            tags.SetByte("QMMDFG", (difficulty == Difficulty.Basic ? (byte)0 : (byte)1)); // 0 if Basic, 1 if anything above (tested on Basic - Hard)
            tags.SetFloat("QMSMEXP", 1f);
            tags.SetFloat("QMSMGLD", 1f);
            tags.SetFloat("QMAMEXP", 1f);
            tags.SetFloat("QMAMGLD", 1f);
            tags.SetInt("QMBHDCTADD", 0);
            tags.SetFloat("QMGNRB", 1f);

            // TODO: Set some hook so scripts can modify quests before sent to user, to set special
            // values like experience, etc.

            // If TODAY flag set
            // values.SetFloat("QMTMEXP", 2f);

            // If VIP TODAY flag set
            // values.SetFloat("QMTMEXP", 1f)
            // .SetBool("QMVTS", false);

            return tags;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userIndex"></param>
        /// <returns>Tuple with X/Y values if found, null if not found</returns>
        public Tuple<uint, uint> GetUserSpawnCoord(uint userIndex)
        {
            Tuple<uint, uint> t = null;
            this.UserSpawnCoords.TryGetValue(userIndex, out t);
            return t;
        }

        /// <summary>
        /// Set how the map is cropped for displaying this Shadow Mission.
        /// </summary>
        /// <param name="x1">X1</param>
        /// <param name="y1">Y1</param>
        /// <param name="x2">X2</param>
        /// <param name="y2">Y2</param>
        public void SetMapCrop(ushort x1, ushort y1, ushort x2, ushort y2)
        {
            this.MapCrop[0] = x1;
            this.MapCrop[1] = y1;
            this.MapCrop[2] = x2;
            this.MapCrop[3] = y2;
        }

        /// <summary>
        /// Set the reward for this Shadow Mission.
        /// </summary>
        /// <param name="difficulty">Difficulty</param>
        /// <param name="exp">Experience amount</param>
        /// <param name="gold">Gold amount</param>
        public void SetReward(Difficulty difficulty, uint exp, uint gold)
        {
            this.SetReward((byte)difficulty, exp, gold);
        }

        /// <summary>
        /// Set the reward for this Shadow Mission.
        /// </summary>
        /// <param name="difficulty">Difficulty</param>
        /// <param name="exp">Experience amount</param>
        /// <param name="gold">Gold amount</param>
        public void SetReward(byte difficulty, uint exp, uint gold)
        {
            //this.Rewards[difficulty] = new ShadowMissionReward(exp, gold);

            // Add reward if it doesn't exist
            if (!this.Rewards.ContainsKey(difficulty))
                this.Rewards.Add(difficulty, new List<QuestRewardInfo>());

            var r = this.Rewards[difficulty];

            var expReward = new QuestRewardInfo();
            expReward.Type = RewardType.Exp;
            expReward.Amount = exp;

            var goldReward = new QuestRewardInfo();
            goldReward.Type = RewardType.Gold;
            goldReward.Amount = gold;

            r.Add(expReward);
            r.Add(goldReward);
        }

        public void SetSpawn(uint index, uint x, uint y)
        {
            // Make this better later...

            if (this.UserSpawnCoords.ContainsKey(index))
                return;

            this.UserSpawnCoords.Add(index, new Tuple<uint, uint>(x, y));
        }

        public void SupportDifficulty(Difficulty d)
        {
            this.SupportDifficulty((byte)d);
        }

        public void SupportDifficulty(byte d)
        {
            if (!this.Difficulties.Contains(d))
                this.Difficulties.Add(d);
        }

        public void SupportDifficulties(params Difficulty[] d)
        {
            for (int i = 0; i < d.Length; i++) this.SupportDifficulty((byte)d[i]);
        }

        public void SupportDifficulties(params byte[] d)
        {
            for (int i = 0; i < d.Length; i++) this.SupportDifficulty(d[i]);
        }

        /// <summary>
        /// Whether or not this Shadow Mission can be started from
        /// a certain prop.
        /// </summary>
        /// <param name="propId">Prop Id</param>
        /// <returns>true if prop supported, false if not</returns>
        public bool SupportsProp(ulong propId)
        {
            return this.Triggers.Contains(propId);
        }

        public bool SupportsDifficulty(Difficulty d)
        {
            return this.SupportsDifficulty((byte)d);
        }

        public bool SupportsDifficulty(byte d)
        {
            return (this.Difficulties.Contains(d));
        }
    }

    /// <summary>
    /// Class to store information on a region in a mission.
    /// </summary>
    public class MissionRegionInfo
    {
        public uint Id = 0; // Map Id, 300 = Tail, etc
        public string VariationFile = "";
        public string DirectoryName = "";

        public uint Unknown1 = 0;
        public byte Unknown2 = 0;

        /// <summary>
        /// Unknown flag, usually 0x80000001, seen as 0x80000003 in a theatre mission.
        /// </summary>
        public uint UnknownFlag = 0x80000001;

        /// <summary>
        /// Get the path of the default variation file.
        /// </summary>
        /// <returns>Path of the default variation file</returns>
        public string GetDefaultVariationFile()
        {
            if (this.DirectoryName == null || this.DirectoryName.Equals(""))
                return "";
            return String.Format("data/world/{0}/region_variation_1.xml", this.DirectoryName);
        }
    }

    // This is the actual Shadow Mission instance

    /// <summary>
    /// Mission instance, used for instances of shadow / theatre missions.
    /// </summary>
    public class MabiMission : IDisposable
    {
        public delegate void Callback(MabiMission mission);

        public MissionInfo MissionInfo = null;
        //public uint RegionId = 0; // Of region instance
        public bool Completed = false;

        // Map player Ids to indices
        private Dictionary<ulong, uint> _playerIndices = new Dictionary<ulong, uint>();
        private Object _indicesLock = new Object();

        /// <summary>
        /// All region instances associated with this Shadow Mission.
        /// Make some RegionContainer parent class later (Dungeons will have Regions too)
        /// </summary>
        //public Dictionary<uint, Region> Regions = new Dictionary<uint, Region>();
        public MissionRegion[] Regions = new MissionRegion[0];

        private uint _startingRegionId = 0;

        private byte _difficulty = (byte)(Difficulty.Basic);

        /// <summary>
        /// The prop Id that triggered this mission instance, or 0 if none.
        /// </summary>
        private ulong _propId = 0;

        /// <summary>
        /// Things to dispose of upon disposing of the mission instance.
        /// </summary>
        private List<IDisposable> _disposables = new List<IDisposable>();

        // Only includes players, one per client, no pets
        protected SortedDictionary<ulong, MabiCreature> _players = new SortedDictionary<ulong, MabiCreature>();
        protected Object _playersLock = new Object();

        public uint PlayerCount
        {
            get { return (_players != null ? (uint)_players.Count : 0); }
        }

        public void AddPlayer(MabiCreature c)
        {
            lock (_playersLock)
            {
                _players.Add(c.Id, c);
            }
        }

        public MabiCreature GetPlayer(ulong creatureId)
        {
            MabiCreature c = null;
            lock (_playersLock)
            {
                _players.TryGetValue(creatureId, out c);
            }
            return c;
        }

        public bool RemovePlayer(ulong creatureId)
        {
            bool r = false;
            lock (_playersLock)
            {
                r = _players.Remove(creatureId);
            }
            return r;
        }

        /// <summary>
        /// Get the starting region instance that players spawn in when entering the mission.
        /// </summary>
        public MissionRegion StartingRegion
        {
            get
            {
                return this.Regions[0];
            }
        }

        private SortedDictionary<ulong, MissionStatus> _statuses
            = new SortedDictionary<ulong, MissionStatus>();

        public MabiMission(uint regionId, MissionInfo info, Difficulty difficulty, ulong propId = 0)
            : this(new uint[] { regionId }, info, (byte)difficulty, propId)
        {
        }

        public MabiMission(uint regionId, MissionInfo info, byte difficulty, ulong propId = 0)
            : this(new uint[] { regionId }, info, difficulty, propId)
        {
        }

        public MabiMission(uint[] regionIds, MissionInfo info, Difficulty difficulty, ulong propId = 0)
            : this(regionIds, info, (byte)difficulty, propId)
        {
        }

        public MabiMission(uint[] regionIds, MissionInfo info, byte difficulty, ulong propId = 0)
        {
            if (regionIds.Length != info.Regions.Count)
                throw new Exception("Region counts are not equal");

            if (regionIds.Length == 0)
                throw new Exception("ShadowMission trying to be instantiated, but has no regions");

            if (!info.SupportsDifficulty(difficulty))
                throw new Exception("Cannot start a Shadow Mission with an unsupported difficulty");

            this.MissionInfo = info;
            _startingRegionId = regionIds[0];
            _difficulty = difficulty;
            _propId = propId;

            this.InitRegions(regionIds);

            // Trigger script-defined callback
            if (this.MissionInfo.OnMissionStart != null)
                this.MissionInfo.OnMissionStart(this);
        }

        /// <summary>
        /// Add objects to dispose of upon disposing the mission.
        /// </summary>
        /// <param name="stuff"></param>
        public void AddDisposable(params IDisposable[] stuff)
        {
            lock (_disposables)
                foreach (var d in stuff)
                    if (d != null)
                        _disposables.Add(d);
        }

        public void Complete(bool receiveRewards = false)
        {
            lock (_playersLock)
            {
                // Assuming only players..
                foreach (var pair in _players)
                {
                    if (!(pair.Value is MabiPC)) continue;
                    var player = pair.Value as MabiPC;

                    var quest = player.GetShadowMissionQuestOrNull();
                    if (quest == null) continue;

                    WorldManager.Instance.CreatureCompletesQuest(player, quest, receiveRewards);
                }
            }
        }

        public void SetPlayerStatus(ulong playerId, MissionStatus status)
        {
            lock (_statuses)
            {
                if (_statuses.ContainsKey(playerId))
                    _statuses[playerId] = status;
                else _statuses.Add(playerId, status);
            }
        }

        public MissionStatus GetPlayerStatus(MabiPC player)
        {
            return this.GetPlayerStatus(player.Id);
        }

        public MissionStatus GetPlayerStatus(ulong playerId)
        {
            MissionStatus status = MissionStatus.None;
            lock (_statuses)
                _statuses.TryGetValue(playerId, out status);
            return status;
        }

        public virtual void Dispose()
        {
            lock (_disposables)
                foreach (var d in _disposables)
                    d.Dispose();

        }

        /// <summary>
        /// Initialize regions. MissionInfo.Regions should have the same number of elements
        /// as regionIds. This should have been checked in constructor already.
        /// </summary>
        /// <param name="regionIds">Map of region indices (as in ShadowMissionInfo) to region Ids</param>
        private void InitRegions(uint[] regionIds)
        {
            this.Regions = new MissionRegion[regionIds.Length];

            uint count = 0;
            foreach (var region in this.MissionInfo.Regions)
            {
                this.Regions[count] = new MissionRegion(regionIds[count], region);
                count++;
            }
        }

        public void AddToPacket(MabiPacket packet, uint playerIndex, bool isPet = false)
        {
            var coords = this.MissionInfo.GetUserSpawnCoord(playerIndex);

            // Might not be the best course of action..
            if (coords == null)
                // Try using default (player index 0) coords
                if ((coords = this.MissionInfo.GetUserSpawnCoord(0)) == null)
                    // If even those aren't set, just use 0,0...
                    coords = new Tuple<uint, uint>(0, 0);

            uint spawnX = coords.Item1, spawnY = coords.Item2;

            // Set pet spawn offset, gotten from Shadow Cast City
            // Might want to not use offset later, SetPetSpawn(1, X, Y) or something
            if (isPet)
            {
                spawnX -= 163;
                spawnY -= 156;
            }

            //var regionsInfo = ;

            //packet.PutInt(this.RegionUnknown1); // 0, 300, ?
            packet.PutInt(this.MissionInfo.MapRegion); // For now just put MapRegion, but sends 0 sometimes as well?

            packet.PutInt(this.StartingRegion.Id); // Region to start on.. set to Regions[0]
            packet.PutInt(spawnX); // X Spawn of user to send to..
            packet.PutInt(spawnY); // Y spawn of user to send to..
            packet.PutInt(0); // Const?
            packet.PutInt((uint)this.Regions.Length); // Number of regions

            foreach (var region in this.Regions)
            {
                packet.PutInt(region.Id);
                packet.PutString(region.ToString());
                packet.PutInt(region.Data.UnknownFlag); // Old const: 0x80000001
                packet.PutInt(region.Data.Id); // Map Id of region
                packet.PutString(region.Data.DirectoryName);
                packet.PutInt(region.Data.Unknown1); // These two might have to do with variation file/client stuff (maybe bitflag)
                packet.PutByte(region.Data.Unknown2);

                string varFile = region.Data.VariationFile;
                if (varFile == null || varFile.Equals(""))
                    varFile = region.Data.GetDefaultVariationFile(); // Use default variation file

                packet.PutString(varFile);
            }
        }

        public void AddPlayerIndex(ulong creatureId, uint index)
        {
            lock (_indicesLock)
            {
                try { _playerIndices.Add(creatureId, index); }
                catch { }
            }
        }

        public uint GetPlayerIndex(ulong creatureId)
        {
            uint r = uint.MaxValue;
            lock (_indicesLock)
            {
                _playerIndices.TryGetValue(creatureId, out r);
            }
            return r;
        }

        public bool RemovePlayerIndex(ulong creatureId)
        {
            bool r = false;
            lock (_indicesLock)
            {
                r = _playerIndices.Remove(creatureId);
            }
            return r;
        }

        /// <summary>
        /// Have a user/creature enter the shadow mission.
        /// </summary>
        /// <param name="creature"></param>
        public void Enter(MabiPC player, uint playerIndex)
        {
            // TODO: Send to any pets out as well? Unsure if needed.

            // User
            if (!(player.Client is WorldClient)) return;
            var client = player.Client as WorldClient;

            // Send AreaChange
            // Not sure how this acts when no prop was used to trigger mission
            if (_propId > 0)
                client.Send(new MabiPacket(Op.AreaChange, _propId)
                        .PutLong(player.Id)
                        .PutInt(202) // ?
                        .PutString(""));

            // TODO: Send a EntityDisappears
            Send.EntityDisappears(client, player);

            // TODO: Send a EntitiesDisappear
            if (client.Creatures.Count > 1)
                Send.EntitiesDisappear(client, client.Creatures.Where(c => c != client.Character));

            // Send a CharacterLock
            Send.CharacterLock(client);

            // Also send a CharacterLock for the pet that's out, if there is one
            var pet = player.Pet;
            if (pet != null) // If pet is out..?
                Send.CharacterLock((WorldClient)pet.Client);

            // If SM, would check for relative quest
            // If TM, checks for scroll item..
            // Maybe make a CanEnter func..

            // Should perform this quest earlier
            var quest = player.GetShadowMissionQuestOrNull();
            if (quest == null) throw new Exception(); // Or something..

            // TODO: Send an ItemRemove for Quest "item", ex. 005000CBBE5B6FAD
            client.Send(new MabiPacket(Op.ItemRemove, player.Id)
                        .PutLong(quest.ItemId)
                        .PutByte(0x17)); // Const? Might need to be changed later

            // TODO: Send a QuestUpdate { Quest Id, 0x01, 0x00, 0x00000000, 0x00, 0x00 }
            var questUpdate = new MabiPacket(Op.QuestUpdate, player.Id)
                .PutLong(quest.Id)
                .PutByte(1) // Can quit this mission
                .PutByte(0)
                .PutInt(0)
                .PutByte(0)
                .PutByte(0);
            client.Send(questUpdate);

            // Some party quest packet?
            if (quest != null)
            {
                var unkPacket = new MabiPacket(0x8EB4, player.Id); // Might not be creature.Id
                unkPacket.PutLong(quest.Id); // This might be some party quest Id, ex. 006000CBBE5B6FAD
                unkPacket.PutByte(0); // Const?
                client.Send(unkPacket);
            }

            // TODO: Send a QuestClear (0x8CA1) with same Id as above
            client.Send(new MabiPacket(Op.QuestClear, player.Id)
                        .PutLong(quest.Id));

            // Manual remove for now
            player.Quests.Remove(quest.Info.Id);

            // Send region data
            var regionPacket = new MabiPacket(Op.ShadowMissionRegionR, Id.Broadcast);
            regionPacket.PutLong(player.Id); // Might not be creature.Id
            this.AddToPacket(regionPacket, playerIndex);
            client.Send(regionPacket);

            // Add player to indices dictionary, will use for now as "proof" that they
            // are a part of this SM..
            this.AddPlayerIndex(player.Id, playerIndex);
            
            //client.ShadowMission = this;
            //client.ShadowMissionStatus = ShadowMissionStatus.Entering;
            MissionManager.Instance.AddPlayerToMission(player.Id, this);

            // This should be it, after server receives a "EnterRegion" from client,
            // it unlocks character, then spawns entities and props, and "reassigns"
            // new SM quest, which will be QuestClear'd upon exiting.
            // Upon getting "EnterRegion" from client, should set MabiCreature's 
            // Shadow Mission field.
            // See AfterEnter()

            this.SetPlayerStatus(player.Id, MissionStatus.Entering);
        }

        /// <summary>
        /// Called after a creature was sent data from Enter(), sends them additional data
        /// and unlocks them. Specifically called after EnterRegion request received.
        /// </summary>
        /// <param name="creature"></param>
        public void AfterEnter(MabiPC player)
        {
            if (!(player.Client is WorldClient)) return;
            var client = player.Client as WorldClient;

            uint playerIndex = this.GetPlayerIndex(player.Id);

            // Not in indices dictionary, might be bug or forged request
            if (playerIndex == uint.MaxValue)
                return;

            Tuple<uint, uint> spawnCoords = this.MissionInfo.GetUserSpawnCoord(playerIndex);
            if (spawnCoords == null) return; // Handle this better?

            // Make sure server knows new position
            player.SetLocation(this.StartingRegion.Id, spawnCoords.Item1, spawnCoords.Item2);

            //client.SendUnlock(player);

            // TODO: Send a EntitiesAppear
            // Send entities in range, copied from EnterRegion handler for now
            // This also sends props, usually.. ? Doesn't yet
            var entities = WorldManager.Instance.GetEntitiesInRange(player);
            if (entities.Count > 0)
            {
                entities.Remove(player);
                entities.Add(player); // Make sure player is at end?
                Send.EntitiesAppear(client, entities);
            }

            // Send all PropAppears (props)
            // Send a QuestNew for SM quest, will have new Id
            //var quest = new MabiQuest(this.MissionInfo.Class);
            var quest = this.MissionInfo.GenerateQuestOrNull(player, _difficulty);
            player.Quests[quest.Class] = quest;

            Send.QuestNew(player, quest);
            //var questNewPacket = new MabiPacket(Op.QuestNew, player.Id);
            //quest.AddToPacket(questNewPacket);
            //client.Send(questNewPacket);

            // Send a WarpRegion with Id, { (byte)1, RegionUnknown2, RegionSpawnX, RegionSpawnY }
            // client.Warp(this.MissionInfo.Region, spawnCoords.Item1, spawnCoords.Item2);
            client.Send(new MabiPacket(Op.WarpRegion, player.Id)
                        .PutByte(1)
                        .PutInt(this.StartingRegion.Id)
                        .PutInt(spawnCoords.Item1)
                        .PutInt(spawnCoords.Item2));

            // Send a 0xA925 { RegionUnknown2, (int)0 }
            // This is actually already sent by Aura as a response to EnterRegion
            var unkPacket = new MabiPacket(0xA925, Id.Broadcast);
            unkPacket.PutInt(this.StartingRegion.Id);
            unkPacket.PutInt(0);
            client.Send(unkPacket);



            // Send an 0xA97F.. 10 seconds after region info sent? (Id: Broadcast)
            // Format: { (uint) # of regions, (uint)regionId1, (uint)regionId2, ... }

            //client.Send(new MabiPacket(0xA97F, Id.Broadcast)
            //            .PutInt(1)
            //            .PutInt(this.RegionId));

            // If the player had a pet spawned when entering, send region data
            // to pet
            //var player = this.GetPlayer(creature.Id);
            //var pet = player.Pet;
            //if(pet != null)
            //{
            //    var regionPacket = new MabiPacket(Op.ShadowMissionRegionR, Id.Broadcast);
            //    regionPacket.PutLong(pet.Id);
            //    this.AddToPacket(regionPacket, playerIndex, true);
            //    client.Send(regionPacket);
            //}

            // These are the result of 0x90A7 requests from client, sends 2
            // response packets (0x52D3, 0x90A8) per request
            // The 0x90A7 requests contain 1 arg, monster Id (long)
            // for(int i = 0; i < CountOf90A7; i++) {
            //      Send a 0x52D3, prop related?
            //      Send a 0x90A8, just verify that 0x90A7 was handled?
            // }

            // Client also sends a 0xA90B request around now, server responds
            // with 0xA90C, seems to contain data/positions of next wave of monsters?
            
            this.SetPlayerStatus(player.Id, MissionStatus.In);
        }

        public void Exit(MabiPC player)
        {
            if (!(player.Client is WorldClient)) return;
            var client = player.Client as WorldClient;

            if (!this.RemovePlayerIndex(client.Character.Id)) // This is thread safe
            {
                // They were recently removed from SM, might be request spamming
                return;
            }

            this.RemovePlayer(client.Character.Id);

            // [#TEMPORARY]
            //client.ClearShadowMission();
            //client.ShadowMissionStatus = ShadowMissionStatus.None;
            MissionManager.Instance.RemovePlayerFromMission(player.Id);

            // Remove SM quest
            var quest = player.GetShadowMissionQuestOrNull();
            if (quest != null)
            {
                player.Quests.Remove(quest.Class);
                WorldManager.Instance.CreatureCompletesQuest(player, quest, false); // Trying this..
            }

            // Can use normal warp here
            client.Warp(this.MissionInfo.ExitRegion, this.MissionInfo.ExitSpawnX, this.MissionInfo.ExitSpawnY);
            client.Send(new MabiPacket(Op.ShadowMissionExitR, client.Character.Id).PutByte(1)); // Success
        }

        /// <summary>
        /// Close the Shadow Mission instance, called after all users have (left region/received reward/completed mission).
        /// Not sure when the best time to close it is yet, need more research.
        /// </summary>
        public void Close()
        {
            // TODO
        }

        public List<MabiProp> GetProps()
        {
            // Get props in temp regions

            // This could be more efficient..
            List<MabiProp> props = new List<MabiProp>();
            foreach (var region in this.Regions)
            {
                props.AddRange(WorldManager.Instance.GetPropsInRegion(region.Id));
            }
            return props;
        }

        
    }

    /// <summary>
    /// Instance of a region
    /// </summary>
    public class MissionRegion
    {
        public uint Id = 0; // Id from pool
        public MissionRegionInfo Data = null;

        public MissionRegion(uint regionId, MissionRegionInfo data)
        {
            this.Id = regionId;
            this.Data = data;
        }

        public override String ToString()
        {
            return String.Format("DynamicRegion{0}", this.Id);
        }
    }

    // These are going to need to be dynamically altered somehow..
    // Will need thread safety
    public enum ShadowMissionSpecial { None = 0, Today, VIPToday = 3 }

    public enum MissionStatus { None, Entering, In, Leaving }

    // Might nameclash?
    public enum Difficulty : byte
    {
        Basic = 0,
        Int,
        Advanced,
        Hard,
        Elite,
        Lord
    }
}