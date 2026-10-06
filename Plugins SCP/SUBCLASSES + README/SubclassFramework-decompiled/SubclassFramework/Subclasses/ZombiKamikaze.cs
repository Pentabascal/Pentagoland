using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class ZombiKamikaze : Subclass
{
	private static readonly HashSet<Player> DetonatedPlayers = new HashSet<Player>();

	private const float BlastRadius = 4f;

	private const float BlastDamage = 60f;

	public override string Id => "zombi_kamikaze";

	public override string Name => "Zombi Kamikaze";

	public override string Description => "Tiene 280 de vida máxima. Al morir explota y causa 60 de daño a humanos cercanos, nunca a SCPs.";

	public override RoleTypeId[] Roles => ScpSubclasses.Scp0492;

	public override float Chance => 5f;

	public override int MaxHealth => 280;

	public override void OnAssigned(Player player)
	{
		DetonatedPlayers.Remove(player);
	}

	public override void OnDeath(Player player, DiedEventArgs ev)
	{
		if (player == null || !DetonatedPlayers.Add(player))
		{
			return;
		}
		Vector3 position = player.Position;
		try
		{
			Ragdoll ragdoll = Ragdoll.List.FirstOrDefault((Ragdoll r) => r != null && r.Owner == player);
			if (ragdoll != null)
			{
				position = ragdoll.Position;
			}
		}
		catch (Exception ex)
		{
			Log.Debug("[SubclassFramework] Zombi Kamikaze (ragdoll): " + ex.Message);
		}
		Timing.CallDelayed(0.05f, () =>
		{
			Detonate(position);
		});
	}

	public static void ClearAll()
	{
		DetonatedPlayers.Clear();
	}

	private static void Detonate(Vector3 position)
	{
		foreach (Player item in Player.List.Where((Player p) => p != null && p.IsAlive && p.IsHuman && (p.Position - position).sqrMagnitude <= BlastRadius * BlastRadius))
		{
			SfUtils.Damage(item, 60f, "Explosion de Zombi Kamikaze");
		}
		try
		{
			Player owner2 = Player.List.FirstOrDefault((Player p) => p != null && p.IsAlive);
			if (Item.Create(ItemType.GrenadeHE, owner2) is ExplosiveGrenade explosiveGrenade)
			{
				explosiveGrenade.FuseTime = 0.1f;
				explosiveGrenade.MaxRadius = 0.1f;
				explosiveGrenade.ScpDamageMultiplier = 0f;
				explosiveGrenade.BurnDuration = 0f;
				explosiveGrenade.DeafenDuration = 0f;
				explosiveGrenade.ConcussDuration = 0f;
				explosiveGrenade.SpawnActive(position, owner2);
			}
		}
		catch (Exception ex)
		{
			Log.Debug("[SubclassFramework] No se pudo mostrar la explosion del Zombi Kamikaze: " + ex.Message);
		}
	}
}
