using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class Acechador : Subclass
{
	private readonly Dictionary<Player, float> lastShot = new Dictionary<Player, float>();

	private readonly Dictionary<Player, int> stacks = new Dictionary<Player, int>();

	public override string Id => "acechador";

	public override string Name => "Acechador";

	public override string Description => "Cada 7s sin disparar acumulas una carga (máximo 3). Haces más daño con la siguiente bala; con 3 cargas, además quemas al objetivo.";

	public override RoleTypeId[] Roles => ChaosSubclasses.Marauder;

	public override float Chance => 10f;

	public override int MaxPerWave => 1;

	public override int MaxHealth => 100;

	public override void OnAssigned(Player player)
	{
		lastShot[player] = Time.time;
		stacks[player] = 0;
	}

	public override void OnShooting(Player player, ShootingEventArgs ev)
	{
		lastShot[player] = Time.time;
	}

	public override void OnTick(Player player)
	{
		if (!lastShot.TryGetValue(player, out var value))
		{
			lastShot[player] = Time.time;
			return;
		}
		int num = (stacks.TryGetValue(player, out var value2) ? value2 : 0);
		int num2 = Mathf.Clamp(Mathf.FloorToInt((Time.time - value) / 7f), 0, 3);
		if (num2 != num)
		{
			stacks[player] = num2;
			if (num2 > num && num2 > 0)
			{
				SfUtils.Hint(player, $"<size=18><color=#c9f>Acecho: {num2}/3</color></size>", 2f);
			}
		}
	}

	public override void OnDealingDamage(Player attacker, HurtingEventArgs ev)
	{
		int num = (stacks.TryGetValue(attacker, out var value) ? value : 0);
		if (num > 0)
		{
			ev.Amount *= 1f + 0.1f * (float)num;
			if (num >= 3 && ev.Player != null)
			{
				ev.Player.EnableEffect(EffectType.Burned, 1, 4f);
			}
			stacks[attacker] = 0;
			lastShot[attacker] = Time.time;
		}
	}

	public override void OnRemoved(Player player)
	{
		lastShot.Remove(player);
		stacks.Remove(player);
	}
}
