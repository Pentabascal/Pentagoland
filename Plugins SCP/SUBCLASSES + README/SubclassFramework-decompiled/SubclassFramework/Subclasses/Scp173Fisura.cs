using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class Scp173Fisura : Subclass
{
	public override string Id => "173_fisura";

	public override string Name => "SCP-173 Fisura";

	public override string Description => "Cuanta menos vida te queda, más daño recibes, pero también te mueves más rápido. Empiezas con algo menos de vida.";

	public override RoleTypeId[] Roles => Scp173Module.Scp173;

	public override float Chance => 10f;

	public override void OnAssigned(Player player)
	{
		int stamp = SubclassManager.Instance.GetAssignmentStamp(player);
		Timing.CallDelayed(1f, () =>
		{
			if (IsCurrentAssignment(player, stamp))
			{
				SfUtils.SetMaxHealth(player, player.MaxHealth * 0.92f, healToFull: true);
			}
		});
	}

	public override void OnTakingDamage(Player victim, HurtingEventArgs ev)
	{
		if (!(victim.MaxHealth <= 0f))
		{
			float num = (1f - victim.Health / victim.MaxHealth) * 100f;
			float num2 = Mathf.Clamp(num / 4f, 0f, 25f);
			ev.Amount *= 1f + num2 / 100f;
			victim.EnableEffect(EffectType.Burned, 1, 2f);
		}
	}

	public override void OnTick(Player player)
	{
		if (!(player.MaxHealth <= 0f))
		{
			float num = (1f - player.Health / player.MaxHealth) * 100f;
			float num2 = Mathf.Clamp(num / 10f * 1.5f, 0f, 15f);
			if (num2 >= 1f)
			{
				SfUtils.SpeedBoost(player, num2, 2f);
			}
		}
	}
}
