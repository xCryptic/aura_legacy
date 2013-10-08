using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aura.World.Player;
using Aura.World.Network;
using Aura.Shared.Network;
using Aura.Shared.Util;

namespace Aura.World.World
{
    public class MissionManager
    {
        public static MissionManager Instance = new MissionManager();

        // [#SM]
        /// <summary>
        /// Current Shadow Missions sorted by region instance Id.
        /// </summary>
        private SortedDictionary<uint, MabiMission> _shadowMissionInstances
            = new SortedDictionary<uint, MabiMission>();

        /// <summary>
        /// Shadow Mission boards sorted by class Id of board prop.
        /// </summary>
        private SortedDictionary<uint, MissionBoard> _shadowMissionBoards
            = new SortedDictionary<uint, MissionBoard>();

        /// <summary>
        /// Region Id pool for Shadow Missions. Might differ between Tara/Tail, or maybe even
        /// different Shadow Mission types? (Not that this should matter anyways..)
        /// </summary>
        private SmallIdPool _shadowMissionRegionPool = new SmallIdPool(35000, 60000); // (25,000 / 8) = 3,125 bytes of memory

        /// <summary>
        /// Singleton constructor
        /// </summary>
        private MissionManager() { }

        /// <summary>
        /// Add a Shadow Mission altar.
        /// </summary>
        /// <param name="propId">Prop Id of altar</param>
        public void AddShadowMissionAltar(ulong propId, uint regionId, uint x, uint y)
        {
            // No need to have dict/list for this, just set prop hook
            // TODO: Make this correct later.. using filler data for now
            MabiProp altarProp = new MabiProp(propId, regionId, x, y);

            WorldManager.Instance.SetPropBehavior(new MabiPropBehavior(altarProp, this.HandleShadowMissionAltarTouch));
        }

        /// <summary>
        /// Add a ShadowMissionBoard by prop Id? Might be different Id.
        /// </summary>
        /// <param name="boardId"></param>
        public void AddShadowMissionBoard(ulong propId, uint classId)
        {
            if (!_shadowMissionBoards.ContainsKey(classId))
            {
                _shadowMissionBoards.Add(classId, new MissionBoard()); // May change constructor later
                // this.SetPropBehavior(new MabiPropBehavior(null, _HandleSMBoardTouch));
            }
        }

        public void AddShadowMission(uint boardId, MissionInfo info)
        {
            MissionBoard board = null;
            if (!_shadowMissionBoards.TryGetValue(boardId, out board))
                return;

            if (board != null)
                board.AddMissionInfo(info);
        }

        public void BeginShadowMission(uint classId, byte difficulty, MabiPC player, ulong propId = 0)
        {
            var sm = this.GetShadowMissionInfo(classId);
            if (sm != null)
                this.BeginShadowMission(sm, difficulty, player, propId);
        }

        public void BeginShadowMission(uint classId, byte difficulty, MabiPC[] players, ulong propId = 0)
        {
            var sm = this.GetShadowMissionInfo(classId);
            if (sm != null)
                this.BeginShadowMission(sm, difficulty, players, propId);
        }

        public void BeginShadowMission(MissionInfo info, byte difficulty, MabiPC player, ulong propId = 0)
        {
            this.BeginShadowMission(info, difficulty, new MabiPC[] { player }, propId);
        }

        /// <summary>
        /// Begin a shadow mission.
        /// </summary>
        /// <param name="info">Shadow Mission info</param>
        /// <param name="creatures">Players (not pets) entering this mission</param>
        public void BeginShadowMission(MissionInfo info, byte difficulty, MabiPC[] players, ulong propId = 0)
        {
            // We'll probably want to send most data only once per client, but
            // send SM region data for pets that are summoned too.. hmm

            uint[] regionIds = new uint[info.Regions.Count];

            for (int i = 0; i < regionIds.Length; i++)
            {
                try
                {
                    regionIds[i] = (uint)_shadowMissionRegionPool.Next();
                }
                catch (Exception e)
                {
                    // Out of Ids.. this should hopefully never happen, else time to start worrying
                    // about the world population
                    throw e; // Just throw exception for now
                }
            }

            var sm = new MabiMission(regionIds, info, difficulty, propId);

            for (int i = 0; i < players.Length; i++)
                sm.Enter(players[i], (uint)i);

            try
            {
                _shadowMissionInstances.Add(sm.StartingRegion.Id, sm);
            }
            catch
            {
                // We got a duplicate Region Id, something that should
                // never happen unless a bug
                throw new Exception(String.Format("Duplicate region Ids, bug in IdPool? {0}", sm.StartingRegion.Id));
            }
        }

        public void EndShadowMission(MabiMission sm)
        {
            sm.Close();
            _shadowMissionInstances.Remove(sm.StartingRegion.Id);

            // Remove from pool
            // Assumes thread safety regarding sm.Regions
            foreach (var region in sm.Regions)
            {
                _shadowMissionRegionPool.Release(region.Id);
            }
        }

        public MissionBoard GetShadowMissionBoard(uint classId)
        {
            MissionBoard board = null;
            _shadowMissionBoards.TryGetValue(classId, out board);
            return board;
        }

        public MissionInfo GetShadowMissionInfo(uint classId)
        {
            MissionInfo info = null;

            // Check each board
            foreach (MissionBoard board in _shadowMissionBoards.Values)
            {
                info = board.GetMissionInfo(classId);
                if (info != null) return info;
            }

            return null;
        }

        private void HandleShadowMissionBoardTouch(WorldClient client, MabiPC character, MabiProp prop)
        {
            // Do nothing for right now
        }

        private void HandleShadowMissionAltarTouch(WorldClient client, MabiPC character, MabiProp prop)
        {
            // client.Send(PacketCreator.MsgBox(character, "This feature is not currently supported"));

            //this.BeginShadowMission(701009, client.Character); // Hardcoded
            this.TryBeginShadowMission(character, prop.Id);
        }

        /// <summary>
        /// Try to begin a Shadow Mission.
        /// </summary>
        /// <param name="player">Player that is invoking the mission (touched altar prop)</param>
        public bool TryBeginShadowMission(MabiPC player, ulong propId = 0)
        {
            if (!(player.Client is WorldClient)) return false;
            var client = player.Client as WorldClient;

            var smQuest = player.GetShadowMissionQuestOrNull();

            // No Shadow Mission quest found
            if (smQuest == null)
            {
                //client.Send(PacketCreator.MsgBox(player, "You don't have a Shadow Mission quest"));
                Send.MsgBox(client, player, "You don't have a Shadow Mission quest");
                return false;
            }

            var smInfo = this.GetShadowMissionInfo(smQuest.Class);

            // Appears to be a Shadow Mission quest, but class Id not recognized
            if (smInfo == null)
                return false;

            var players = new List<MabiPC>();

            var party = player.Party;
            if (party == null)
            {
                // Just this player

                // Too few players
                if (smInfo.PartyCountMin > 1)
                    return false;

                players.Add(player);
            }
            else
            {
                var members = party.Members;

                foreach (var member in members)
                {
                    if (member.IsPlayer && member is MabiPC)
                        players.Add(member as MabiPC);
                }
            }

            // TODO: Perform check that all members meet various requirements
            // to play SM

            this.BeginShadowMission(smInfo, smQuest.MissionDifficulty, players.ToArray(), propId);
            return true;
        }
    }
}