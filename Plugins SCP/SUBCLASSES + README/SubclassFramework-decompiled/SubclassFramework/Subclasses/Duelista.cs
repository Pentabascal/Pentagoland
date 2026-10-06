using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class Duelista : Subclass
{
	private readonly Dictionary<Player, float> empowerUntil = new Dictionary<Player, float>();

	private readonly Dictionary<Player, float> guardUntil = new Dictionary<Player, float>();

	public override string Id => "duelista";

	public override string Name => "Duelista";

	public override string Description => "Adopta una postura de duelo durante 6s: resiste parte del daño y su siguiente impacto inflige un 15% más.";

	public override RoleTypeId[] Roles => ChaosSubclasses.Rifleman;

	public override float Chance => 10f;

	public override int MaxHealth => 95;

	public override float Cooldown => 150f;

	public override bool OverrideInventory => true;

	public override ItemType[] Items => new ItemType[6]
	{
		ItemType.GunCrossvec,
		ItemType.GunCOM15,
		ItemType.ArmorCombat,
		ItemType.KeycardChaosInsurgency,
		ItemType.Medkit,
		ItemType.Adrenaline
	};

	public override KeyValuePair<AmmoType, ushort>[] Ammo => new KeyValuePair<AmmoType, ushort>[1] { Subclass.A(AmmoType.Nato9, 100) };

	public override bool UseAbility(Player player)
	{
		empowerUntil[player] = Time.time + 6f;
		guardUntil[player] = Time.time + 6f;
		SfUtils.Hint(player, "<color=#fa6><b>Postura de duelo.</b> Durante 6s recibes menos daño y tu siguiente impacto pega un 15% más.</color>");
		return true;
	}

	public override void OnDealingDamage(Player attacker, HurtingEventArgs ev)
	{
		if (empowerUntil.TryGetValue(attacker, out var value) && !(Time.time > value))
		{
			empowerUntil.Remove(attacker);
			ev.Amount *= 1.15f;
		}
	}

	public override void OnTakingDamage(Player victim, HurtingEventArgs ev)
	{
		if (guardUntil.TryGetValue(victim, out var value))
		{
			if (Time.time > value)
			{
				guardUntil.Remove(victim);
			}
			else
			{
				ev.Amount *= 0.8f;
			}
		}
	}

	public override void OnRemoved(Player player)
	{
		empowerUntil.Remove(player);
		guardUntil.Remove(player);
	}
}
