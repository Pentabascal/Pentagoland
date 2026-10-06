using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class CaosUltimoRecurso : Subclass
{
	private const float Threshold = 0.25f;

	private const float Duration = 10f;

	private const float Boost = 15f;

	private const float InternalCooldown = 90f;

	private float activeUntil;

	private float nextReady;

	private bool activated;

	public override string Id => "caos_ultimo_recurso";

	public override string Name => "Último Recurso";

	public override string Description => "La primera vez que bajas del 25% de vida, corres más rápido y recuperas stamina durante 10 segundos.";

	public override RoleTypeId[] Roles => ChaosSubclasses.AllChaos;

	public override float Chance => 10f;

	public override int MaxPerRound => 2;

	public override void OnAssigned(Player player)
	{
		activeUntil = 0f;
		nextReady = Time.time;
		activated = false;
	}

	public override void OnTick(Player player)
	{
		if (player == null || !player.IsAlive)
		{
			return;
		}
		if (activeUntil > 0f)
		{
			if (Time.time < activeUntil)
			{
				SfUtils.RefillStamina(player);
				SfUtils.SpeedBoost(player, 15f, 2f);
				return;
			}
			activeUntil = 0f;
		}
		if (!activated && !(Time.time < nextReady) && !(player.MaxHealth <= 0f) && player.Health <= player.MaxHealth * 0.25f)
		{
			activated = true;
			nextReady = Time.time + 90f;
			activeUntil = Time.time + 10f;
			SfUtils.SpeedBoost(player, 15f, 10f);
			SfUtils.RefillStamina(player);
			SfUtils.Hint(player, "<color=#f6a><b>Ultimo recurso.</b> 10 segundos para escapar.</color>");
		}
	}

	public override void OnRemoved(Player player)
	{
		activeUntil = 0f;
		activated = false;
	}
}
