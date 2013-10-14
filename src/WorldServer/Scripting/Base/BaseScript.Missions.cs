﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aura.World.World;
using Aura.Data;

using System.Threading;
using Aura.World.Player;
using Aura.World.Network;
using Aura.Shared.Network;

namespace Aura.World.Scripting
{
    public partial class BaseScript
    {
        /// <summary>
        /// Add a shadow mission board. Should be used in init script, there might be
        /// a better place to add this..
        /// </summary>
        /// <param name="classId">Class Id of board prop</param>
        public void AddShadowMissionBoard(uint classId)
        {
            MissionManager.Instance.AddShadowMissionBoard(classId);
        }

        /// <summary>
        /// Add a shadow mission altar.
        /// </summary>
        /// <param name="propId">Prop Id of altar</param>
        public void AddShadowMissionAltar(ulong propId, uint regionId, uint x, uint y)
        {
            MissionManager.Instance.AddShadowMissionAltar(propId, regionId, x, y);
        }

        public MabiOrbProp SpawnOrb(uint regionId, Point point, OrbHitCallback callback)
        {
            return this.SpawnOrb(regionId, point.X, point.Y, callback);
        }

        public MabiOrbProp SpawnOrb(uint regionId, uint x, uint y, OrbHitCallback callback)
        {
            // Note: Color2 is orb color, all others are default (do nothing probably)
            MabiOrbProp orb = new MabiOrbProp(regionId, x, y, callback); // Hardcoded for now
            orb.IsOn = true;
            
            // These might not be constant.. check more orbs
            //orb.Info.__unknown65 = 0x31;
            //orb.Info.__unknown66 = 0x2C;
            //orb.Info.__unknown67 = 0x34;

            WorldManager.Instance.AddProp(orb);
            return orb;
        }

        public List<Tuple<uint, uint>> Positions(params uint[] p)
        {
            var list = new List<Tuple<uint, uint>>();
            for (int i = 0; (i + 1) < p.Length; i += 2)
            {
                list.Add(new Tuple<uint, uint>(p[i], p[i + 1]));
            }
            return list;
        }

        // TODO: SpawnDoorOrbGroup

        // TODO: SpawnMobOrbGroup

        // TODO: SpawnCircleOrbGroup

        // ---------------------
        // SpawnBlinkingOrbGroup
        // ---------------------

        public BlinkingOrbGroup SpawnBlinkingOrbGroup(string region, List<Tuple<uint, uint>> pos, OrbGroupCompleteCallback callback = null, uint deltaTime = 10000, uint maxEnabled = 0, OrbHitCallback badOrb = null)
        {
            uint regionId = MabiData.RegionDb.TryGetRegionId(region);
            return this.SpawnBlinkingOrbGroup(regionId, pos, callback, deltaTime, maxEnabled, badOrb);
        }

        public BlinkingOrbGroup SpawnBlinkingOrbGroup(uint regionId, List<Tuple<uint, uint>> pos, OrbGroupCompleteCallback callback = null, uint deltaTime = 10000, uint maxEnabled = 0, OrbHitCallback badOrb = null)
        {
            var orbs = new List<MabiOrbProp>();
            foreach (var p in pos)
            {
                var orb = this.SpawnOrb(regionId, p.Item1, p.Item2, null);
                orb.Info.Color2 = 0xFFF0F0F0; // White-ish
                orbs.Add(orb);
            }
            return this.SpawnBlinkingOrbGroup(regionId, orbs, callback, deltaTime, maxEnabled, badOrb);
        }
        /*
        public BlinkingOrbGroup SpawnBlinkingOrbGroup(string region, List<Tuple<uint, uint>> pos, OrbGroupCompleteCallback callback = null, OrbHitCallback badOrb = null)
        {
            uint regionId = MabiData.MapDb.TryGetRegionId(region);
            return this.SpawnBlinkingOrbGroup(regionId, pos, callback, badOrb);
        }

        public BlinkingOrbGroup SpawnBlinkingOrbGroup(uint regionId, List<Tuple<uint, uint>> pos, OrbGroupCompleteCallback callback = null, OrbHitCallback badOrb = null)
        {
            return this.SpawnBlinkingOrbGroup(regionId, pos, callback, 10000, 0, badOrb);
        }
        */
        public BlinkingOrbGroup SpawnBlinkingOrbGroup(uint regionId, List<MabiOrbProp> orbs, OrbGroupCompleteCallback callback = null, uint deltaTime = 10000, uint maxEnabled = 0, OrbHitCallback badOrb = null)
        {
            if (maxEnabled == 0 || maxEnabled > orbs.Count)
                maxEnabled = (uint)orbs.Count;

            var group = new BlinkingOrbGroup(orbs.ToArray(), callback, deltaTime, maxEnabled, badOrb);

            // ...

            return group;
        }

        public BlinkingOrbGroup SpawnStarOrbGroup(uint regionId, uint cx, uint cy, double radius, double dir,
            OrbGroupCompleteCallback callback = null, uint deltaTime = 10000, uint maxEnabled = 0, OrbHitCallback badOrb = null)
        {
            var positions = Aura.World.Util.Positions.Circle(cx, cy, radius, dir, 5);
            var orbs = new List<MabiOrbProp>();
            foreach (var pair in positions)
            {
                var orb = this.SpawnOrb(regionId, (uint)pair.Item1, (uint)pair.Item2, null);
                orb.Info.Color2 = 0xFFF0F0F0; // White-ish
                orbs.Add(orb);
            }
            return this.SpawnBlinkingOrbGroup(regionId, orbs, callback, deltaTime, maxEnabled, badOrb);
        }

        public void RemoveProp(MabiProp prop)
        {
            WorldManager.Instance.RemoveProp(prop);
        }
    }
}

// Add this here for now..
namespace Aura.World.World
{
    public delegate void OrbGroupCompleteCallback(MabiOrbGroup orbGroup);
    public delegate void OrbHitCallback(MabiOrbProp orb);

    //public class BoolStateProp : MabiProp
    //{
    //    public bool On
    //    {
    //        get { return (this.State != null && this.State.Equals("on")); }
    //        set { this.State = (value ? "on" : "off"); }
    //    }
    //}

    /// <summary>
    /// Not sure if this belongs in core, might add as prop script..
    /// </summary>
    public class MabiOrbProp : MabiProp
    {
        /// <summary>
        /// The group this orb is assigned to, or null if none.
        /// </summary>
        public MabiOrbGroup Group = null;

        public bool IsOn
        {
            get { return (this.State != null && this.State.Equals("on")); }
            set { this.State = (value ? "on" : "off"); }
        }

        public MabiOrbProp(uint region, float x, float y, OrbHitCallback callback)
            : this(region, (uint)x, (uint)y, callback)
        {
        }

        public MabiOrbProp(uint region, uint x, uint y, OrbHitCallback callback)
            : base(41467, region, x, y, 0f)
        {
            if (callback != null)
                this.OnHit += callback;

            this.IsTouchable = true;
            this.UseExtraData = false;

            this.InitOrb();
        }

        public MabiPropBehavior Behavior = null;
        public event OrbHitCallback OnHit;

        public override void Dispose()
        {
            // Note from experience: Don't fucking call RemoveProp here

            this.RemoveEvent(); // If it's registered
            base.Dispose();
        }

        private void InitOrb()
        {
            this.Behavior = new MabiPropBehavior(this, this.OrbHitBehaviorCallback);
            this.RegisterEvent();
        }

        private void RegisterEvent()
        {
            WorldManager.Instance.SetPropBehavior(this.Behavior);
        }

        private void RemoveEvent()
        {
            // IMPORTANT: Only commented because function no longer exists
            //WorldManager.Instance.RemovePropBehavior(this.Id);
        }

        /// <summary>
        /// Callback to pass to the MabiPropBehavior.
        /// </summary>
        private void OrbHitBehaviorCallback(WorldClient client, MabiPC character, MabiProp prop)
        {
            this.OnHit(this);
        }
    }

    /*
    public class CircleOrbProp : MabiOrbProp
    {
        private MabiProp _circleProp = null;

        /// <summary>
        /// Add the status of the circle on the ground to packet..? Seems similar
        /// to a prop update, packet with Op 0x52DB.
        /// </summary>
        /// <param name="packet">Packet to add to</param>
        public void AddCircleStatusToPacket(MabiPacket packet)
        {
            if (_circleProp == null) return;

            packet.PutInt(2)
                  .PutInt(_circleProp.Region)
                  .PutFloat(_circleProp.Info.X)
                  .PutFloat(_circleProp.Info.Y)
                  .PutFloat(1f) // ?
                  .PutString(_circleProp.State); // default, changed1, changed2, completed
        }
    }
    */

    public abstract class MabiOrbGroup : IDisposable
    {
        public SortedDictionary<ulong, MabiOrbProp> Orbs
            = new SortedDictionary<ulong, MabiOrbProp>();

        private OrbGroupCompleteCallback OnComplete = null;

        public MabiOrbGroup(MabiOrbProp orb, OrbGroupCompleteCallback callback)
            : this(new MabiOrbProp[] { orb }, callback)
        {
        }

        public MabiOrbGroup(MabiOrbProp[] orbs, OrbGroupCompleteCallback callback)
        {
            this.InitOrbs(orbs);
            this.OnComplete = callback;
            this.Init();
        }

        /// <summary>
        /// Dispose of resources and those of associated orbs, also broadcasts that they
        /// are removed.
        /// </summary>
        public virtual void Dispose()
        {
            foreach (var pair in this.Orbs)
            {
                // This function calls the prop's Dispose()
                WorldManager.Instance.RemoveProp(pair.Value);
            }
        }

        protected void InitOrbs(IEnumerable<MabiOrbProp> orbs)
        {
            foreach (var orb in orbs)
            {
                orb.Group = this;
                orb.OnHit += this.OnOrbHit;
                this.Orbs.Add(orb.Id, orb);
            }
        }

        /// <summary>
        /// To be called by child class to signal the orb group has been "completed."
        /// </summary>
        protected void Complete()
        {
            var f = this.OnComplete;
            if (f != null) f(this);
        }

        /// <summary>
        /// Init function to be overridden by a child class.
        /// </summary>
        protected abstract void Init();

        /// <summary>
        /// Function called when an orb in this group is hit. To be overridden
        /// by a child class.
        /// </summary>
        /// <param name="orb">Orb that was hit</param>
        protected abstract void OnOrbHit(MabiOrbProp orb);
    }

    /// <summary>
    /// A group of orbs, one of which (selected randomly) triggers completion, while
    /// the others trigger some other event.
    /// </summary>
    public class TriggerOrbGroup : MabiOrbGroup
    {
        private ulong _trigger = 0;

        private OrbHitCallback OnHitBadOrb;

        public TriggerOrbGroup(MabiOrbProp orb, OrbGroupCompleteCallback callback, OrbHitCallback badOrb)
            : base(orb, callback)
        {
            this.OnHitBadOrb = badOrb;
        }

        public TriggerOrbGroup(MabiOrbProp[] orbs, OrbGroupCompleteCallback callback, OrbHitCallback badOrb)
            : base(orbs, callback)
        {
            this.OnHitBadOrb = badOrb;
        }

        protected override void Init()
        {
            var rng = new System.Random();
            int index = rng.Next(this.Orbs.Count);
            _trigger = this.Orbs.ElementAt(index).Key; // Select the trigger which calls complete
        }

        protected override void OnOrbHit(MabiOrbProp orb)
        {
            if (orb.Id == _trigger)
                this.Complete();

            else
                this.OnHitBadOrb(orb);
        }
    }

    /// <summary>
    /// A group of orbs, one of which triggers the opening of a door.
    /// </summary>
    //public class DoorTriggerOrbGroup : TriggerOrbGroup
    //{
    /// <summary>
    /// Open the door
    /// </summary>
    //    protected void OpenDoor()
    //    {
    // TODO: Investigate packets and see what needs to be done/sent
    //    }
    //}



    public class BlinkingOrbGroup : MabiOrbGroup
    {
        private uint _remaining = 0;

        private SortedDictionary<ulong, bool> _orbStatuses
            = new SortedDictionary<ulong, bool>();

        private OrbHitCallback OnHitBadOrb = null;
        private uint _deltaTime = 10000; // Amount of time between auto Refresh()

        private Timer _refreshTimer = null;
        private Object _timerLock = new Object();

        /// <summary>
        /// PRNG used in Refresh.
        /// </summary>
        private System.Random _random = Aura.Shared.Util.RandomProvider.Get();

        public BlinkingOrbGroup(MabiOrbProp orb, OrbGroupCompleteCallback callback,
            uint deltaTime = 10000, uint maxEnabled = 0, OrbHitCallback badOrb = null)
            : base(orb, callback)
        {
            _Init(deltaTime, maxEnabled, badOrb);
        }

        public BlinkingOrbGroup(MabiOrbProp[] orbs, OrbGroupCompleteCallback callback,
            uint deltaTime = 10000, uint maxEnabled = 0, OrbHitCallback badOrb = null)
            : base(orbs, callback)
        {
            _Init(deltaTime, maxEnabled, badOrb);
        }

        private void _Init(uint deltaTime, uint maxEnabled, OrbHitCallback badOrb)
        {
            this.OnHitBadOrb = badOrb;
            _deltaTime = deltaTime;
            _remaining = maxEnabled; //maxEnabled == 0 ? (uint)this.Orbs.Count : maxEnabled;

            // Copied from old Init():
            // Don't overwrite custom Remaining value
            if (_remaining == 0)
                _remaining = (uint)(this.Orbs.Count);

            foreach (var pair in this.Orbs)
                _orbStatuses.Add(pair.Key, false);

            lock (_timerLock)
                _refreshTimer = new Timer(this.Refresh, null, 0, _deltaTime);

            this.Refresh();
        }

        public override void Dispose()
        {
            this.DisposeTimer();

            base.Dispose();
        }

        /// <summary>
        /// Dispose of the Timer object.
        /// </summary>
        protected void DisposeTimer()
        {
            lock (_timerLock)
            {
                if (_refreshTimer != null)
                {
                    _refreshTimer.Dispose();  // Assuming this disposes correctly?
                    _refreshTimer = null;
                }
            }
        }
        
        protected override void Init()
        {
        //    // Don't overwrite custom Remaining value
        //    if (_remaining == 0)
        //        _remaining = (uint)(this.Orbs.Count);
        //
        //    foreach (var pair in this.Orbs)
        //        _orbStatuses.Add(pair.Key, false);
        //
        //    lock (_timerLock)
        //        _refreshTimer = new Timer(this.Refresh, null, 0, _deltaTime);
        //
        //    this.Refresh();
        }

        /// <summary>
        /// Called when an orb in this group is hit.
        /// </summary>
        /// <param name="orb">Orb that was hit</param>
        protected override void OnOrbHit(MabiOrbProp orb)
        {
            //bool status = false;
            //if (!_orbStatuses.TryGetValue(orb.Id, out status))
            //{
            // Orb status not found? Should never happen unless a bug
            //    throw new Exception(String.Format("OrbStatuses entry could not be found for prop Id: {0}", orb.Id));
            //}

            // Two people hitting an active orb at once could cause race issues..
            // Makes the assumptions that Complete, OnHitBadOrb callbacks don't
            // lock this orb as well
            lock (orb)
            {
                bool status = orb.IsOn;
                
                if (status) // Hit a good orb
                {
                    orb.IsOn = false;
                    _orbStatuses[orb.Id] = false;

                    Send.PropUpdate(orb);
                    
                    _remaining--;
                    
                    if (_remaining == 0)
                    {
                        this.DisposeTimer();
                        
                        this.Complete();
                    }
                }
                else if (this.OnHitBadOrb != null) // If you want a bad orb event..
                    this.OnHitBadOrb(orb);
            }
        }

        /// <summary>
        /// Called on timely refresh.
        /// TODO: Send prop updates all in one packet, instead of
        /// one per packet.
        /// </summary>
        /// <param name="state"></param>
        private void Refresh(object state = null)
        {
            // Nothing to do
            if (_remaining == this.Orbs.Count)
                return;
            
            List<MabiOrbProp> o = this.Orbs.Values.ToList<MabiOrbProp>();
            o = this.RandomOrbs(o, _remaining);
            
            // These 2 for loops together might not be most efficient method
            foreach (var orb in this.Orbs.Values)
                lock (orb) { orb.IsOn = false; }

            foreach (var orb in o)
                lock (orb) { orb.IsOn = true; }

            // Send necessary prop updates
            foreach (var pair in this.Orbs)
            {
                var wasPrevOn = _orbStatuses[pair.Key];
                var orb = pair.Value;
                var isOn = orb.IsOn;

                // Leave commented for now:
                //if (wasPrevOn != isOn)
                    Send.PropUpdate(orb);

                _orbStatuses[pair.Key] = orb.IsOn;
            }
        }

        public List<MabiOrbProp> RandomOrbs(List<MabiOrbProp> orbs, uint count)
        {
            if (count >= orbs.Count) return orbs;

            int remove = (int)(orbs.Count - count);

            for (int i = 0; i < remove; i++)
            {
                orbs.RemoveAt(_random.Next(orbs.Count));
            }

            return orbs;
        }
    }
}