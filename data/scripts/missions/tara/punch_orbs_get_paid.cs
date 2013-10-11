// --- Aura Script ----------------------------------------------------------
//  Mission: Punch Orbs, Get Paid (custom test mission)
// --------------------------------------------------------------------------

using System.Collections;
using Aura.Shared.Const;
using Aura.Data;
using Aura.World.Scripting;
using Aura.World.Network;
using Aura.World.World;

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
		SetDescription("Hate orbs with a burning, unquenchable passion? Punch them all day and get paid! Fuck yeah.");
		SetPartyRange(1, 2);
		SetTimeLimit(3600000);
		
		SetMapRegion(401);
		SetMapCrop(200, 150, 800, 600);
		
		AddMapMarking(112400, 118600);
		
		SupportDifficulty(Difficulty.Basic);
		
		// Difficulty, Exp, Gold
		SetReward(Difficulty.Basic, 10, 2); // 2 whole gold, don't spend it all in one place
		
		// TODO: Should use [0] as default spawn
		SetSpawn(0, 8874, 6802); // Player 1
		SetSpawn(1, 8874, 6802);
		
		// SetDefaultExit(0, 401, 81255, 126210); // Tara exit
		
		
		// Unknowns
		SetUnknown13(63251);
		
		var region = AddRegion(411);
		SetDirectoryName(region, "Tara_castle_gatehall");
		SetVariationFile(region, "data/world/Tara_castle_gatehall/region_variation_1.xml");
		SetRegionUnknown1(region, 100);
		
		//AddToBoard(41699); // Shadow Mission board prop it's tied to (Tara)
		//AddTrigger(0x00A00191000D001E); // Altar prop it's tied to (Tara)
		
		InitTaraMission(); // Adds to mission board, sets altar trigger, sets default exit
		
		HookMissionStart(OnMissionStart);
		
		FinishMissionInit();
	}
	
	/// <summary>
	/// Initialize the mobs.
	/// </summary>
	/*
	private void InitMobs(ShadowMission mission)
	{
		// Maybe use player count when initializing the mob percents/maximums
		var playerCount = mission.PlayerCount;
	
		// Add named mob groups
		AddPredefinedMobGroup(
			"group1", // Name of the group, null if none
			"root", // Name of pool/collection to add it to, "root" (or null) if root
			0f, // Chance of being randomly picked from respective pool
			
			// The following mob names should be set by an init script,
			// init_mobs_tara.cs or something, and underlying ShadowMissionScript
			// class (or Mission instance?) should auto-handle difficulty
			Mob("shadow_alchemist", 3), // 3 shadow alchemists
			Mob("stone_golem"), // 1 stone golem
			Mob("forest_golem"), // 1 forest golem
			Mob("sulfur_spider_small", 4), // Throw in some sulfur spiders for good measure and no real reason
			Mob("shadow_wizard") // 1 shadow wizard
		);
		
		AddRandomizedMobGroup(
			"rgroup1",
			"root",
			0f, // Only referenced by name
			Mob("shadow_alchemist", 40f),
			Mob("sulfur_spider_large", 10f),
			Mob("shadow_wizard", 20f),
			Mob("stone_golem", 20f),
			Mob("forest_golem", 5f),
			Mob("blinker", 5f, playerCount) // 5% chance to spawn a blinker, but no more than player count
		);
		
		// Name, x1, y1, x2, y2
		var bounds1 = AddSpawnBounds("bounds1", 8000, 8000, 8200, 8400);
		
		// Spawn a random, predefined group (NOT random like rgroup1) from root node
		SpawnRandomGroup();
		
		SpawnGroup("rgroup1", "bounds1"); 
		SpawnGroup("group1");
		
		//Spawn("group1");
		//Spawn("rgroup1", 20); // Spawn 20 creatures from rgroup1
	}
	*/
	
	private void OnMissionStart(MabiMission mission) // Mission instance calls this
	{
		//InitMobs(mission);
	
		uint tempRegion = GetRegionInstanceId(mission, 0); // First region
		
		// All one orb to punch for now
		//SpawnOrb(tempRegion, 8852, 8964, orb => { Complete(mission); });
		
		// Positions for each orb
		// Todo: Make PositionsGrid, PositionsCircle
		var positions = Positions(
				8852, 8764,
				8852, 8964,
				8852, 9164,
				8652, 8764,
				8652, 8964,
				8652, 9164,
				9052, 8764,
				9052, 8964,
				9052, 9164
			);
		
		// Spawn the orb group
		var group = SpawnBlinkingOrbGroup(tempRegion, positions, g => { Complete(mission); g.Dispose(); }, 3000); // Auto-adds to disposables?
		
		SetMarkers(mission, group); // To activate orbs and make them hittable, they must be set as markers
		
		// Temporary solution for disposable classes that are disposed along
		// with the mission being disposed. A better solution requires region
		// instances, but this works for now
		AddDisposable(mission, group);
	}
}