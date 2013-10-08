// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see licence.txt in the main folder

using System.Collections;
using System.Collections.Specialized;
using Aura.Data;
using Aura.Shared.Network;
using Aura.Shared.Util;

namespace Aura.World.World
{
	public class MabiQuestProgress
	{
		public string Objective;
		public uint Count;
		public bool Done;
		public bool Unlocked;

		public MabiQuestProgress(string objective, bool unlockedByDefault)
		{
			this.Objective = objective;
			this.Unlocked = unlockedByDefault;
		}

		public MabiQuestProgress(string objective, uint count, bool done, bool unlocked)
		{
			this.Objective = objective;
			this.Count = count;
			this.Done = done;
			this.Unlocked = unlocked;
		}
	}

	public class MabiQuest
	{
		private static ulong _questIds = Aura.Shared.Const.Id.QuestsTmp;

		public ulong Id;
		public uint Class;
		public MabiQuestState State;

		public OrderedDictionary Progresses = new OrderedDictionary();

        // Key:Type:Value set sent with QuestNew
        public MabiTags Tags = null;

        // Shadow Mission specific
        public byte MissionDifficulty = (byte)Difficulty.Basic;
        public bool ShowExitButton = false; // Sent as a byte in QuestNew for some reason..

		/// <summary>
		/// Every quest has an invisible item that goes with it.
		/// The id is always quest id - 0x0010000000000000.
		/// </summary>
		public ulong ItemId { get { return this.Id - Aura.Shared.Const.Id.QuestItemOffset; } }

		// Abstract way of accessing the info, but an easy and clean one.
		// Automatic querying of QuestDb with warning msg and caching.
		private QuestInfo _info;
		public QuestInfo Info
		{
			get
			{
				if (_info != null)
					return _info;

				var qi = MabiData.QuestDb.Find(this.Class);
				if (qi == null)
				{
					Logger.Warning("Unknown quest id '{0}'.", this.Class);
					return null;
				}

				return (_info = qi);
			}
		}

		/// <summary>
		/// Returns true if all objectives are done.
		/// </summary>
		public bool IsDone
		{
			get
			{
				int n = 0;
				for (int i = 0; i < this.Progresses.Count; ++i)
				{
					if ((this.Progresses[i] as MabiQuestProgress).Done)
						n++;
				}
				return (n >= this.Progresses.Count);
			}
		}

		/// <summary>
		/// Used in quest items, although seemingly not required.
		/// </summary>
		public string ToolTip
		{
			get
			{
				return string.Format("QSTTIP:s:N_{0}|D_{1}|A_|R_{2}|T_0;", this.Info.Name, this.Info.Description, string.Join(", ", this.Info.Rewards));
			}
		}

		private MabiItem _questitem;
		public MabiItem QuestItem
		{
			get
			{
				if (_questitem != null)
					return _questitem;
				return (_questitem = new MabiItem(this));
			}
		}

		public MabiQuest(uint cls, ulong id = 0)
		{
			this.Class = cls;
			this.Id = (id == 0 ? _questIds++ : id);

			// Loading quest from db, progress comes from there as well.
			if (id > 0)
				return;

			if (this.Info == null)
				return;

			// Fill progress list based on the objectives of this quest.
			foreach (DictionaryEntry de in this.Info.Objectives)
			{
				var key = de.Key as string;
				var val = de.Value as QuestObjectiveInfo;
				this.Progresses.Add(key, new MabiQuestProgress(key, val.Unlocked));
			}
		}

		public MabiQuestProgress CurrentProgress
		{
			get
			{
				// Go through objective progress
				foreach (MabiQuestProgress p in this.Progresses.Values)
				{
					// First not done objective is the current one.
					if (!p.Done)
						return p;
				}

				// Quest done?
				return null;
			}
		}

		public void SetObjectiveDone(string objective)
		{
			for (int i = 0; i < this.Progresses.Count; ++i)
			{
				var p = this.Progresses[i] as MabiQuestProgress;
				if (p.Objective == objective)
				{
					p.Done = true;
					p.Unlocked = false;
					//p.Value.Count = (this.Info.Objectives[p.Key] as QuestObjectiveInfo).Amount;

					// Unlock the next objective.
					if (i + 1 < this.Progresses.Count)
					{
						p = this.Progresses[i + 1] as MabiQuestProgress;
						p.Unlocked = true;
					}
				}
			}
		}

		public void SetObjectiveUndone(string objective)
		{
			for (int i = 0; i < this.Progresses.Count; ++i)
			{
				var p = this.Progresses[i] as MabiQuestProgress;
				if (p.Objective == objective)
				{
					p.Done = false;
					p.Unlocked = true;
					return;
				}
			}
		}
	}

	public enum MabiQuestState : byte { Active, Complete }
}
