using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class CapitanTactico : Subclass
{
	private readonly Dictionary<Player, float> scanUntil = new Dictionary<Player, float>();

	public override string Id => "capitan_tactico";

	public override string Name => "Capitán Táctico";

	public override string Description => "Revela los generadores y los SCP sueltos a todo el equipo durante 30 segundos.";

	public override RoleTypeId[] Roles => MtfSubclasses.Captains;

	public override float Chance => 10f;

	public override int MaxHealth => 110;

	public override float Cooldown => 200f;

	public override bool OverrideInventory => true;

	public override ItemType[] Items => new ItemType[6]
	{
		ItemType.GunCrossvec,
		ItemType.ArmorCombat,
		ItemType.Radio,
		ItemType.KeycardMTFCaptain,
		ItemType.Medkit,
		ItemType.Medkit
	};

	public override KeyValuePair<AmmoType, ushort>[] Ammo => new KeyValuePair<AmmoType, ushort>[1] { Subclass.A(AmmoType.Nato9, 200) };

	public override bool UseAbility(Player player)
	{
		scanUntil[player] = Time.time + 30f;
		SfUtils.Hint(player, "<color=#6cf><b>Barrido táctico.</b> 30 segundos de información para todo el equipo.</color>");
		return true;
	}

	public override void OnTick(Player player)
	{
		if (!scanUntil.TryGetValue(player, out var value))
		{
			return;
		}
		if (Time.time > value)
		{
			scanUntil.Remove(player);
			return;
		}
		string text = BuildGeneratorReport();
		string text2 = BuildScpReport();
		string message = "<size=18><color=#6cf><b>BARRIDO TACTICO</b>\n" + text + "\n" + text2 + "</color></size>";
		foreach (Player item in Player.List.Where(SfUtils.IsMtf))
		{
			SfUtils.Hint(item, message, 1.2f);
		}
	}

	private static string BuildGeneratorReport()
	{
		try
		{
			int num = Generator.List.Count((Generator g) => g?.IsEngaged ?? false);
			int num2 = Generator.List.Count();
			return $"Generadores: {num}/{num2} activados";
		}
		catch
		{
			return "Generadores: sin datos";
		}
	}

	private static string BuildScpReport()
	{
		List<string> list = (from p in Player.List
			where p != null && p.IsAlive && p.IsScp
			select $"{p.Role.Type} - {SfUtils.RoomName(p)}").Take(5).ToList();
		return (list.Count == 0) ? "Sin SCPs localizados" : string.Join("\n", list);
	}

	public override void OnRemoved(Player player)
	{
		scanUntil.Remove(player);
	}
}
