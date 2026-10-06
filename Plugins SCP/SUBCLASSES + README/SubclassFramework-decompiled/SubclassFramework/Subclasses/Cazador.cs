using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class Cazador : Subclass
{
	public override string Id => "cazador";

	public override string Name => "Cazador";

	public override string Description => "Hace un 15% más de daño a cualquier objetivo que tenga menos del 50% de su vida.";

	public override RoleTypeId[] Roles => ChaosSubclasses.Rifleman;

	public override float Chance => 10f;

	public override int MaxHealth => 100;

	public override bool OverrideInventory => true;

	public override ItemType[] Items => new ItemType[6]
	{
		ItemType.GunAK,
		ItemType.ArmorCombat,
		ItemType.Medkit,
		ItemType.KeycardChaosInsurgency,
		ItemType.GunShotgun,
		ItemType.GrenadeFlash
	};

	public override KeyValuePair<AmmoType, ushort>[] Ammo => new KeyValuePair<AmmoType, ushort>[2]
	{
		Subclass.A(AmmoType.Nato762, 140),
		Subclass.A(AmmoType.Ammo12Gauge, 32)
	};

	public override void OnDealingDamage(Player attacker, HurtingEventArgs ev)
	{
		Player player = ev.Player;
		if (!(player == null) && !(player.MaxHealth <= 0f) && player.Health < player.MaxHealth * 0.5f)
		{
			ev.Amount *= 1.15f;
		}
	}
}
