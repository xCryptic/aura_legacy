// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see licence.txt in the main folder

using System.Runtime.InteropServices;
using Aura.Shared.Network;
using Aura.World.World;
using Aura.World.Network;
using Aura.Data;
using Aura.World.Player;
using System;
using Aura.Shared.Util;

namespace Aura.World.World
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct MabiPropInfo
	{
		public uint Class;
		public uint Region;
		public float X;
		public float Altitude;
		public float Y;
		public float Direction;
		public float Scale;
		public uint Color1;
		public uint Color2;
		public uint Color3;
		public uint Color4;
		public uint Color5;
		public uint Color6;
		public uint Color7;
		public uint Color8;
		public uint Color9;
		public byte FixedAltitude;
        // Will want to access these, to public
        public byte __unknown65;
        public byte __unknown66;
        public byte __unknown67;
	}

	public class MabiProp : MabiEntity
	{
		private static ulong _propIndex = Aura.Shared.Const.Id.Props;

		public MabiPropInfo Info;

		public string Name { get; set; }
		public string Title { get; set; }

		/// <summary>
		/// Known states: single, closed, open, state1-3
		/// </summary>
		public string State { get; set; }
		public string ExtraData { get; set; }
        public bool UseExtraData { get; set; } // Added w/ mission stuff

		public override EntityType EntityType
		{
			get { return EntityType.Prop; }
		}

		public override MabiVertex GetPosition()
		{
			return new MabiVertex((uint)this.Info.X, (uint)this.Info.Y);
		}

		public override uint Region
		{
			get { return this.Info.Region; }
			set { this.Info.Region = value; }
		}

		public override ushort DataType
		{
			get { return 160; }
		}

        /// <summary>
        /// Whether or not a prop is touchable. This modifies the prop Id, as
        /// an Id such as YYYYYYYY0001XXXX is touchable, whereas
        /// YYYYYYYYFFFFXXXX is not touchable. This is more of a temporary
        /// workaround.
        /// </summary>
        public bool IsTouchable
        {
            get { return ((this.Id & 0xFFFF0000) == 0x00010000); }
            set
            {
                // Set touchable
                if (value)
                {
                    this.Id &= 0xFFFFFFFF0000FFFFL;
                    this.Id |= 0x10000L;
                }
                // Set untouchable
                else
                {
                    this.Id |= 0xFFFF0000L;
                }
            }
        }

		public MabiProp(ulong id, uint region, uint x, uint y)
		{
			this.Id = id;
			this.Region = region;
			this.Info.X = x;
			this.Info.Y = y;
		}

		public MabiProp(uint cls, uint region, uint x, uint y, float direction, float scale = 1f, float altitude = 0)
			: this("", "", "", cls, region, x, y, direction, scale, altitude)
		{ }

		public MabiProp(string name, string title, string extra, uint cls, uint region, uint x, uint y, float direction, float scale = 1, float altitude = 0)
			: this(0, name, title, extra, cls, region, x, y, direction, scale, altitude)
		{
			this.Id = ++_propIndex;
			this.Id += (ulong)region << 32;
			this.Id += MabiData.RegionInfoDb.GetAreaId(region, x, y) << 16;
		}

		public MabiProp(ulong id, string name, string title, string extra, uint cls, uint region, uint x, uint y, float direction, float scale = 1, float altitude = 0)
		{
			this.Id = id;

			this.Name = name;
			this.Title = title;
			this.ExtraData = extra;

            // If something in extra, use it
            this.UseExtraData = (extra != null && extra.Length > 0);

			this.Info.Class = cls;
			this.Info.Region = region;
			this.Info.X = x;
			this.Info.Y = y;
			this.Info.Direction = direction;
			this.Info.Scale = scale;

			this.Info.Color1 =
			this.Info.Color2 =
			this.Info.Color3 =
			this.Info.Color4 =
			this.Info.Color5 =
			this.Info.Color6 =
			this.Info.Color7 =
			this.Info.Color8 =
			this.Info.Color9 = 0xFF808080;
		}
	}

	public delegate void MabiPropFunc(WorldClient client, MabiPC character, MabiProp prop);

	public class MabiPropBehavior
	{
		public MabiProp Prop { get; set; }
		public MabiPropFunc Func { get; protected set; }

		public MabiPropBehavior(MabiProp prop, MabiPropFunc func = null)
		{
			this.Prop = prop;
			this.Func = func;
		}
	}
}
