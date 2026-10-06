using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class EspecialistaTecnico : Subclass
{
	private readonly Dictionary<Player, float> armed = new Dictionary<Player, float>();

	public override string Id => "especialista_tecnico";

	public override string Name => "Especialista Técnico";

	public override string Description => "Fuerza el acceso a una puerta o la bloquea temporalmente, tenga o no la tarjeta necesaria.";

	public override RoleTypeId[] Roles => MtfSubclasses.Privates;

	public override float Chance => 10f;

	public override int MaxHealth => 100;

	public override float Cooldown => 150f;

	public override bool OverrideInventory => true;

	public override ItemType[] Items => new ItemType[6]
	{
		ItemType.GunCrossvec,
		ItemType.ArmorLight,
		ItemType.Radio,
		ItemType.KeycardMTFOperative,
		ItemType.Medkit,
		ItemType.Painkillers
	};

	public override KeyValuePair<AmmoType, ushort>[] Ammo => new KeyValuePair<AmmoType, ushort>[1] { Subclass.A(AmmoType.Nato9, 120) };

	public override bool UseAbility(Player player)
	{
		armed[player] = Time.time + 15f;
		SfUtils.Hint(player, "<color=#6cf><b>Anulador listo.</b> La siguiente puerta se abrirá o se bloqueará durante 10s.</color>", 6f);
		return true;
	}

	public override void OnInteractingDoor(Player player, InteractingDoorEventArgs ev)
	{
		if (!armed.TryGetValue(player, out var value) || Time.time > value || ev.Door == null)
		{
			return;
		}
		armed.Remove(player);
		ev.IsAllowed = true;
		bool isOpen = ev.Door.IsOpen;
		Door door = ev.Door;
		if (isOpen)
		{
			door.IsOpen = false;
			door.Lock(10f, DoorLockType.AdminCommand);
			SfUtils.Hint(player, "<color=#6cf><b>Puerta sellada</b> durante 10 segundos.</color>");
			return;
		}
		door.IsOpen = true;
		door.Lock(10f, DoorLockType.AdminCommand);
		SfUtils.Hint(player, "<color=#6cf><b>Acceso forzado.</b> Se cerrara sola en 10 segundos.</color>");
		Timing.CallDelayed(10.2f, () =>
		{
			try
			{
				if (door != null)
				{
					door.IsOpen = false;
				}
			}
			catch
			{
			}
		});
	}

	public override void OnRemoved(Player player)
	{
		armed.Remove(player);
	}
}
