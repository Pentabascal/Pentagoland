using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class Sanitario : Subclass
{
	public override string Id => "sanitario";

	public override string Name => "Sanitario";

	public override string Description => "Guardia especializado en mantener con vida a sus compañeros.";

	public override RoleTypeId[] Roles => FacilitySubclasses.Guard;

	public override float Chance => 10f;

	public override int MaxPerRound => 1;

	public override float Cooldown => 150f;

	public override bool OverrideInventory => true;

	public override ItemType[] Items => new ItemType[6]
	{
		ItemType.GunCOM18,
		ItemType.ArmorLight,
		ItemType.KeycardGuard,
		ItemType.Medkit,
		ItemType.Medkit,
		ItemType.Medkit
	};

	public override KeyValuePair<AmmoType, ushort>[] Ammo => new KeyValuePair<AmmoType, ushort>[1] { Subclass.A(AmmoType.Nato9, 30) };

	public override bool UseAbility(Player player)
	{
		int num = 0;
		foreach (Player item in SfUtils.AliveNear(player, 6f).Where(SfUtils.IsGuard))
		{
			SfUtils.Heal(item, 30f);
			SfUtils.AddAhp(item, 10f, 100f, 0f, 15f);
			SfUtils.Hint(item, "<color=#8f8>El sanitario te ha curado.</color>", 4f);
			num++;
		}
		SfUtils.Hint(player, $"<color=#8f8><b>Curaci de zona.</b> Guardias atendidos: {num}.</color>");
		return true;
	}
}
