using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class Juggernaut : Subclass
{
	private readonly Dictionary<Player, float> bunkerUntil = new Dictionary<Player, float>();

	private readonly Dictionary<Player, float> vulnerableUntil = new Dictionary<Player, float>();

	public override string Id => "juggernaut";

	public override string Name => "Juggernaut";

	public override string Description => "Operativo pesado con gran resistencia y movilidad reducida.";

	public override RoleTypeId[] Roles => MtfSubclasses.AllMtf;

	public override float Chance => 7f;

	public override int MaxHealth => 125;

	public override float Cooldown => 120f;

	public override float SpeedMultiplier => 0.85f;

	public override float SpeedDelay => 10f;

	public override bool OverrideInventory => true;

	public override ItemType[] Items => new ItemType[6]
	{
		ItemType.GunLogicer,
		ItemType.ArmorHeavy,
		ItemType.Radio,
		ItemType.KeycardMTFOperative,
		ItemType.Medkit,
		ItemType.Medkit
	};

	public override KeyValuePair<AmmoType, ushort>[] Ammo => new KeyValuePair<AmmoType, ushort>[1] { Subclass.A(AmmoType.Nato762, 200) };

	public override bool UseAbility(Player player)
	{
		SfUtils.Heal(player, player.MaxHealth * 0.3f);
		SfUtils.AddAhp(player, 60f, 100f, 0f, 8f);
		bunkerUntil[player] = Time.time + 8f;
		SfUtils.Slow(player, 50f, 8f);
		SfUtils.Hint(player, "<color=#8cf><b>Modo bunker.</b> 8 segundos clavado al suelo.</color>", 6f);
		return true;
	}

	public override void OnTick(Player player)
	{
		if (bunkerUntil.TryGetValue(player, out var value) && !(Time.time < value))
		{
			bunkerUntil.Remove(player);
			vulnerableUntil[player] = Time.time + 3f;
			player.EnableEffect(EffectType.Burned, 1, 3f);
			SfUtils.Hint(player, "<color=#f88>Sobrecalentado: 3s recibiendo más daño.</color>", 4f);
		}
	}

	public override void OnDealingDamage(Player attacker, HurtingEventArgs ev)
	{
		if (bunkerUntil.TryGetValue(attacker, out var value) && !(Time.time > value) && ev.Player != null && SfUtils.Roll(7f))
		{
			ev.Player.EnableEffect(EffectType.Burned, 1, 2f);
		}
	}

	public override void OnTakingDamage(Player victim, HurtingEventArgs ev)
	{
		if (vulnerableUntil.TryGetValue(victim, out var value) && Time.time <= value)
		{
			ev.Amount *= 1.25f;
		}
	}

	public override void OnRemoved(Player player)
	{
		bunkerUntil.Remove(player);
		vulnerableUntil.Remove(player);
		player?.DisableEffect(EffectType.Slowness);
		player?.DisableEffect(EffectType.Burned);
	}
}
