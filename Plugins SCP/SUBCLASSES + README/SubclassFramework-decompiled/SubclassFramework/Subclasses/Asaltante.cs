using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class Asaltante : Subclass
{
	private readonly Dictionary<Player, float> assaultUntil = new Dictionary<Player, float>();

	public override string Id => "asaltante";

	public override string Name => "Asaltante";

	public override string Description => "Abre una ventana de presión: durante unos segundos, tu primer impacto frena y pega más fuerte.";

	public override RoleTypeId[] Roles => ChaosSubclasses.Marauder;

	public override float Chance => 10f;

	public override int MaxHealth => 100;

	public override float Cooldown => 130f;

	public override ItemType[] ExtraItems => new ItemType[1] { ItemType.GrenadeFlash };

	public override string FailMessage => "No puedes preparar el asalto ahora mismo.";

	public override bool UseAbility(Player player)
	{
		assaultUntil[player] = Time.time + 8f;
		SfUtils.SpeedBoost(player, 12f, 8f);
		SfUtils.Hint(player, "<color=#f96><b>Asalto preparado.</b> Tu primer impacto será más contundente.</color>");
		return true;
	}

	public override void OnDealingDamage(Player attacker, HurtingEventArgs ev)
	{
		if (assaultUntil.TryGetValue(attacker, out var value) && !(Time.time > value))
		{
			assaultUntil.Remove(attacker);
			ev.Amount *= 1.25f;
			if (ev.Player != null && ev.Player.IsHuman)
			{
				ev.Player.EnableEffect(EffectType.Slowness, 15, 3f);
				SfUtils.Hint(ev.Player, "<color=#f96>Te han alcanzado durante un asalto.</color>", 3f);
			}
		}
	}

	public override void OnRemoved(Player player)
	{
		assaultUntil.Remove(player);
	}
}
