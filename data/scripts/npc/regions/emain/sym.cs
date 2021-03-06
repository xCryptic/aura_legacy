using Aura.Shared.Const;
using System;
using Aura.World.Network;
using Aura.World.Scripting;
using Aura.World.World;

public class SymScript : NPCScript
{
	public override void OnLoad()
	{
		base.OnLoad();
		SetName("_sym");
		SetRace(10002);
		SetBody(height: 0.1000001f, fat: 1.2f, upper: 1.3f, lower: 1.1f);
		SetFace(skin: 17, eye: 167, eyeColor: 162, lip: 54);

		SetColor(0x0, 0x0, 0x0);

		EquipItem(Pocket.Face, 0x1324, 0xF262A2, 0x5B3F60, 0x6C81);
		EquipItem(Pocket.Hair, 0x1774, 0x6C6955, 0x6C6955, 0x6C6955);
		EquipItem(Pocket.Armor, 0x3B03, 0x304E48, 0x8B9C65, 0x282795);
		EquipItem(Pocket.Shoe, 0x427C, 0x7D99AF, 0x808080, 0x808080);

		SetLocation(region: 52, x: 30954, y: 43159);

		SetDirection(110);
		SetStand("chapter4/human/male/anim/male_c4_npc_cry_boy");
        
		Phrases.Add("(He thrashes about on the floor.)");
		Phrases.Add("I want it I want it I WANT IT!!");
		Phrases.Add("Waah! Waah!");
		Phrases.Add("Waah!!");
	}
}
