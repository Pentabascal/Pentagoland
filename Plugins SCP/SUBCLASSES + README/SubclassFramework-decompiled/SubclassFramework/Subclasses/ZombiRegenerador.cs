using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class ZombiRegenerador : Subclass
{
	private readonly Dictionary<Player, float> lastHit = new Dictionary<Player, float>();

	private readonly Dictionary<Player, float> nextHeal = new Dictionary<Player, float>();

	public override string Id => "zombi_regenerador";

	public override string Name => "Zombi Regenerador";

	public override string Description => "Tras 24 s sin recibir daño, te curas cada 10 s.";

	public override RoleTypeId[] Roles => ScpSubclasses.Scp0492;

	public override float Chance => 10f;

	public override void OnAssigned(Player player)
	{
		if (player == null)
		{
			return;
		}
		lastHit[player] = Time.time;
		nextHeal[player] = Time.time + 24f;
	}

	public override void OnTakingDamage(Player victim, HurtingEventArgs ev)
	{
		if (victim == null)
		{
			return;
		}
		lastHit[victim] = Time.time;
		nextHeal.Remove(victim);
	}

	public override void OnTick(Player player)
	{
		if (player == null || !player.IsAlive)
		{
			return;
		}
		if (!lastHit.TryGetValue(player, out var lastDamage))
		{
			lastHit[player] = Time.time;
			nextHeal[player] = Time.time + 24f;
			return;
		}
		if (Time.time - lastDamage < 24f || (nextHeal.TryGetValue(player, out var next) && Time.time < next))
		{
			return;
		}
		nextHeal[player] = Time.time + 10f;
		SfUtils.Heal(player, 10f);
	}

	public override void OnRemoved(Player player)
	{
		lastHit.Remove(player);
		nextHeal.Remove(player);
	}
}
