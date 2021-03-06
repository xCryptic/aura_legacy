﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see licence.txt in the main folder

using Aura.Shared.Network;
using Aura.Shared.Util;
using Aura.World.Network;
using Aura.World.World;

namespace Aura.World.Skills
{
	/// <summary>
	/// Simplified handler for skills using Start/Stop, that's automatically
	/// sending back back SkillStart/SkillStop.
	/// </summary>
	public abstract class StartStopSkillHandler : SkillHandler
	{
		public override SkillResults Start(MabiCreature creature, MabiSkill skill, MabiPacket packet)
		{
			var parameter = (packet != null ? packet.GetStringOrEmpty() : "");

			var result = this.Start(creature, skill);

			Send.SkillStart(creature, skill.Id, parameter);

			return result;
		}

		public override SkillResults Stop(MabiCreature creature, MabiSkill skill, MabiPacket packet)
		{
			var parameter = (packet != null && packet.GetElementType() == ElementType.String ? packet.GetString() : null);

			var result = this.Stop(creature, skill);

			if (parameter != null)
				Send.SkillStop(creature, skill.Id, parameter);
			else
				Send.SkillStop(creature, skill.Id, 1);

			return result;
		}

		public abstract SkillResults Start(MabiCreature creature, MabiSkill skill);
		public abstract SkillResults Stop(MabiCreature creature, MabiSkill skill);
	}
}
