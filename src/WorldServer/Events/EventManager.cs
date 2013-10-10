// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see licence.txt in the main folder

using System;
using System.Linq;
using Aura.Shared.Util;
using Aura.World.Player;
using Aura.World.World;

namespace Aura.World.Events
{
	/// <summary>
	/// The server raises these events.
	/// </summary>
	public static class EventManager
	{
		/// <summary>
		/// Contains all events dealing with creatures
		/// </summary>
		public static class CreatureEvents
		{
			/// <summary>
			/// Fired when a creature starts to walk/run.
			/// </summary>
			public static event MoveEventHandler CreatureMoves;
			/// <summary>
			/// Fired when a creature got killed.
			/// </summary>
			public static event KillEventHandler CreatureKilled;
			/// <summary>
			/// Fired on a creature's level up.
			/// </summary>
			public static event CreatureEventHandler CreatureLevelsUp;
			/// <summary>
			/// Fired when the rank of a skill of a creature changes.
			/// </summary>
			public static event SkillChangeEventHandler CreatureSkillChange;
			/// <summary>
			/// Fired when a creature drops an item.
			/// </summary>
			public static event ItemEventHandler CreatureDropItem;
			/// <summary>
			/// Fired when a creature gets or loses an item.
			/// </summary>
			public static event ItemClassEventHandler CreatureItemAction;

			public static void OnCreatureKilled(MabiCreature victim, MabiCreature killer)
			{
				if (CreatureKilled != null)
					CreatureKilled(victim, killer);
			}

			public static void OnCreatureMoves(MabiCreature creature, MabiVertex from, MabiVertex to)
			{
				if (CreatureMoves != null)
					CreatureMoves(creature, from, to);
			}

			public static void OnCreatureLevelsUp(MabiCreature creature)
			{
				if (CreatureLevelsUp != null)
					CreatureLevelsUp(creature);
			}

			public static void OnCreatureSkillChange(MabiCreature creature, MabiSkill skill, bool isNew)
			{
				if (CreatureSkillChange != null)
					CreatureSkillChange(creature, skill, isNew);
			}

			public static void OnCreatureDropItem(MabiCreature creature, MabiItem item)
			{
				if (CreatureDropItem != null)
					CreatureDropItem(creature, item);
			}

			public static void OnCreatureItemAction(MabiCreature creature, uint cls)
			{
				if (CreatureItemAction != null)
					CreatureItemAction(creature, cls);
			}
		}

		/// <summary>
		/// Contains all events dealing with entities
		/// </summary>
		public static class EntityEvents
		{
			public static event EntityEventHandler EntityEntersRegion;
			public static event EntityEventHandler EntityLeavesRegion;

			public static void OnEntityEntersRegion(MabiEntity entity)
			{
				if (EntityEntersRegion != null)
					EntityEntersRegion(entity);
			}

			public static void OnEntityLeavesRegion(MabiEntity entity)
			{
				if (EntityLeavesRegion != null)
					EntityLeavesRegion(entity);
			}
		}

		/// <summary>
		/// Contains player-only events
		/// </summary>
		public static class PlayerEvents
		{
			/// <summary>
			/// Fired a few seconds after the player logged in successfully.
			/// </summary>
			public static event PlayerEventHandler PlayerLoggedIn;
            /// <summary>
            /// Fired after a player logs off, but before anything saved to the db.
            /// Also fired after a player is disconnected unexpectedly.
            /// </summary>
            public static event PlayerEventHandler PlayerLoggedOff;
			/// <summary>
			/// Fired when a player is changing regions (EnterRegion).
			/// </summary>
			public static event PlayerEventHandler PlayerChangesRegion;
			/// <summary>
			/// Fired when some creature got killed by a player. (Obsolete?)
			/// </summary>
			public static event KillEventHandler KilledByPlayer;
			/// <summary>
			/// Fired when a player talks in public chat.
			/// </summary>
			public static event ChatEventHandler PlayerTalks;

			public static void OnPlayerLoggedIn(MabiPC character)
			{
				if (PlayerLoggedIn != null)
					PlayerLoggedIn(character);
			}

            public static void OnPlayerLoggedOff(MabiPC character)
            {
                if (PlayerLoggedOff != null)
                    PlayerLoggedOff(character);
            }

			public static void OnPlayerChangesRegion(MabiPC character)
			{
				if (PlayerChangesRegion != null)
					PlayerChangesRegion(character);
			}

			public static void OnKilledByPlayer(MabiCreature victim, MabiCreature killer)
			{
				if (KilledByPlayer != null)
					KilledByPlayer(victim, killer);
			}

			public static void OnPlayerTalks(MabiCreature creature, string message)
			{
				if (PlayerTalks != null)
					PlayerTalks(creature, message);
			}
		}

		/// <summary>
		/// Contains all events dealing with time
		/// </summary>
		public static class TimeEvents
		{
			/// <summary>
			/// Raised every minute (erinn time) (1.5s real time).
			/// </summary>
			public static event TimeEventHandler ErinnTimeTick;
			/// <summary>
			/// Raised every minute (real time).
			/// </summary>
			public static event TimeEventHandler RealTimeTick;
			/// <summary>
			/// Raised every second (real time).
			/// </summary>
			public static event TimeEventHandler RealTimeSecondTick;
			/// <summary>
			/// Raised at at 6:00am and 6:00pm (erinn time) (every 18 minutes real time).
			/// </summary>
			public static event TimeEventHandler ErinnDaytimeTick;
			/// <summary>
			/// Raised at 0:00am (erinn time).
			/// </summary>
			public static event TimeEventHandler ErinnMidnightTick;

			public static void OnErinnTimeTick(MabiTime time)
			{
				try
				{
					if (ErinnTimeTick != null)
						ErinnTimeTick(time);
				}
				catch (Exception ex)
				{
					Logger.Exception(ex, "In OnErinnTimeTick: " + ex.Message, true);
				}
			}

			public static void OnErinnDaytimeTick(MabiTime time)
			{
				if (ErinnDaytimeTick == null)
					return;

				// Iterate through the handlers to be able to tell which one errored,
				// and let the ones after that on be called.
				foreach (var handler in ErinnDaytimeTick.GetInvocationList().Cast<TimeEventHandler>())
				{
					try { handler(time); }
					catch (Exception ex) { Logger.Error("Source: {0}.{1}, Error: {2}", handler.Target, handler.Method.Name, ex.Message); }
				}
			}

			public static void OnErinnMidnightTick(MabiTime time)
			{
				if (ErinnMidnightTick != null)
					ErinnMidnightTick(time);
			}

			public static void OnRealTimeTick(MabiTime time)
			{
				if (RealTimeTick != null)
					RealTimeTick(time);
			}

			public static void OnRealTimeSecondTick(MabiTime time)
			{
				if (RealTimeSecondTick != null)
					RealTimeSecondTick(time);
			}
		}
	}
}
