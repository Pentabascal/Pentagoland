using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class Infiltrado : Subclass
{
	private readonly HashSet<Player> disguised = new HashSet<Player>();

	public override string Id => "infiltrado";

	public override string Name => "Infiltrado";

	public override string Description => "Espía infiltrado. Puede aparecer con el Caos o como soldado raso de los MTF; el fuego cruzado entre ambos bandos no causa daño.";

	public override RoleTypeId[] Roles => ChaosSubclasses.AllChaos.Concat(new RoleTypeId[1] { RoleTypeId.NtfPrivate }).ToArray();

	public override float Chance => 10f;

	public override int MaxPerRound => 1;

	public override void OnAssigned(Player player)
	{
		disguised.Add(player);
		if (player.Role.Team == Team.ChaosInsurgency)
		{
			player.AddItem(ItemType.KeycardMTFCaptain);
		}
		SfUtils.Hint(player, "<size=23><color=#5df><b>INFILTRADO</b></color></size>\n<size=19>Tu objetivo es pasar desapercibido entre el bando contrario.\nLos disparos entre tú y los MTF no causan daño.\nAl matar a un companero del Caos, tu disfraz cae.</size>", 10f);
	}

	public bool IsDisguised(Player player)
	{
		return disguised.Contains(player);
	}

	public override void OnTakingDamage(Player victim, HurtingEventArgs ev)
	{
		if (disguised.Contains(victim) && SfUtils.IsMtf(ev.Attacker))
		{
			ev.IsAllowed = false;
			SfUtils.Hint(ev.Attacker, "<color=#f6c85f><b>Estás disparando al Espía.</b></color>", 4f);
		}
	}

	public override void OnDealingDamage(Player attacker, HurtingEventArgs ev)
	{
		if (disguised.Contains(attacker) && SfUtils.IsMtf(ev.Player))
		{
			ev.IsAllowed = false;
			SfUtils.Hint(attacker, "<color=#f6c85f><b>Estás disparando a un aliado MTF.</b></color>", 4f);
		}
	}

	public override void OnKill(Player killer, Player victim, DiedEventArgs ev)
	{
		if (disguised.Contains(killer) && !(victim == null) && (victim.Role.Team == Team.ChaosInsurgency || victim.Role.Team == Team.ClassD))
		{
			Reveal(killer);
		}
	}

	private void Reveal(Player player)
	{
		disguised.Remove(player);
		SfUtils.Hint(player, "<size=23><color=#f84><b>DISFRAZ AL DESCUBIERTO</b></color></size>\n<size=19>Ahora eres MTF.</size>", 8f);
		SubclassManager.Instance.SuppressNextAssign(player);
		try
		{
			player.Role.Set(RoleTypeId.NtfPrivate, SpawnReason.ForceClass, RoleSpawnFlags.None);
		}
		catch (Exception ex)
		{
			Log.Error("[SubclassFramework] Infiltrado: no se pudo cambiar de rol: " + ex.Message);
		}
	}

	public override void OnRemoved(Player player)
	{
		disguised.Remove(player);
	}
}
