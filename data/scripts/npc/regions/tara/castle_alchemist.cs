using Common.Constants;
using Common.World;
using System;
using World.Network;
using World.Scripting;
using World.World;

public class Castle_alchemistScript : NPCScript
{
	public override void OnLoad()
	{
		SetName("_castle_alchemist");
		SetRace(10002);
		SetBody(height: 0.9f, fat: 1.3f, upper: 1.2f, lower: 1f);
		SetFace(skin: 21, eye: 20, eyeColor: 0, lip: 22);

		NPC.ColorA = 0x808080;
		NPC.ColorB = 0x808080;
		NPC.ColorC = 0x808080;		

		EquipItem(Pocket.Face, 0x1324, 0x521313, 0x62003C, 0x565700);
		EquipItem(Pocket.Hair, 0x1779, 0xC69500, 0xC69500, 0xC69500);
		EquipItem(Pocket.Armor, 0x3BE5, 0x211C39, 0xB58A7B, 0xF7F3DE);
		EquipItem(Pocket.Head, 0x4657, 0x211C39, 0x9F8DF5, 0x7B00CD);
		EquipItem(Pocket.RightHand1, 0x9D42, 0x669966, 0xBD7D21, 0x9C5D42);

		SetLocation(region: 401, x: 105064, y: 112507);

		SetDirection(164);
		SetStand("human/male/anim/male_stand_Tarlach_anguish");
	}
}