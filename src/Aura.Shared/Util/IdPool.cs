using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aura.Shared.Util
{
    //public interface IdPoolValue
    //{
    //    static long operator +(IdPoolValue a, IdPoolValue b);
    //    static long operator -(IdPoolValue a, IdPoolValue b);
    //}

    public abstract class AbsIdPool
    {
        protected ulong _start = 0, _end = 0;
        protected ulong _currentUnused = 0;
        protected ulong _remaining = 0;

        public ulong Count
        {
            get { return _end - _start; }
        }

        public ulong Remaining
        {
            get { return _remaining; }
        }

        public AbsIdPool(ulong start, ulong end)
        {
            if (start < end) { _start = start; _end = end; }
            else { _start = end; _end = start; }

            _remaining = this.Count;
            _currentUnused = _start;
        }

        public abstract void Release(ulong[] vals);
        public abstract void Release(ulong val);

        public abstract ulong[] Next(uint count);
        public abstract ulong Next();
    }

    public class SmallIdPool : AbsIdPool
    {
        private Object _lock = new Object();

        // This should be optimized (one bit per element) by runtime
        // With a 25,000 Id range, this should only take up 3125 bytes, about 3 kilobytes
        private bool[] _inUse = null;

        public SmallIdPool(ulong start, ulong end)
            : base(start, end)
        {
            _inUse = new bool[_remaining];
        }

        public override void Release(params ulong[] vals)
        {
            for (int i = 0; i < vals.Length; i++)
                this.Release(vals[i]);
        }

        public override void Release(ulong val)
        {
            if (val < _start || val >= _end)
                return;

            lock (_lock)
            {
                _inUse[(val - _start)] = false;

                // Set current pointer to this released val
                if (val < _currentUnused)
                    _currentUnused = val;
            }
        }

        public override ulong[] Next(uint count)
        {
            ulong[] r = new ulong[count];
            for (uint i = 0; i < count; i++)
                r[i] = this.Next();
            return r;
        }

        public override ulong Next()
        {
            ulong next = 0;
            bool nextSet = false;

            // Thread safety is very important, especially here
            lock (_lock)
            {
                next = _currentUnused;

                while (next < _end && _inUse[(next - _start)])
                    next++;

                // We got one
                if (next < _end)
                {
                    _inUse[(next - _start)] = true;
                    nextSet = true;
                }
            }

            if (!nextSet)
                throw new Exception("No Ids remaining");

            return next;
        }
    }

    // TODO: Make generic compatible
    /*
    public class IdPool
    {
        private uint _start = 0, _end = 0;
        private uint _currentUnused = 0;
        private Object _lock = new Object();
        private uint _remaining = 0;

        // This should be optimized (one bit per element) by runtime
        // With a 25,000 Id range, this should only take up 3125 bytes, about 3 kilobytes
        private bool[] _inUse = null; 
        
        public uint Count
        {
            get { return _end - _start; }
        }

        public uint Remaining
        {
            get { return this._remaining; }
        }

        /// <summary>
        /// Create a new Id pool with the specified range: [start, end)
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public IdPool(uint start, uint end)
        {
            if (start < end) { _start = start; _end = end; }
            else { _start = end; _end = start; }

            _remaining = this.Count;
            _inUse = new bool[_remaining];
            _currentUnused = _start;
        }

        public void Release(params uint[] vals)
        {
            for (int i = 0; i < vals.Length; i++)
                this.Release(vals[i]);
        }

        public void Release(uint val)
        {
            if (val < _start || val >= _end)
                return;

            lock (_lock)
            {
                _inUse[val] = false;

                // Set current pointer to this released val
                if (val < _currentUnused)
                    _currentUnused = val;
            }
        }

        public uint[] Next(uint count)
        {
            uint[] r = new uint[count];
            for (int i = 0; i < count; i++)
                r[i] = this.Next();
            return r;
        }

        public uint Next()
        {
            uint next = 0;
            bool nextSet = false;

            // Thread safety is very important, especially here
            lock (_lock)
            {
                next = _currentUnused;
                
                while (next < _end && _inUse[(next - _start)])
                    next++;

                // We got one
                if (next < _end)
                {
                    _inUse[(next - _start)] = true;
                    nextSet = true;
                }
            }

            if (!nextSet)
                throw new Exception("No Ids remaining");

            return next;
        }
    }
    */

    /// <summary>
    /// An Id pool specialized for very large ranges.
    /// </summary>
    //public class LargeIdPool : AbsIdPool
    //{
    //    public LargeIdPool(ulong start, ulong end)
    //        : base(start, end)
    //    {
    //
    //    }
    //}
}