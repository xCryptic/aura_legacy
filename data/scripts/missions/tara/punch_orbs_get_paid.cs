// --- Aura Script ----------------------------------------------------------
//  Mission: Punch Orbs, Get Paid (custom test mission)
// --------------------------------------------------------------------------

using System.Collections;
using Aura.Shared.Const;
using Aura.Data;
using Aura.World.Scripting;
using Aura.World.Network;
using Aura.World.World;
using Aura.World.Util;
using Aura.Shared.Util;

public class PunchOrbsGetPaidScript : ShadowMissionScript
{
	public override void OnLoad()
	{	
		InitMission();
	}
	
	private void InitMission()
	{
		SetClass(701901);
		
		SetName("Punch Orbs, Get Paid");
		SetDescription("Hate orbs with a burning, unquenchable passion? Punch them all day and get paid!");
		SetPartyRange(1, 2);
		SetTimeLimit(3600000);
		
		SetMapRegion(401);
		SetMapCrop(200, 150, 800, 600);
		
		AddMapMarking(112400, 118600);
		
		SupportDifficulty(Difficulty.Basic);
		
		// Difficulty, Exp, Gold
		SetReward(Difficulty.Basic, 10, 2); // 2 whole gold, don't spend it all in one place
		
		SetSpawn(0, 8874, 6802); // Player 1
		SetSpawn(1, 8874, 6802); // Player 2
		
		SetUnknown13(63251);
		
		// Regions
		var region = AddRegion(411);
		SetDirectoryName(region, "Tara_castle_gatehall");
		SetVariationFile(region, "data/world/Tara_castle_gatehall/region_variation_1.xml");
		SetRegionUnknown1(region, 100);
		
		InitTaraMission(); // Adds to mission board, sets altar trigger, sets default exit
		FinishMissionInit();
	}
	
	public override IEnumerable Continue(MabiMission mission)
	{
		var spawner1 = new Spawner(411);
		uint region1 = GetRegionInstanceId(mission, 0);
		
		Logger.Info("[" + region1 + "] Step 1");
		
		var orb1 = SpawnOrb(regionId: region1,
						    point: spawner1.Point("room_middle_orig"),
						    callback: orb => { RemoveProp(orb); mission.Continue(); });
		yield return false; // Continue once orb is hit
		
		Logger.Info("[" + region1 + "] Step 2");
		
		var orb2 = SpawnOrb(regionId: region1,
						    point: spawner1.Point("room_middle_orig"),
						    callback: orb => { RemoveProp(orb); mission.Continue(); });
		yield return false; // Continue once orb is hit
		
		Logger.Info("[" + region1 + "] Step 3");
		
		mission.Succeed();
	}
}