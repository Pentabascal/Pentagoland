using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class SargentoExplorador : Subclass
{
	private readonly Dictionary<Player, float> visionUntil = new Dictionary<Player, float>();

	public override string Id => "sargento_explorador";

	public override string Name => "Sargento Explorador";

	public override string Description => "Ve a través de las paredes y se mueve más rápido durante 13 segundos, pero termina con la vista borrosa.";

	public override RoleTypeId[] Roles => MtfSubclasses.Sergeants;

	public override float Chance => 10f;

	public override float Cooldown => 180f;

	public override bool OverrideInventory => true;

	public override ItemType[] Items => new ItemType[6]
	{
		ItemType.GunE11SR,
		ItemType.Radio,
		ItemType.KeycardMTFOperative,
		ItemType.ArmorCombat,
		ItemType.Medkit,
		ItemType.GrenadeFlash
	};

	public override KeyValuePair<AmmoType, ushort>[] Ammo => new KeyValuePair<AmmoType, ushort>[1] { Subclass.A(AmmoType.Nato556, 160) };

	public override bool UseAbility(Player player)
	{
		visionUntil[player] = Time.time + 13f;
		player.EnableEffect(EffectType.Scp1344, 1, 13f);
		SfUtils.SpeedBoost(player, 10f, 13f);
		SfUtils.Hint(player, "<color=#6cf><b>Visión táctica.</b> Tienes 13 segundos; aprovéchala bien.</color>");
		return true;
	}

	public override void OnTick(Player player)
	{
		if (visionUntil.TryGetValue(player, out var value) && !(Time.time < value))
		{
			visionUntil.Remove(player);
			player.DisableEffect(EffectType.Scp1344);
			player.EnableEffect(EffectType.Blurred, 1, 4f);
			SfUtils.Hint(player, "<color=#f88>Los ojos te pasan factura: 4s de vista borrosa.</color>", 4f);
		}
	}

	public override void OnRemoved(Player player)
	{
		visionUntil.Remove(player);
		player?.DisableEffect(EffectType.Scp1344);
	}
}
