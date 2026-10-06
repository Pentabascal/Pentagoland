using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class ClaseDAmotinado : Subclass
{
	private readonly Dictionary<Player, Player> grudge = new Dictionary<Player, Player>();

	private readonly Dictionary<Player, float> grudgeUntil = new Dictionary<Player, float>();

	public override string Id => "clased_amotinado";

	public override string Name => "Clase D Amotinado";

	public override string Description => "Venganza: tras recibir un golpe de un guardia, tu siguiente ataque contra él hace un 15% más de daño.";

	public override RoleTypeId[] Roles => FacilitySubclasses.ClassD;

	public override float Chance => 10f;

	public override float Cooldown => 150f;

	public override string FailMessage => "Ningún guardia te ha tocado hace poco.";

	public override void OnTakingDamage(Player victim, HurtingEventArgs ev)
	{
		if (SfUtils.IsGuard(ev.Attacker))
		{
			grudge[victim] = ev.Attacker;
			grudgeUntil[victim] = Time.time + 10f;
		}
	}

	public override bool UseAbility(Player player)
	{
		if (!grudge.TryGetValue(player, out var value) || value == null || !grudgeUntil.TryGetValue(player, out var value2) || Time.time > value2)
		{
			return false;
		}
		grudgeUntil[player] = Time.time + 10f;
		SfUtils.Hint(player, "<color=#f84><b>Venganza preparada</b> contra " + value.Nickname + ".</color>");
		return true;
	}

	public override void OnDealingDamage(Player attacker, HurtingEventArgs ev)
	{
		if (grudge.TryGetValue(attacker, out var value) && !(value != ev.Player) && grudgeUntil.TryGetValue(attacker, out var value2) && !(Time.time > value2))
		{
			grudge.Remove(attacker);
			grudgeUntil.Remove(attacker);
			ev.Amount *= 1.15f;
		}
	}

	public override void OnRemoved(Player player)
	{
		grudge.Remove(player);
		grudgeUntil.Remove(player);
	}
}
