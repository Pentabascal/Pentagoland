using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class Trampero : Subclass
{
	private readonly Dictionary<Player, float> preparedUntil = new Dictionary<Player, float>();

	public override string Id => "trampero";

	public override string Name => "Trampero";

	public override string Description => "Prepara una emboscada durante 8s. Tu siguiente impacto ralentiza al objetivo un 15% durante 3s.";

	public override RoleTypeId[] Roles => ChaosSubclasses.Marauder;

	public override float Chance => 10f;

	public override int MaxPerWave => 1;

	public override int MaxHealth => 100;

	public override float Cooldown => 100f;

	public override bool UseAbility(Player player)
	{
		preparedUntil[player] = Time.time + 8f;
		SfUtils.Hint(player, "<color=#9f9><b>Emboscada preparada.</b> Tu siguiente impacto ralentizara al objetivo.</color>", 4f);
		return true;
	}

	public override void OnDealingDamage(Player attacker, HurtingEventArgs ev)
	{
		if (!preparedUntil.TryGetValue(attacker, out var value))
		{
			return;
		}
		preparedUntil.Remove(attacker);
		if (!(Time.time > value) && !(ev.Player == null))
		{
			SfUtils.Slow(ev.Player, 15f, 3f);
			SfUtils.Hint(attacker, "<color=#9f9>Emboscada activada: objetivo ralentizado.</color>", 4f);
			if (ev.Player.IsConnected)
			{
				SfUtils.Hint(ev.Player, "<color=#f84>Has sido alcanzado por una emboscada y te mueves más despacio.</color>", 4f);
			}
		}
	}

	public override void OnRemoved(Player player)
	{
		preparedUntil.Remove(player);
	}
}
