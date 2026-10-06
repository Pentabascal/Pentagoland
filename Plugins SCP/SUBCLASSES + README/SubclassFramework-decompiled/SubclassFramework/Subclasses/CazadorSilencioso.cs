using System;
using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class CazadorSilencioso : Subclass
{
	private readonly HashSet<ReferenceHub> capturesInProgress = new HashSet<ReferenceHub>();

	public override string Id => "106_cazador";

	public override string Name => "SCP-106 Cazador Silencioso";

	public override string Description => "Al golpear a un humano aplica una vez el ataque de SCP-106 y lo envía a la Pocket Dimension. Tiene algo menos de vida que el 106 normal.";

	public override RoleTypeId[] Roles => ScpSubclasses.Scp106;

	public override float Chance => 10f;

	public override void OnAssigned(Player player)
	{
		int stamp = SubclassManager.Instance.GetAssignmentStamp(player);
		Timing.CallDelayed(1f, () =>
		{
			if (IsCurrentAssignment(player, stamp))
			{
				SfUtils.SetMaxHealth(player, player.MaxHealth * 0.85f, healToFull: true);
			}
		});
	}

	public override void OnDealingDamage(Player attacker, HurtingEventArgs ev)
	{
		Player player = ev?.Player;
		if (ev == null || !ev.IsAllowed || attacker == null || !attacker.IsAlive || player == null || !player.IsAlive || !player.IsHuman || !(attacker.Role is Scp106Role scp106Role))
		{
			return;
		}

		ReferenceHub attackerHub = attacker.ReferenceHub;
		if (attackerHub == null || capturesInProgress.Contains(attackerHub))
		{
			return;
		}

		capturesInProgress.Add(attackerHub);
		try
		{
			ev.IsAllowed = false;
			scp106Role.CapturePlayer(player);
		}
		catch (Exception ex)
		{
			Log.Debug("[SubclassFramework] SCP-106 Cazador Silencioso no pudo aplicar el ataque: " + ex.Message);
		}
		finally
		{
			capturesInProgress.Remove(attackerHub);
		}
	}
}
