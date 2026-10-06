using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class Verdugo : Subclass
{
	private readonly Dictionary<Player, float> nextExecution = new Dictionary<Player, float>();

	public override string Id => "verdugo";

	public override string Name => "Verdugo";

	public override string Description => "Contra humanos por debajo del 35% de vida, un disparo hace un 50% más de daño una vez cada 40 segundos.";

	public override RoleTypeId[] Roles => ChaosSubclasses.Repressor;

	public override float Chance => 10f;

	public override int MaxHealth => 95;

	public override void OnDealingDamage(Player attacker, HurtingEventArgs ev)
	{
		Player player = ev.Player;
		if (!(player == null) && player.IsHuman && !(player.Health > player.MaxHealth * 0.35f) && (!nextExecution.TryGetValue(attacker, out var value) || !(Time.time < value)))
		{
			nextExecution[attacker] = Time.time + 40f;
			ev.Amount *= 1.5f;
			SfUtils.Hint(attacker, "<color=#f44><b>Ejecucion.</b></color>", 2.5f);
		}
	}

	public override void OnRemoved(Player player)
	{
		nextExecution.Remove(player);
	}
}
