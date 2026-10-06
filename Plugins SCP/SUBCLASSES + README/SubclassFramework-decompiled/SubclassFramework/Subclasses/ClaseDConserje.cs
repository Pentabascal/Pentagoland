using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Hazards;
using MEC;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class ClaseDConserje : Subclass
{
	private readonly Dictionary<Player, float> cleaningUntil = new Dictionary<Player, float>();

	private const float CleanDuration = 3f;

	private const float CleanDistance = 4f;

	public override string Id => "clased_conserje";

	public override string Name => "Clase D Conserje";

	public override string Description => "Limpia charcos de SCP-173 y empieza en los banos de LCZ.";

	public override RoleTypeId[] Roles => FacilitySubclasses.ClassD;

	public override float Chance => 10f;

	public override int MaxPerRound => 1;

	public override float Cooldown => 90f;

	public override bool OverrideInventory => true;

	public override ItemType[] Items => new ItemType[3]
	{
		ItemType.KeycardJanitor,
		ItemType.Flashlight,
		ItemType.Painkillers
	};

	public override string FailMessage => "Debes estar junto a un charco de SCP-173.";

	public override void OnAssigned(Player player)
	{
		int stamp = SubclassManager.Instance.GetAssignmentStamp(player);
		Timing.CallDelayed(0.2f, () =>
		{
			if (IsCurrentAssignment(player, stamp) && player.Role.Type == RoleTypeId.ClassD)
			{
				Room room = Room.List.FirstOrDefault((Room r) => r != null && r.Type == RoomType.LczToilets);
				SfUtils.TryTeleportToRoom(player, room);
			}
		});
	}

	public override bool UseAbility(Player player)
	{
		if (player == null || FindNearbyTantrum(player) == null)
		{
			return false;
		}
		cleaningUntil[player] = Time.time + 3f;
		SfUtils.Hint(player, "<color=#9fd><b>Limpieza iniciada</b>... no te alejes del charco.</color>", 3.5f);
		return true;
	}

	public override void OnTick(Player player)
	{
		if (!cleaningUntil.TryGetValue(player, out var value))
		{
			return;
		}
		TantrumHazard tantrumHazard = FindNearbyTantrum(player);
		if (tantrumHazard == null)
		{
			cleaningUntil.Remove(player);
			SfUtils.Hint(player, "<color=#f88>La limpieza se ha cancelado: el charco ya no está cerca.</color>", 3f);
			return;
		}
		float num = Mathf.Max(0f, value - Time.time);
		float num2 = Mathf.Clamp01(1f - num / 3f);
		int num3 = Mathf.Clamp(Mathf.RoundToInt(num2 * 10f), 0, 10);
		string arg = new string('|', num3) + new string('.', 10 - num3);
		SfUtils.HudHint(player, $"<color=#9fd><b>Limpiando:</b> [{arg}] {Mathf.CeilToInt(num)}s</color>", 1.2f);
		if (!(num > 0f))
		{
			Object.Destroy((Object)(object)tantrumHazard.Base);
			cleaningUntil.Remove(player);
			SfUtils.Hint(player, "<color=#9f9><b>Charco limpiado.</b></color>", 4f);
		}
	}

	public override void OnRemoved(Player player)
	{
		cleaningUntil.Remove(player);
	}

	private static TantrumHazard FindNearbyTantrum(Player player)
	{
		return (from h in Hazard.Get(HazardType.Tantrum).OfType<TantrumHazard>()
			where h != null && h.IsActive && (h.Position - player.Position).sqrMagnitude <= 16f
			orderby (h.Position - player.Position).sqrMagnitude
			select h).FirstOrDefault();
	}
}
