using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using UnityEngine;

namespace SubclassFramework.Core;

public static class GlobalHooks
{
	private sealed class Mark
	{
		public float Expiry;

		public float Multiplier;

		public Team AttackerTeam;
	}

	private static readonly Dictionary<Player, Mark> Marks = new Dictionary<Player, Mark>();

	private static readonly Dictionary<Player, float> Corrupted = new Dictionary<Player, float>();

	private static readonly Dictionary<Player, float> LastHealth = new Dictionary<Player, float>();

	private static readonly Dictionary<Player, float> LastArtificialHealth = new Dictionary<Player, float>();

	public static void MarkTarget(Player target, float duration, float multiplier, Team attackerTeam)
	{
		if (!(target == null))
		{
			Marks[target] = new Mark
			{
				Expiry = Time.time + duration,
				Multiplier = multiplier,
				AttackerTeam = attackerTeam
			};
		}
	}

	public static bool IsMarked(Player target)
	{
		Mark value;
		return target != null && Marks.TryGetValue(target, out value) && Time.time <= value.Expiry;
	}

	public static void Corrupt(Player target, float duration)
	{
		if (!(target == null))
		{
			Corrupted[target] = Time.time + duration;
			LastHealth[target] = target.Health;
			LastArtificialHealth[target] = target.ArtificialHealth;
		}
	}

	public static bool IsCorrupted(Player target)
	{
		float value;
		return target != null && Corrupted.TryGetValue(target, out value) && Time.time <= value;
	}

	public static void ClearPlayer(Player player)
	{
		if (!(player == null))
		{
			Marks.Remove(player);
			Corrupted.Remove(player);
			LastHealth.Remove(player);
			LastArtificialHealth.Remove(player);
		}
	}

	public static void ApplyCorruptionOnHeal(Player target, float healedHealth, float healedAhp)
	{
		if (IsCorrupted(target) && (!(healedHealth <= 0f) || !(healedAhp <= 0f)))
		{
			if (healedHealth > 0f)
			{
				target.Health = Mathf.Max(1f, target.Health - healedHealth * 0.5f);
			}
			if (healedAhp > 0f)
			{
				target.ArtificialHealth = Mathf.Max(0f, target.ArtificialHealth - healedAhp * 0.5f);
			}
			SfUtils.Hint(target, "<color=#a6f>La corrupción frena tu curación.</color>", 3f);
		}
	}

	public static void OnHurting(HurtingEventArgs ev)
	{
		if (ev?.Player == null || ev.Attacker == null)
		{
			return;
		}

		if (!Marks.TryGetValue(ev.Player, out var value))
		{
			return;
		}

		if (Time.time > value.Expiry)
		{
			Marks.Remove(ev.Player);
			return;
		}

		if (ev.Attacker.Role == null || ev.Attacker.Role.Team != value.AttackerTeam)
		{
			return;
		}

		if (value.Multiplier <= 1f)
		{
			return;
		}

		ev.Amount *= Mathf.Clamp(value.Multiplier, 1f, 3f);
	}

	public static void Tick()
	{
		foreach (Player item in (from kv in Marks
			where Time.time > kv.Value.Expiry
			select kv.Key).ToList())
		{
			Marks.Remove(item);
		}
		foreach (Player item2 in (from kv in Corrupted
			where Time.time > kv.Value
			select kv.Key).ToList())
		{
			Corrupted.Remove(item2);
			LastHealth.Remove(item2);
			LastArtificialHealth.Remove(item2);
		}
		foreach (Player item3 in Corrupted.Keys.ToList())
		{
			if (item3 == null || !item3.IsAlive)
			{
				Corrupted.Remove(item3);
				LastHealth.Remove(item3);
				LastArtificialHealth.Remove(item3);
				continue;
			}
			float health = item3.Health;
			float artificialHealth = item3.ArtificialHealth;
			float healedHealth = ((LastHealth.TryGetValue(item3, out var value) && health > value) ? (health - value) : 0f);
			float healedAhp = ((LastArtificialHealth.TryGetValue(item3, out var value2) && artificialHealth > value2) ? (artificialHealth - value2) : 0f);
			ApplyCorruptionOnHeal(item3, healedHealth, healedAhp);
			LastHealth[item3] = item3.Health;
			LastArtificialHealth[item3] = item3.ArtificialHealth;
		}
	}

	public static void Clear()
	{
		Marks.Clear();
		Corrupted.Clear();
		LastHealth.Clear();
		LastArtificialHealth.Clear();
	}
}
