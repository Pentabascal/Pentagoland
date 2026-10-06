using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class Devorador : Subclass
{
	public override string Id => "939_devorador";

	public override string Name => "SCP-939 Devorador";

	public override string Description => "Puede devorar cadáveres para recuperar 75 de vida, pero queda inmóvil durante 5 segundos y tiene 150 de vida máxima menos.";

	public override RoleTypeId[] Roles => ScpSubclasses.Scp939;

	public override float Chance => 10f;

	public override float Cooldown => 110f;

	public override string FailMessage => "No hay ningún cadáver lo bastante cerca.";

	public override void OnAssigned(Player player)
	{
		int stamp = SubclassManager.Instance.GetAssignmentStamp(player);
		Timing.CallDelayed(1f, () =>
		{
			if (IsCurrentAssignment(player, stamp))
			{
				SfUtils.SetMaxHealth(player, Mathf.Max(100f, player.MaxHealth - 150f), healToFull: true);
			}
		});
	}

	public override bool UseAbility(Player player)
	{
		Ragdoll ragdoll;
		try
		{
			ragdoll = (from r in Ragdoll.List
				where r != null && (r.Position - player.Position).sqrMagnitude <= 9f
				orderby (r.Position - player.Position).sqrMagnitude
				select r).FirstOrDefault();
		}
		catch
		{
			ragdoll = null;
		}
		if (ragdoll == null)
		{
			return false;
		}
		try
		{
			ragdoll.Destroy();
		}
		catch
		{
		}
		SfUtils.Heal(player, 75f);
		player.EnableEffect(EffectType.Ensnared, 1, 5f);
		SfUtils.Hint(player, "<color=#f88><b>Devorando.</b> 5 segundos inmóvil. +75 de vida.</color>", 6f);
		return true;
	}
}
