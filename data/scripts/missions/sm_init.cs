using System.Collections;
using Aura.Shared.Const;
using Aura.Data;
using Aura.World.Scripting;
using Aura.World.Network;
using Aura.World.World;
using Aura.Shared.Const;

public class ShadowMissionInitScript : BaseScript
{
	public override void OnLoad()
	{
		// Load SM boards/altars
		AddShadowMissionBoard(41400); // Tail board
		AddShadowMissionBoard(41699); // Tara board
		
		// propId, regionId, x, y
		AddShadowMissionAltar(0x00A0012C00120099, 300, 176112, 224672); // Tail altar
		AddShadowMissionAltar(0x00A00191000D001E, 401, 81787, 126630); // Tara altar
	}
}