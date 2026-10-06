using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class GuardianDeLaFosa : Subclass
{
	private readonly Dictionary<Player, float> shieldedUntil = new Dictionary<Player, float>();

	public override string Id => "106_guardian";

	public override string Name => "SCP-106 Guardián de la Fosa";

	public override string Description => "Mucho más duro y con una ventana de resistencia tras capturar, pero pierde movilidad.";

	public override RoleTypeId[] Roles => ScpSubclasses.Scp106;

	public override float Chance => 10f;

	public override float SpeedMultiplier => 0.91f;

	public override void OnAssigned(Player player)
	{
		int stamp = SubclassManager.Instance.GetAssignmentStamp(player);
		Timing.CallDelayed(1f, () =>
		{
			if (IsCurrentAssignment(player, stamp))
			{
				SfUtils.SetMaxHealth(player, player.MaxHealth + 400f, healToFull: true);
			}
		});
	}

	public override void OnDealingDamage(Player attacker, HurtingEventArgs ev)
	{
		shieldedUntil[attacker] = Time.time + 4f;
	}

	public override void OnTakingDamage(Player victim, HurtingEventArgs ev)
	{
		if (shieldedUntil.TryGetValue(victim, out var value) && Time.time <= value)
		{
			ev.Amount *= 0.75f;
		}
	}

	public override void OnRemoved(Player player)
	{
		shieldedUntil.Remove(player);
	}
}
