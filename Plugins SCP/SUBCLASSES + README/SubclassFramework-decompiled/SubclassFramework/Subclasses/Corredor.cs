using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class Corredor : Subclass
{
	private readonly Dictionary<Player, float> sprintUntil = new Dictionary<Player, float>();

	public override string Id => "corredor";

	public override string Name => "Corredor";

	public override string Description => "Corre a gran velocidad durante 7 segundos, pero recibe un 25% más de daño mientras dura.";

	public override RoleTypeId[] Roles => MtfSubclasses.Privates;

	public override float Chance => 10f;

	public override int MaxHealth => 100;

	public override float Cooldown => 150f;

	public override bool OverrideInventory => true;

	public override ItemType[] Items => new ItemType[5]
	{
		ItemType.GunCrossvec,
		ItemType.ArmorLight,
		ItemType.Medkit,
		ItemType.KeycardMTFOperative,
		ItemType.Radio
	};

	public override KeyValuePair<AmmoType, ushort>[] Ammo => new KeyValuePair<AmmoType, ushort>[1] { Subclass.A(AmmoType.Nato9, 120) };

	public override void OnTick(Player player)
	{
		if (Time.time % 5f < 1.05f)
		{
			SfUtils.RefillStamina(player);
		}
		if (sprintUntil.TryGetValue(player, out var value) && !(Time.time < value))
		{
			sprintUntil.Remove(player);
			player.DisableEffect(EffectType.MovementBoost);
			SfUtils.Slow(player, 10f, 3f);
			SfUtils.Hint(player, "<color=#f88>Se te acaba el fuelle.</color>", 3f);
		}
	}

	public override bool UseAbility(Player player)
	{
		sprintUntil[player] = Time.time + 7f;
		SfUtils.SpeedBoost(player, 20f, 7f);
		player.EnableEffect(EffectType.Burned, 1, 7f);
		SfUtils.Hint(player, "<color=#fa6><b>Esprint.</b> 7s a tope, pero eres de papel.</color>");
		return true;
	}

	public override void OnTakingDamage(Player victim, HurtingEventArgs ev)
	{
		if (sprintUntil.TryGetValue(victim, out var value) && Time.time <= value)
		{
			ev.Amount *= 1.25f;
		}
	}

	public override void OnRemoved(Player player)
	{
		sprintUntil.Remove(player);
		player?.DisableEffect(EffectType.MovementBoost);
	}
}
