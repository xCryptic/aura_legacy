using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aura.World.World;

namespace Aura.World.Scripting
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class MissionScript : BaseScript
    {
        public virtual IEnumerable Continue(MabiMission mission) { yield break; }
    }
}
