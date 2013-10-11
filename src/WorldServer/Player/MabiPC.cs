// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see licence.txt in the main folder

using System;
using System.Collections.Generic;
using System.Linq;
using Aura.Data;
using Aura.Shared.Network;
using Aura.Shared.Util;
using Aura.World.Events;
using Aura.World.World;
using Aura.Shared.Const;

namespace Aura.World.Player
{
	public class MabiPC : MabiCreature
	{
		public string Server;

		public ushort RebirthCount;
		public string SpouseName;
		public ulong SpouseId;
		public uint MarriageTime;
		public ushort MarriageCount;

		public DateTime CreationTime = DateTime.Now;
		public DateTime LastRebirth = DateTime.Now;

		public bool Save = false;

		public List<ushort> Keywords = new List<ushort>();
		public Dictionary<ushort, bool> Titles = new Dictionary<ushort, bool>();
		public List<ShamalaTransformation> Shamalas = new List<ShamalaTransformation>();

		public Dictionary<uint, MabiQuest> Quests = new Dictionary<uint, MabiQuest>();

		public override EntityType EntityType
		{
			get { return EntityType.Character; }
		}

		public override float CombatPower
		{
			get
			{
				float result = 0;

				result += this.LifeMaxBase;
				result += this.ManaMaxBase * 0.5f;
				result += this.StaminaMaxBase * 0.5f;
				result += this.StrBase;
				result += this.IntBase * 0.2f;
				result += this.DexBase * 0.1f;
				result += this.WillBase * 0.5f;
				result += this.LuckBase * 0.1f;

				return result;
			}
		}

		public override bool IsAttackableBy(MabiCreature other)
		{
			if (other is MabiNPC)
				return (other.State & CreatureStates.GoodNpc) == 0;
			else
			{
				var res = false;

				if (this.EvGEnabled && other.EvGEnabled)
					if (this.EvGSupportRace != 0 && other.EvGSupportRace != 0 && other.EvGSupportRace != this.EvGSupportRace)
						res = true;

				if (this.ArenaPvPManager != null && this.ArenaPvPManager == other.ArenaPvPManager)
					if (this.ArenaPvPManager.IsAttackableBy(this, other))
						res = true;

				return res; // For now... add more PvP later and other stuff
			}
		}

		public void GiveTitle(ushort title, bool usable = false)
		{
			if (this.Titles.ContainsKey(title))
				this.Titles[title] = usable;
			else
				this.Titles.Add(title, usable);

			if (usable)
			{
				this.Client.Send(new MabiPacket(Op.AddTitle, this.Id).PutShort(title).PutInt(0));
			}
			else
			{
				this.Client.Send(new MabiPacket(Op.AddTitleKnowledge, this.Id).PutShort(title).PutInt(0));
			}
		}

		public MabiQuest GetQuestOrNull(uint cls)
		{
			MabiQuest result = null;
			this.Quests.TryGetValue(cls, out result);
			return result;
		}

		public MabiQuest GetQuestOrNull(ulong id)
		{
			return this.Quests.Values.FirstOrDefault(a => a.Id == id);
		}

        /// <summary>
        /// Get all quests whose class Id lies within a certain range.
        /// Range is [inclusive, exclusive)
        /// </summary>
        /// <param name="clsStart">Range start</param>
        /// <param name="clsEnd">Range end</param>
        /// <param name="maxCount">Max number of quests to find</param>
        /// <returns></returns>
        public MabiQuest[] GetQuests(uint clsStart, uint clsEnd, uint maxCount = 0)
        {
            uint start = 0, end = 0;

            if (clsStart == clsEnd)
            {
                var quest = this.GetQuestOrNull(clsStart);
                if (quest == null) return new MabiQuest[0];
                else return new MabiQuest[] { quest };
            }
            else if (clsStart < clsEnd) { start = clsStart; end = clsEnd; }
            else { start = clsEnd; end = clsStart; }

            var quests = new List<MabiQuest>();

            // Might be a more efficient way, like get iterator starting at clsStart
            foreach (var kvp in this.Quests)
            {
                // No longer interested
                if (kvp.Key >= end) break;

                else if (kvp.Key >= start && kvp.Value != null)
                {
                    quests.Add(kvp.Value);

                    // Check if we hit max
                    if (maxCount > 0 && maxCount <= quests.Count)
                        break;
                }
            }

            return quests.ToArray();
        }

        public MabiQuest GetShadowMissionQuestOrNull()
        {
            // Much better idea, just check Info.Type
            return this.Quests.Values.FirstOrDefault(q => q.Info.Type == 7);

            //var quests = this.GetQuests(700000, 702000, 1); // Make this class range flexible?
            //if (quests.Length == 0) return null;
            //else return quests[0];
        }
	}
}
