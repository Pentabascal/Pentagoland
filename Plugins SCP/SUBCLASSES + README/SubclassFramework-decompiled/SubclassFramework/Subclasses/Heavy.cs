using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class Heavy : Subclass
{
	public override string Id => "heavy";

	public override string Name => "Heavy";

	public override string Description => "Combatiente pesado con resistencia adicional y maniobra reducida.";

	public override RoleTypeId[] Roles => ChaosSubclasses.AllChaos;

	public override float Chance => 7f;

	public override int MaxPerWave => 1;

	public override int MaxHealth => 120;

	public override float Cooldown => 150f;

	public override float SpeedMultiplier => 0.87f;

	public override bool OverrideInventory => true;

	public override ItemType[] Items => new ItemType[6]
	{
		ItemType.GunLogicer,
		ItemType.KeycardChaosInsurgency,
		ItemType.ArmorHeavy,
		ItemType.Medkit,
		ItemType.Adrenaline,
		ItemType.GrenadeHE
	};

	public override KeyValuePair<AmmoType, ushort>[] Ammo => new KeyValuePair<AmmoType, ushort>[1] { Subclass.A(AmmoType.Nato762, 150) };

	public override bool UseAbility(Player player)
	{
		player.EnableEffect(EffectType.Scp1853, 1, 13f);
		SfUtils.Hint(player, "<color=#8f8><b>Liquido verde en vena.</b> 13 segundos de precision y recarga rapida.</color>");
		return true;
	}
}
