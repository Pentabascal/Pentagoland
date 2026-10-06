using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class CientificoDeCampo : Subclass
{
	private readonly Dictionary<Player, float> armed = new Dictionary<Player, float>();

	public override string Id => "cientifico_campo";

	public override string Name => "Científico de Campo";

	public override string Description => "Puede intentar forzar una puerta para la que no tiene acceso. Hace ruido, así que cuidado.";

	public override RoleTypeId[] Roles => FacilitySubclasses.Scientist;

	public override float Chance => 10f;

	public override int MaxHealth => 100;

	public override float Cooldown => 180f;

	public override bool OverrideInventory => true;

	public override ItemType[] Items => new ItemType[4]
	{
		ItemType.KeycardScientist,
		ItemType.Radio,
		ItemType.Medkit,
		ItemType.Painkillers
	};

	public override bool UseAbility(Player player)
	{
		armed[player] = Time.time + 15f;
		SfUtils.Hint(player, "<color=#fc6><b>Ganzua preparada.</b> Intentara forzar la siguiente puerta bloqueada.</color>", 6f);
		return true;
	}

	public override void OnInteractingDoor(Player player, InteractingDoorEventArgs ev)
	{
		if (!armed.TryGetValue(player, out var value) || Time.time > value || ev.Door == null || ev.IsAllowed)
		{
			return;
		}
		armed.Remove(player);
		if (!SfUtils.Roll(30f))
		{
			SfUtils.Hint(player, "<color=#f88>La cerradura aguanta. Has perdido el intento.</color>");
			return;
		}
		ev.IsAllowed = true;
		SfUtils.Hint(player, "<color=#8f8><b>Puerta forzada.</b> Y ha sonado medio pasillo.</color>");
		foreach (Player item in from p in SfUtils.AliveNear(player, 18f)
			where p != player
			select p)
		{
			SfUtils.Hint(item, "<color=#fc6>Oyes a alguien forzando una puerta cerca.</color>", 4f);
		}
	}

	public override void OnRemoved(Player player)
	{
		armed.Remove(player);
	}
}
