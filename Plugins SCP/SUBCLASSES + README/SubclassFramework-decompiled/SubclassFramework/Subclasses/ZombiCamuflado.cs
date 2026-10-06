using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class ZombiCamuflado : Subclass
{
	private readonly Dictionary<Player, Vector3> lastPosition = new Dictionary<Player, Vector3>();

	private readonly Dictionary<Player, float> stillSince = new Dictionary<Player, float>();

	private readonly Dictionary<Player, float> nextCamo = new Dictionary<Player, float>();

	private readonly HashSet<Player> hidden = new HashSet<Player>();

	private readonly Dictionary<Player, float> ambushUntil = new Dictionary<Player, float>();

	public override string Id => "zombi_camuflado";

	public override string Name => "Zombi Camuflado";

	public override string Description => "Si te quedas quieto junto a cadáveres, pasarás por uno más. Al moverte, tu primer golpe inflige un 20% más de daño, pero tienes menos vida.";

	public override RoleTypeId[] Roles => ScpSubclasses.Scp0492;

	public override float Chance => 10f;

	public override void OnAssigned(Player player)
	{
		int stamp = SubclassManager.Instance.GetAssignmentStamp(player);
		Timing.CallDelayed(1f, () =>
		{
			if (IsCurrentAssignment(player, stamp))
			{
				SfUtils.SetMaxHealth(player, Mathf.Max(400f, player.MaxHealth * 0.85f), healToFull: true);
			}
		});
	}

	public override void OnTick(Player player)
	{
		Vector3 position = player.Position;
		bool flag = lastPosition.TryGetValue(player, out var value) && (position - value).sqrMagnitude < 0.05f;
		lastPosition[player] = position;
		if (!flag)
		{
			stillSince.Remove(player);
			BreakCamo(player, ambush: false);
			return;
		}
		if (!stillSince.ContainsKey(player))
		{
			stillSince[player] = Time.time;
		}
		bool flag2 = !nextCamo.TryGetValue(player, out var value2) || Time.time >= value2;
		bool flag3 = NearCorpse(player);
		if ((!hidden.Contains(player) & flag2 & flag3) && Time.time - stillSince[player] >= 3f)
		{
			hidden.Add(player);
			SfUtils.Hint(player, "<color=#9f9><b>Camuflado</b> entre los cuerpos. No te muevas.</color>", 4f);
		}
		if (hidden.Contains(player))
		{
			player.EnableEffect(EffectType.Invisible, 1, 2f);
			Player player2 = SfUtils.AliveNear(player, 2f).FirstOrDefault((Player p) => p != player && p.IsHuman);
			if (player2 != null)
			{
				BreakCamo(player, ambush: true);
			}
		}
	}

	private void BreakCamo(Player player, bool ambush)
	{
		if (hidden.Remove(player))
		{
			player.DisableEffect(EffectType.Invisible);
			nextCamo[player] = Time.time + 40f;
			if (ambush)
			{
				ambushUntil[player] = Time.time + 5f;
				SfUtils.Hint(player, "<color=#f84><b>Emboscada.</b> Tu primer golpe pega más fuerte.</color>", 4f);
			}
		}
	}

	public override void OnDealingDamage(Player attacker, HurtingEventArgs ev)
	{
		if (ambushUntil.TryGetValue(attacker, out var value) && !(Time.time > value))
		{
			ambushUntil.Remove(attacker);
			ev.Amount *= 1.2f;
		}
	}

	private static bool NearCorpse(Player player)
	{
		try
		{
			return Ragdoll.List.Any((Ragdoll r) => r != null && (r.Position - player.Position).sqrMagnitude <= 9f);
		}
		catch
		{
			return false;
		}
	}

	public override void OnRemoved(Player player)
	{
		lastPosition.Remove(player);
		stillSince.Remove(player);
		nextCamo.Remove(player);
		ambushUntil.Remove(player);
		hidden.Remove(player);
		player?.DisableEffect(EffectType.Invisible);
	}
}
