using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class Asesino : Subclass
{
	private readonly Dictionary<Player, float> frenzyUntil = new Dictionary<Player, float>();

	private readonly Dictionary<Player, int> frenzyKills = new Dictionary<Player, int>();

	private readonly Dictionary<Player, float> frenzyAhp = new Dictionary<Player, float>();

	public override string Id => "asesino";

	public override string Name => "Asesino";

	public override string Description => "Durante 15 segundos ganas velocidad y vida adicional por cada baja. Si no consigues ninguna, pierdes 40 de vida.";

	public override RoleTypeId[] Roles => ChaosSubclasses.Rifleman;

	public override float Chance => 10f;

	public override int MaxHealth => 90;

	public override float Cooldown => 240f;

	public override bool OverrideInventory => true;

	public override ItemType[] Items => new ItemType[6]
	{
		ItemType.GunCrossvec,
		ItemType.GunCOM18,
		ItemType.ArmorCombat,
		ItemType.KeycardChaosInsurgency,
		ItemType.Medkit,
		ItemType.Adrenaline
	};

	public override KeyValuePair<AmmoType, ushort>[] Ammo => new KeyValuePair<AmmoType, ushort>[1] { Subclass.A(AmmoType.Nato9, 160) };

	public override bool UseAbility(Player player)
	{
		frenzyUntil[player] = Time.time + 15f;
		frenzyKills[player] = 0;
		frenzyAhp[player] = 0f;
		SfUtils.SpeedBoost(player, 15f, 15f);
		SfUtils.Hint(player, "<color=#f6a><b>Frenesí.</b> 15s. Mata o pierde 40 de vida.</color>", 6f);
		return true;
	}

	public override void OnKill(Player killer, Player victim, DiedEventArgs ev)
	{
		if (frenzyUntil.TryGetValue(killer, out var value) && !(Time.time > value))
		{
			frenzyKills[killer] = (frenzyKills.TryGetValue(killer, out var value2) ? value2 : 0) + 1;
			SfUtils.AddAhp(killer, 15f, 200f, 1.2f, Mathf.Max(0.5f, value - Time.time));
			frenzyAhp[killer] = (frenzyAhp.TryGetValue(killer, out var value3) ? (value3 + 15f) : 15f);
			SfUtils.Heal(killer, 15f);
		}
	}

	public override void OnTick(Player player)
	{
		if (frenzyUntil.TryGetValue(player, out var value) && !(Time.time < value))
		{
			frenzyUntil.Remove(player);
			int num = (frenzyKills.TryGetValue(player, out var value2) ? value2 : 0);
			frenzyKills.Remove(player);
			player.DisableEffect(EffectType.MovementBoost);
			if (frenzyAhp.TryGetValue(player, out var value3))
			{
				player.ArtificialHealth = Mathf.Max(0f, player.ArtificialHealth - value3);
				frenzyAhp.Remove(player);
			}
			if (num == 0)
			{
				SfUtils.Damage(player, 40f, "Frenesí fallido");
				SfUtils.Hint(player, "<color=#f44>No has conseguido ninguna baja. El esfuerzo pasa factura: -40 de vida.</color>");
			}
			else
			{
				SfUtils.Hint(player, $"<color=#8f8>Frenesí terminado. Bajas conseguidas: {num}.</color>", 4f);
			}
		}
	}

	public override void OnRemoved(Player player)
	{
		frenzyUntil.Remove(player);
		frenzyKills.Remove(player);
		frenzyAhp.Remove(player);
	}
}
