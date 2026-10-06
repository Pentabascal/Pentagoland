using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class Demoledor : Subclass
{
	private readonly Dictionary<Player, float> armed = new Dictionary<Player, float>();

	public override string Id => "demoledor";

	public override string Name => "Demoledor";

	public override string Description => "Represor equipado con explosivos. Prepara una carga para destruir la siguiente puerta que toque.";

	public override RoleTypeId[] Roles => ChaosSubclasses.Repressor;

	public override float Chance => 8f;

	public override int MaxPerRound => 2;

	public override int MaxHealth => 110;

	public override float Cooldown => 100f;

	public override ItemType[] ExtraItems => new ItemType[4]
	{
		ItemType.GrenadeHE,
		ItemType.GrenadeHE,
		ItemType.ArmorCombat,
		ItemType.Medkit
	};

	public override void OnAssigned(Player player)
	{
		if (!(player == null))
		{
			player.AddItem((player.Role.Team == Team.FoundationForces) ? ItemType.KeycardMTFOperative : ItemType.KeycardChaosInsurgency);
		}
	}

	public override bool UseAbility(Player player)
	{
		armed[player] = Time.time + 15f;
		SfUtils.Hint(player, "<color=#f84><b>Carga preparada.</b> La siguiente puerta que uses quedara destrozada.</color>", 6f);
		return true;
	}

	public override void OnInteractingDoor(Player player, InteractingDoorEventArgs ev)
	{
		if (player == null || ev.Door == null || !armed.TryGetValue(player, out var value) || Time.time > value)
		{
			return;
		}
		if (ev.Door.IsCheckpoint || ev.Door.IsPartOfCheckpoint || ev.Door.IsElevator)
		{
			SfUtils.Hint(player, "<color=#f88>Esta puerta está protegida contra la carga.</color>", 4f);
			return;
		}
		if (!(ev.Door is BreakableDoor breakableDoor))
		{
			SfUtils.Hint(player, "<color=#f88>Esta puerta no puede ser destruida.</color>", 4f);
			return;
		}
		if (breakableDoor.IsDestroyed)
		{
			armed.Remove(player);
			SfUtils.Hint(player, "<color=#aaa>La puerta ya estaba destruida.</color>", 3f);
			return;
		}
		ev.IsAllowed = false;
		if (!breakableDoor.Break())
		{
			SfUtils.Hint(player, "<color=#f88>No se ha podido destruir esta puerta.</color>", 4f);
			return;
		}
		armed.Remove(player);
		SfUtils.Hint(player, "<size=22><color=#f84><b>¡PUERTA DESTRUIDA!</b></color></size>", 4f);
	}

	public override void OnRemoved(Player player)
	{
		armed.Remove(player);
	}
}
