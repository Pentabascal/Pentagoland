using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class CapitanDeMando : Subclass
{
	public override string Id => "capitan_mando";

	public override string Name => "Capitán de Mando";

	public override string Description => "Obliga al escuadrón a ir junto: 12s de SCP-1853 y 30 balas para todos los que están cerca.";

	public override RoleTypeId[] Roles => MtfSubclasses.Captains;

	public override float Chance => 10f;

	public override int MaxHealth => 120;

	public override float Cooldown => 240f;

	public override bool OverrideInventory => true;

	public override ItemType[] Items => new ItemType[7]
	{
		ItemType.GunFRMG0,
		ItemType.ArmorHeavy,
		ItemType.Radio,
		ItemType.KeycardMTFCaptain,
		ItemType.Medkit,
		ItemType.Medkit,
		ItemType.GrenadeHE
	};

	public override KeyValuePair<AmmoType, ushort>[] Ammo => new KeyValuePair<AmmoType, ushort>[1] { Subclass.A(AmmoType.Nato762, 200) };

	public override bool UseAbility(Player player)
	{
		int num = 0;
		foreach (Player item in SfUtils.AliveNear(player, 15f).Where(SfUtils.IsMtf))
		{
			item.EnableEffect(EffectType.Scp1853, 1, 12f);
			SfUtils.GiveAmmoForHeldWeapon(item, 30);
			if (item != player)
			{
				SfUtils.Hint(item, "<color=#8f8><b>Orden del Capitán.</b> 12s de rendimiento y munición.</color>");
			}
			num++;
		}
		SfUtils.Hint(player, $"<color=#8f8><b>Al frente.</b> Soldados reforzados: {num}.</color>");
		return true;
	}
}
