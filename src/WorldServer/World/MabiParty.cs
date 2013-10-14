// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see licence.txt in the main folder

using System.Collections.Generic;
using Aura.Shared.Network;

namespace Aura.World.World
{
	public enum PartyFinishRule { BiggestContributer = 0, Turn = 1, Anyone = 2 }
	public enum PartyExpSharing { Equal = 0, MoreToFinish = 1, AllToFinish = 2 }

	public class MabiParty
	{
		public uint Type = 0;
		public string Name = "";
		public string Level = "";
		public string Info = "";
		public string Password = "";
		public uint MaxSize = 8;

		public PartyFinishRule Finish = PartyFinishRule.BiggestContributer;
		public PartyExpSharing ExpShare = PartyExpSharing.Equal;

		public List<MabiCreature> Members = new List<MabiCreature>();
		private MabiCreature _leader;
		public MabiCreature Leader { get { return _leader; } }
		public bool IsOpen = false;

		private ulong _id;
        public ulong Id
        {
            get { return _id; }
        }

		private static ulong _worldPartyIndex = Aura.Shared.Const.Id.Parties;
		public static ulong NewPartyId { get { return _worldPartyIndex++; } }

		public MabiParty(MabiCreature leader)
		{
			_id = NewPartyId;

			Members.Add(leader);
			_leader = leader;
			_leader.PartyNumber = 1;
		}

		public void LoadFromPacket(MabiPacket packet)
		{
			Type = packet.GetInt();
			Name = packet.GetString();
			Level = (Type == 1) ? packet.GetString() : "";
			Info = (Type == 1) ? packet.GetString() : "";
			Password = packet.GetString();
			MaxSize = packet.GetInt();

			// TODO: PartyBoard Support
			/*
			PartyBoard = packet.GetByte();
			005 [..............01] Byte   : 1
			006 [0000000000000000] Long   : 0
			007 [0000000400000000] Long   : 17179869184
			008 [................] String :
			009 [..............46] Byte   : 70
			010 [..............00] Byte   : 0
			011 [..............00] Byte   : 0
			012 [................] String :
			013 [................] String : Unrestricted
			014 [................] String : Unrestricted
			015 [................] String :
			016 [............0000] Short  : 0
			017 [........00000000] Int      : 0
			018 [........00000000] Int      : 0
			019 [........00000000] Int      : 0
			020 [..............01] Byte   : 1
			*/
		}

		public string GetMemberWantedString()
		{
			return string.Format("p{0}{1:d2}{2:d2}{3}{4}", Type, Members.Count, MaxSize, (HasPassword() ? "y" : "n"), Name);
		}

		public uint GetAvailablePartyNumber()
		{
			var numberTaken = new List<uint>();
			foreach (var member in Members)
				numberTaken.Add(member.PartyNumber);

			for (uint i = 1; i <= MaxSize; i++)
			{
				if (!numberTaken.Contains(i))
					return i;
			}

			return 0;
		}

		public MabiCreature GetNextLeader()
		{
			if (Members.Count > 0)
				return Members[0];

			return null;
		}

		public void SetLeader(MabiCreature leader)
		{
			if (!Members.Contains(leader))
				return;

			_leader = leader;
		}

		public bool HasPassword()
		{
			return (Password != "") ? true : false;
		}

		public void AddPartyMember(MabiCreature creature)
		{
			Members.Add(creature);
			creature.PartyNumber = this.GetAvailablePartyNumber();
		}

		public void RemovePartyMember(MabiCreature creature)
		{
			if (Members.Contains(creature))
				Members.Remove(creature);
		}

		public uint GetMemberNumber(MabiCreature member)
		{
			if (!Members.Contains(member))
				return 0;

			return member.PartyNumber;
		}

		public void AddMemberPacket(MabiPacket packet, MabiCreature member)
		{
			if (!Members.Contains(member))
				return;

			packet.PutInt(this.GetMemberNumber(member));
			packet.PutLong(member.Id);
			packet.PutString(member.Name);
			packet.PutByte(1); // ?
			packet.PutInt(member.Region);
			MabiVertex loc = member.GetPosition();
			packet.PutInt(loc.X);
			packet.PutInt(loc.Y);
			packet.PutByte(0); // ?
			packet.PutInt((uint)((member.Life * 100) / member.LifeMax));
			packet.PutInt((uint)member.LifeMax);
			packet.PutLong(0); // ?
		}

		public void AddPartyPacket(MabiPacket packet)
		{
			packet.PutLong(_id); // Party ID
			packet.PutString(Name);
			packet.PutLong(Leader.Id);
			packet.PutByte(IsOpen);
			packet.PutInt((uint)Finish);
			packet.PutInt((uint)ExpShare);
			packet.PutLong(0); // Quest ID?
			packet.PutInt(MaxSize);
			packet.PutInt(Type);
			packet.PutString(Level);
			packet.PutString(Info);
			packet.PutInt((uint)Members.Count);

			foreach (var member in Members)
				AddMemberPacket(packet, member);

			packet.PutByte(0);
		}
	}
}