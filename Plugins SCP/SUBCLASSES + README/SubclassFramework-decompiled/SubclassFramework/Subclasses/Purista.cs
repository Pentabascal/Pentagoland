using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class Purista : Subclass
{
	private static readonly HashSet<Ragdoll> PurifiedCorpses = new HashSet<Ragdoll>();

	private readonly Dictionary<Player, float> resistanceExpiresAt = new Dictionary<Player, float>();

	public override string Id => "049_purista";

	public override string Name => "SCP-049 Purista";

	public override string Description => "Tiene 2600 HP y +10% de velocidad. Tras una baja, obtiene 20% de resistencia durante 6 s y evita la reanimación de la víctima.";

	public override RoleTypeId[] Roles => ScpSubclasses.Scp049;

	public override float Chance => 10f;

	public override int MaxHealth => 2600;

	public override float SpeedMultiplier => 1.1f;

	public override void OnKill(Player killer, Player victim, DiedEventArgs ev)
	{
		if (killer == null || victim == null || !victim.IsHuman)
		{
			return;
		}
		resistanceExpiresAt[killer] = Time.time + 6f;
		Vector3 where = victim.Position;
		try
		{
			Ragdoll ragdoll = Ragdoll.List.FirstOrDefault((Ragdoll r) => r != null && r.Owner == victim);
			if (ragdoll != null)
			{
				where = ragdoll.Position;
			}
		}
		catch (Exception ex)
		{
			Log.Debug("[SubclassFramework] Purista (ragdoll): " + ex.Message);
		}
		DestroyVictimCorpses(victim, where);
		Timing.CallDelayed(0.1f, () =>
		{
			DestroyVictimCorpses(victim, where);
		});
		SfUtils.Hint(killer, "<color=#a6f>La cura ha sido definitiva.</color>", 3.5f);
	}

	public override void OnTakingDamage(Player victim, HurtingEventArgs ev)
	{
		if (resistanceExpiresAt.TryGetValue(victim, out float expiry) && Time.time < expiry)
		{
			ev.Amount *= 0.8f;
		}
	}

	public override void OnRemoved(Player player)
	{
		resistanceExpiresAt.Remove(player);
	}

	public static void Block(Ragdoll corpse)
	{
		if (corpse != null)
		{
			PurifiedCorpses.Add(corpse);
		}
	}

	public static bool IsBlocked(Ragdoll corpse)
	{
		return corpse != null && PurifiedCorpses.Contains(corpse);
	}

	public static void ClearAll()
	{
		PurifiedCorpses.Clear();
	}

	public static void DestroyVictimCorpses(Player victim, Vector3 fallbackPosition)
	{
		try
		{
			List<Ragdoll> list = Ragdoll.List.Where((Ragdoll r) => r != null && ((victim != null && r.Owner == victim) || (victim == null && (r.Position - fallbackPosition).sqrMagnitude <= 1f))).ToList();
			foreach (Ragdoll item in list)
			{
				Block(item);
				item.Destroy();
			}
		}
		catch (Exception ex)
		{
			Log.Debug("[SubclassFramework] Purista: " + ex.Message);
		}
	}
}
