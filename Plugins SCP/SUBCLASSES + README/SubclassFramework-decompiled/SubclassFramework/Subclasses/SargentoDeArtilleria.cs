using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class SargentoDeArtilleria : Subclass
{
	public override string Id => "sargento_artilleria";

	public override string Name => "Sargento de Artillería";

	public override string Description => "Reparte 30 balas para el arma que lleve cada MTF de su sala.";

	public override RoleTypeId[] Roles => MtfSubclasses.Sergeants;

	public override float Chance => 10f;

	public override int MaxHealth => 105;

	public override float Cooldown => 180f;

	public override bool OverrideInventory => true;

	public override ItemType[] Items => new ItemType[4]
	{
		ItemType.GunE11SR,
		ItemType.ArmorCombat,
		ItemType.Radio,
		ItemType.KeycardMTFOperative
	};

	public override KeyValuePair<AmmoType, ushort>[] Ammo => new KeyValuePair<AmmoType, ushort>[1] { Subclass.A(AmmoType.Nato556, 150) };

	public override bool UseAbility(Player player)
	{
		int num = 0;
		foreach (Player item in SfUtils.InSameRoom(player).Where(SfUtils.IsMtf))
		{
			SfUtils.GiveAmmoForHeldWeapon(item, 30);
			if (item != player)
			{
				SfUtils.Hint(item, "<color=#8cf>+30 balas para tu arma.</color>", 4f);
			}
			num++;
		}
		SfUtils.Hint(player, $"<color=#8cf><b>Reparto de munición</b> a {num} soldados.</color>");
		return true;
	}
}
