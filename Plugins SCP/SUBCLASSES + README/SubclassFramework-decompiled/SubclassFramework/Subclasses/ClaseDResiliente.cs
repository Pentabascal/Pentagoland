using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class ClaseDResiliente : Subclass
{
	private readonly Dictionary<Player, float> lastHit = new Dictionary<Player, float>();

	private readonly Dictionary<Player, float> nextRegen = new Dictionary<Player, float>();

	public override string Id => "clased_resiliente";

	public override string Name => "Clase D Resiliente";

	public override string Description => "Si pasas 15 segundos sin recibir daño, regeneras 1 punto de vida cada 10 segundos.";

	public override RoleTypeId[] Roles => FacilitySubclasses.ClassD;

	public override float Chance => 10f;

	public override void OnAssigned(Player player)
	{
		lastHit[player] = Time.time;
	}

	public override void OnTakingDamage(Player victim, HurtingEventArgs ev)
	{
		lastHit[victim] = Time.time;
	}

	public override void OnTick(Player player)
	{
		float value2;
		if (!lastHit.TryGetValue(player, out var value))
		{
			lastHit[player] = Time.time;
		}
		else if (!(Time.time - value < 15f) && (!nextRegen.TryGetValue(player, out value2) || !(Time.time < value2)))
		{
			nextRegen[player] = Time.time + 10f;
			if (player.Health < player.MaxHealth)
			{
				SfUtils.Heal(player, 1f);
			}
		}
	}

	public override void OnRemoved(Player player)
	{
		lastHit.Remove(player);
		nextRegen.Remove(player);
	}
}
