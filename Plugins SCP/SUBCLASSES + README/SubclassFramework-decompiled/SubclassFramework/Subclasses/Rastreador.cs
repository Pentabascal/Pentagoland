using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class Rastreador : Subclass
{
	private readonly Dictionary<Player, Player> marks = new Dictionary<Player, Player>();

	private readonly Dictionary<Player, float> markUntil = new Dictionary<Player, float>();

	public override string Id => "rastreador";

	public override string Name => "Rastreador";

	public override string Description => "Apoyo de información: marca a un objetivo durante 20s y todo el Caos cercano sabe dónde está.";

	public override RoleTypeId[] Roles => ChaosSubclasses.Rifleman;

	public override float Chance => 10f;

	public override int MaxHealth => 85;

	public override float Cooldown => 130f;

	public override bool OverrideInventory => true;

	public override string FailMessage => "Apunta a un jugador para marcarlo.";

	public override ItemType[] Items => new ItemType[5]
	{
		ItemType.GunCOM15,
		ItemType.GunCOM18,
		ItemType.ArmorLight,
		ItemType.KeycardChaosInsurgency,
		ItemType.Medkit
	};

	public override KeyValuePair<AmmoType, ushort>[] Ammo => new KeyValuePair<AmmoType, ushort>[1] { Subclass.A(AmmoType.Nato9, 90) };

	public override bool UseAbility(Player player)
	{
		Player lookingAt = SfUtils.GetLookingAt(player);
		if (lookingAt == null)
		{
			return false;
		}
		marks[player] = lookingAt;
		markUntil[player] = Time.time + 20f;
		SfUtils.Hint(player, "<color=#6cf><b>Objetivo marcado:</b> " + lookingAt.Nickname + "</color>");
		return true;
	}

	public override void OnTick(Player player)
	{
		if (!marks.TryGetValue(player, out var value))
		{
			return;
		}
		if (value == null || !value.IsAlive)
		{
			float num = SubclassManager.Instance.RemainingCooldown(player);
			if (num > 0f)
			{
				SubclassManager.Instance.StartCooldown(player, num * 0.5f);
				SfUtils.Hint(player, "<color=#6cf>Objetivo abatido. Enfriamiento reducido a la mitad.</color>", 4f);
			}
			Clear(player);
			return;
		}
		if (!markUntil.TryGetValue(player, out var value2) || Time.time > value2)
		{
			Clear(player);
			return;
		}
		string message = "<size=18><color=#6cf>Marcado: <b>" + value.Nickname + "</b> - " + SfUtils.RoomName(value) + "</color></size>";
		foreach (Player item in SfUtils.AliveNear(player, 40f).Where(SfUtils.IsChaos))
		{
			SfUtils.Hint(item, message, 1.2f);
		}
	}

	private void Clear(Player player)
	{
		marks.Remove(player);
		markUntil.Remove(player);
	}

	public override void OnRemoved(Player player)
	{
		Clear(player);
	}
}
