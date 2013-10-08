// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see licence.txt in the main folder

using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Aura.Data
{
	public class QuestInfo
	{
		public uint Id = 0;
		public string Name = "Untitled";
		public string Description = "";
		public string AdditionalInfo = "";
		public bool Cancelable = false;

		public OrderedDictionary Objectives = new OrderedDictionary();
		public List<QuestRewardInfo> Rewards = new List<QuestRewardInfo>();

        public byte Type = 2;

        public uint Unknown1 = 70024; // 70501 const for missions?
        public byte Unknown2 = 0; // 1 for blue icon
        public String Unknown3 = ""; // <xml soundset="4" npc="GUI_NPCportrait_Lanier"/>

        // These apply only to SMs it seems
        public byte PartyCountMin = 0, PartyCountMax = 0;
        public uint TimeLimit = 0; // Milliseconds
        public String TooltipDescription = "";

        public String GetRewardsString()
        {
            String s = "";
            bool first = true;
            foreach (QuestRewardInfo r in this.Rewards)
            {
                if (!first) s += '\x0A'; // Newline
                s += String.Format("* {0}", r.ToString());
                first = false;
            }
            return s;
        }
	}

	public class QuestObjectiveInfo
	{
		public ObjectiveType Type;
		public string Description = "";
		public bool Unlocked;

		public uint Id;
		public uint Amount = 1;
		public string Target;
		public readonly List<uint> Races = new List<uint>();

		public uint Region, X, Y;

		public override string ToString()
		{
			switch (this.Type)
			{
				case ObjectiveType.Kill:
					return string.Format("TGTSID:4:{0};TARGETCOUNT:4:{1};TGTCLS:2:0;", this.Id, this.Amount);

				case ObjectiveType.Collect:
					return string.Format("TARGETITEM:4:{0};TARGETCOUNT:4:{1};QO_FLAG:4:1;", this.Id, this.Amount);

				case ObjectiveType.Talk:
					return string.Format("TARGECHAR:s:{0};TARGETCOUNT:4:1;", this.Target);

				case ObjectiveType.Deliver:
					return string.Format("TARGECHAR:s:{0};TARGETITEM:4:{0};TARGETCOUNT:4:1;", this.Target, this.Id);

				case ObjectiveType.ReachRank:
					return string.Format("TGTSKL:2:{0};TGTLVL:2:{1};TARGETCOUNT:4:1;", this.Id, this.Amount);

				case ObjectiveType.ReachLevel:
					return string.Format("TGTLVL:2:{0};TARGETCOUNT:4:1;", this.Id);

				default:
					return "";
			}
		}
	}

	public class QuestRewardInfo
	{
		public RewardType Type;
		public uint Amount;
		public uint Id;
		public byte Group;

		public override string ToString()
		{
			switch (this.Type)
			{
				case RewardType.Item:
					var ii = MabiData.ItemDb.Find(this.Id);
					if (ii == null)
						return "Unknown item";
					return string.Format("{0} {1}", this.Amount, ii.Name);

				case RewardType.Gold:
					return string.Format("{0}G", this.Amount);

				case RewardType.Exp:
					return string.Format("{0} Experience Point", this.Amount);

				case RewardType.ExplExp:
					return string.Format("Exploration EXP {0}", this.Amount);

				case RewardType.AP:
					return string.Format("{0} Ability Point", this.Amount);

				case RewardType.Skill:
					var si = MabiData.SkillDb.Find((ushort)this.Id);
					if (si == null)
						return "Unknown skill";
					return string.Format("[Skill] {0}", si.Name);

				default:
					return "Unknown reward type";
			}
		}
	}

	public enum ObjectiveType : byte { Kill = 1, Collect, Talk, Deliver, ReachRank = 9, ReachLevel = 15 }
	public enum RewardType : byte { Item = 1, Gold = 2, Exp = 3, ExplExp = 4, AP = 5, Skill = 8 /* ? */ }

	/// <summary>
	/// This class is only here for consistency and easy access,
	/// data comes from the outside.
	/// </summary>
	public class QuestDb : IDatabase
	{
		public Dictionary<uint, QuestInfo> Entries = new Dictionary<uint, QuestInfo>();

		public List<DatabaseWarningException> Warnings
		{
			get
			{
				return new List<DatabaseWarningException>();
			}
		}

		public int Count
		{
			get
			{
				return this.Entries.Count;
			}
		}

		public QuestInfo Find(uint id)
		{
			QuestInfo result = null;
			this.Entries.TryGetValue(id, out result);
			return result;
		}

		public bool Exists(uint id)
		{
			return this.Entries.ContainsKey(id);
		}

		public int Load(string path)
		{
			return this.Load(path, false);
		}

		public int Load(string path, bool clear)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			this.Entries.Clear();
		}
	}
}
