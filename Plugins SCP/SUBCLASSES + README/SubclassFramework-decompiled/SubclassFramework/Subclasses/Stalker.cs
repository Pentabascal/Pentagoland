using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using LabApi.Events.Arguments.Scp106Events;
using MEC;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class Stalker : Subclass
{
	private const float VigorCost = 0.5f;
	private const float SubmergeTimeout = 5f;
	private const float SubmergedStabilityDelay = 1f;
	private static readonly HashSet<ReferenceHub> LockedStalkers = new HashSet<ReferenceHub>();

	public override string Id => "106_stalker";

	public override string Name => "SCP-106 Stalker";

	public override string Description => "Se hunde bajo el suelo y aparece sumergido junto a una persona aleatoria. Consume 50% de vigor.";

	public override RoleTypeId[] Roles => ScpSubclasses.Scp106;

	public override float Chance => 10f;

	public override float Cooldown => 180f;

	public override string FailMessage => "Necesitas al menos 50% de vigor y un objetivo vivo para usar la habilidad.";

	public override bool UseAbility(Player player)
	{
		if (player == null || !player.IsAlive || player.Role is not Scp106Role role || role.Vigor < VigorCost)
		{
			return false;
		}

		Player target = GetRandomTarget(player);
		if (target == null)
		{
			return false;
		}

		Vector3 origin = player.Position;
		int assignmentStamp = SubclassManager.Instance.GetAssignmentStamp(player);
		LockedStalkers.Add(player.ReferenceHub);
		role.IsStalking = true;
		SfUtils.Hint(player, "<color=#7a5c3e><b>Stalker:</b> te sumerges antes del salto.</color>", 6f);
		Timing.RunCoroutine(CompleteTeleport(player, this, target, origin, assignmentStamp));
		return true;
	}

	public static void OnChangingStalkMode(Scp106ChangingStalkModeEventArgs ev)
	{
		if (ev?.Player?.ReferenceHub != null && !ev.IsStalkActive && LockedStalkers.Contains(ev.Player.ReferenceHub))
		{
			ev.IsAllowed = false;
		}
	}

	public static void ClearAll()
	{
		LockedStalkers.Clear();
	}

	public static void RemoveLock(Player player)
	{
		if (player != null)
		{
			LockedStalkers.Remove(player.ReferenceHub);
		}
	}

	private static IEnumerator<float> CompleteTeleport(Player player, Subclass expectedSubclass, Player target, Vector3 origin, int assignmentStamp)
	{
		float deadline = Time.time + SubmergeTimeout;
		Scp106Role role = player.Role as Scp106Role;

		while (Time.time < deadline)
		{
			if (!player.IsAlive || player.Role is not Scp106Role currentRole ||
				SubclassManager.Instance == null ||
				!SubclassManager.Instance.IsCurrentAssignment(player, expectedSubclass, assignmentStamp))
			{
				Abort(player, role, origin, false);
				yield break;
			}

			role = currentRole;
			if (!role.IsStalking)
			{
				role.IsStalking = true;
			}
			if (role.Vigor < VigorCost)
			{
				Abort(player, role, origin, true);
				SfUtils.Hint(player, "<color=#f88>No tienes vigor suficiente para completar el salto.</color>", 6f);
				yield break;
			}
			if (role.SinkholeController.TargetSubmerged && !role.SinkholeController.IsDuringAnimation)
			{
				break;
			}

			yield return Timing.WaitForSeconds(0.05f);
		}

		if (role == null || !role.SinkholeController.TargetSubmerged || role.SinkholeController.IsDuringAnimation)
		{
			Abort(player, role, origin, true);
			SfUtils.Hint(player, "<color=#f88>No se pudo completar la inmersión; vuelves a emerger.</color>", 6f);
			yield break;
		}

		float stableUntil = Time.time + SubmergedStabilityDelay;
		float stabilizationDeadline = Time.time + SubmergeTimeout;
		while (Time.time < stableUntil && Time.time < stabilizationDeadline)
		{
			if (!player.IsAlive || player.Role is not Scp106Role currentRole ||
				SubclassManager.Instance == null ||
				!SubclassManager.Instance.IsCurrentAssignment(player, expectedSubclass, assignmentStamp))
			{
				Abort(player, role, origin, false);
				yield break;
			}

			role = currentRole;
			if (role.Vigor < VigorCost)
			{
				Abort(player, role, origin, true);
				SfUtils.Hint(player, "<color=#f88>No tienes vigor suficiente para completar el salto.</color>", 6f);
				yield break;
			}
			if (!role.IsStalking)
			{
				role.IsStalking = true;
			}
			if (!role.SinkholeController.TargetSubmerged || role.SinkholeController.IsDuringAnimation)
			{
				stableUntil = Time.time + SubmergedStabilityDelay;
			}

			yield return Timing.WaitForSeconds(0.05f);
		}

		if (Time.time < stableUntil || !player.IsAlive || player.Role is not Scp106Role || role.Vigor < VigorCost ||
			!role.SinkholeController.TargetSubmerged || role.SinkholeController.IsDuringAnimation)
		{
			Abort(player, role, origin, true);
			SfUtils.Hint(player, "<color=#f88>No se pudo mantener la inmersión; vuelves a emerger.</color>", 6f);
			yield break;
		}

		if (target == null || !target.IsAlive || target is Npc)
		{
			target = GetRandomTarget(player);
		}
		if (target == null)
		{
			Abort(player, role, origin, true);
			SfUtils.Hint(player, "<color=#f88>No queda nadie a quien alcanzar; vuelves a emerger.</color>", 6f);
			yield break;
		}

		try
		{
			player.Position = target.Position + Vector3.up * 1.5f;
			role.Vigor = Mathf.Max(0f, role.Vigor - VigorCost);
			LockedStalkers.Remove(player.ReferenceHub);
			if (role.Vigor <= 0f)
			{
				role.IsStalking = false;
			}
			SfUtils.Hint(player, "<color=#7a5c3e><b>Apareces sumergido junto a tu presa.</b></color>", 6f);
		}
		catch (System.Exception ex)
		{
			Abort(player, role, origin, true);
			Log.Debug("[SubclassFramework] SCP-106 Stalker no pudo completar el salto: " + ex.Message);
		}
	}

	private static Player GetRandomTarget(Player stalker)
	{
		List<Player> targets = Player.List
			.Where(player => player != null && !(player is Npc) && player != stalker && player.IsAlive)
			.ToList();
		return targets.Count == 0 ? null : targets[UnityEngine.Random.Range(0, targets.Count)];
	}

	private static void Abort(Player player, Scp106Role role, Vector3 origin, bool restorePosition)
	{
		if (player != null)
		{
			LockedStalkers.Remove(player.ReferenceHub);
		}
		if (role == null)
		{
			return;
		}
		if (restorePosition && player != null && player.IsAlive && player.Role is Scp106Role)
		{
			try
			{
				player.Position = origin;
			}
			catch (System.Exception ex)
			{
				Log.Debug("[SubclassFramework] SCP-106 Stalker no pudo restaurar la posición: " + ex.Message);
			}
		}
		try
		{
			role.IsStalking = false;
		}
		catch (System.Exception ex)
		{
			Log.Debug("[SubclassFramework] SCP-106 Stalker no pudo cancelar el acecho: " + ex.Message);
		}
	}
}
