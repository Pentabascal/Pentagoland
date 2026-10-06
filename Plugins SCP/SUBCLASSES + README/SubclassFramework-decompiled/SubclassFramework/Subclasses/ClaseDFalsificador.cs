using System;
using System.Collections.Generic;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class ClaseDFalsificador : Subclass
{
	private readonly Dictionary<Player, float> disguisedUntil = new Dictionary<Player, float>();

	public override string Id => "clased_falsificador";

	public override string Name => "Falsificador";

	public override string Description => "Durante 20 segundos aparentas ser un Guardia, pero sigues siendo Clase D y no obtienes permisos ni equipo.";

	public override RoleTypeId[] Roles => ExtraSubclasses.ClassD;

	public override float Chance => 10f;

	public override int MaxPerRound => 1;

	public override float Cooldown => 180f;

	public override string FailMessage => "No puedes preparar otro disfraz.";

	public override bool UseAbility(Player player)
	{
		if (player == null || !player.IsAlive || disguisedUntil.ContainsKey(player))
		{
			return false;
		}
		try
		{
			player.ChangeAppearance(RoleTypeId.FacilityGuard, skipJump: false, 0);
			disguisedUntil[player] = Time.time + 20f;
			SfUtils.Hint(player, "<color=#ffd166><b>Disfraz activado.</b> Un golpe lo revelara.</color>");
			return true;
		}
		catch (Exception ex)
		{
			Log.Debug("[SubclassFramework] No se pudo activar el disfraz: " + ex.Message);
			return false;
		}
	}

	public override void OnTakingDamage(Player victim, HurtingEventArgs ev)
	{
		EndDisguise(victim, hit: true);
	}

	public override void OnTick(Player player)
	{
		if (disguisedUntil.TryGetValue(player, out var value) && Time.time >= value)
		{
			EndDisguise(player, hit: false);
		}
	}

	public override void OnDeath(Player player, DiedEventArgs ev)
	{
		EndDisguise(player, hit: false);
	}

	public override void OnRemoved(Player player)
	{
		EndDisguise(player, hit: false);
	}

	private void EndDisguise(Player player, bool hit)
	{
		if (player == null || !disguisedUntil.Remove(player))
		{
			return;
		}
		try
		{
			if (player.IsAlive && player.Role.Type == RoleTypeId.ClassD)
			{
				player.ChangeAppearance(RoleTypeId.ClassD, skipJump: false, 0);
			}
		}
		catch (Exception ex)
		{
			Log.Debug("[SubclassFramework] No se pudo restaurar el aspecto del falsificador: " + ex.Message);
		}
		if (hit && player.IsAlive)
		{
			SfUtils.Hint(player, "<color=#ffd166>El disfraz se ha roto al recibir daño.</color>", 3f);
		}
	}
}
