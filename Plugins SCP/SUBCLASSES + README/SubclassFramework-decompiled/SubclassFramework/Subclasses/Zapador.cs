using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class Zapador : Subclass
{
	private readonly Dictionary<Player, float> armed = new Dictionary<Player, float>();

	public override string Id => "zapador";

	public override string Name => "Zapador";

	public override string Description => "Refuerza una puerta durante 20 segundos para mantener una sala hasta que lleguen refuerzos.";

	public override RoleTypeId[] Roles => MtfSubclasses.Privates;

	public override float Chance => 10f;

	public override int MaxHealth => 100;

	public override float Cooldown => 180f;

	public override bool OverrideInventory => true;

	public override ItemType[] Items => new ItemType[5]
	{
		ItemType.GunCrossvec,
		ItemType.ArmorLight,
		ItemType.Medkit,
		ItemType.KeycardMTFOperative,
		ItemType.Radio
	};

	public override KeyValuePair<AmmoType, ushort>[] Ammo => new KeyValuePair<AmmoType, ushort>[1] { Subclass.A(AmmoType.Nato9, 120) };

	public override bool UseAbility(Player player)
	{
		armed[player] = Time.time + 15f;
		SfUtils.Hint(player, "<color=#fc6><b>Barricada lista.</b> Usa la puerta que quieras reforzar.</color>", 6f);
		return true;
	}

	public override void OnInteractingDoor(Player player, InteractingDoorEventArgs ev)
	{
		if (armed.TryGetValue(player, out var value) && !(Time.time > value) && ev.Door != null)
		{
			armed.Remove(player);
			ev.IsAllowed = true;
			ev.Door.Lock(8f, DoorLockType.AdminCommand);
			SfUtils.Hint(player, "<color=#fc6><b>Puerta reforzada</b> durante 8 segundos.</color>");
		}
	}

	public override void OnRemoved(Player player)
	{
		armed.Remove(player);
	}
}
