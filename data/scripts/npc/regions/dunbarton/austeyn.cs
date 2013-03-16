using Aura.Shared.Const;
using System;
using Aura.World.Network;
using Aura.World.Scripting;
using Aura.World.World;

public class AusteynScript : NPCScript
{
	public override void OnLoad()
	{
		base.OnLoad();
		SetName("_austeyn");
		SetRace(10002);
		SetBody(height: 0.9999999f, fat: 1f, upper: 1.2f, lower: 1f);
		SetFace(skin: 16, eye: 8, eyeColor: 84, lip: 1);

		SetColor(0x0, 0x0, 0x0);

		EquipItem(Pocket.Face, 0x1328, 0x81A6, 0x806588, 0x8DA62C);
		EquipItem(Pocket.Hair, 0xFBB, 0xD1D9E3, 0xD1D9E3, 0xD1D9E3);
		EquipItem(Pocket.Armor, 0x3A9B, 0x36485A, 0xBDC2B1, 0x626C76);
		EquipItem(Pocket.Shoe, 0x4271, 0x36485A, 0xFFE1B9, 0x9A004E);

		SetLocation(region: 20, x: 660, y: 770);

		SetDirection(251);
		SetStand("");
        
        Phrases.Add("*Doze off*");
		Phrases.Add("*Yawn*");
		Phrases.Add("Ah... How boring...");
		Phrases.Add("Come to think of it, it's been a while since my last hair cut.");
		Phrases.Add("It's boring during the day with everyone attending school.");
		Phrases.Add("Let's see... That fellow should be coming to the Bank soon.");
		Phrases.Add("Mmm... I must have dozed off.");
		Phrases.Add("Mmm... I'm tired...");
		Phrases.Add("My body's not like it used be... Hahaha.");
		Phrases.Add("Oops. The mistakes have been getting more frequent lately.");
		Phrases.Add("Perhaps I should hire a cute office assistant. Who knows? Maybe that will bring in more business.");
		Phrases.Add("That fellow looks like he might have some Gold on him...");

	}
}
