using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class CientificoLoco : Subclass
{
	private static readonly EffectType[] Roulette = new EffectType[10]
	{
		EffectType.MovementBoost,
		EffectType.Scp207,
		EffectType.Invigorated,
		EffectType.Blurred,
		EffectType.Concussed,
		EffectType.Burned,
		EffectType.Slowness,
		EffectType.Deafened,
		EffectType.Disabled,
		EffectType.Scp1853
	};

	private readonly Dictionary<Player, HashSet<Item>> pending = new Dictionary<Player, HashSet<Item>>();

	public override string Id => "cientifico_loco";

	public override string Name => "Científico Loco";

	public override string Description => "Sintetiza un analgésico experimental: 65% de las veces no pasa nada y 35% de las veces sufres un efecto aleatorio.";

	public override RoleTypeId[] Roles => FacilitySubclasses.Scientist;

	public override float Chance => 10f;

	public override float Cooldown => 210f;

	public override bool OverrideInventory => true;

	public override ItemType[] Items => new ItemType[4]
	{
		ItemType.KeycardScientist,
		ItemType.GunCOM15,
		ItemType.Radio,
		ItemType.Medkit
	};

	public override KeyValuePair<AmmoType, ushort>[] Ammo => new KeyValuePair<AmmoType, ushort>[1] { Subclass.A(AmmoType.Nato9, 12) };

	public override bool UseAbility(Player player)
	{
		Item item = player.AddItem(ItemType.Painkillers);
		if (item != null)
		{
			if (!pending.TryGetValue(player, out var value))
			{
				value = (pending[player] = new HashSet<Item>());
			}
			value.Add(item);
		}
		SfUtils.Hint(player, "<color=#c9f><b>Analgesico experimental</b> sintetizado. Suerte.</color>");
		return true;
	}

	public override void OnItemConsumed(Player player, Item item)
	{
		if (item != null && pending.TryGetValue(player, out var value) && value.Remove(item))
		{
			if (!SfUtils.Roll(35f))
			{
				SfUtils.Hint(player, "<color=#aaa>Esta vez no ha pasado nada raro.</color>", 4f);
				return;
			}
			EffectType effectType = SfUtils.Pick(Roulette);
			player.EnableEffect(effectType, 1, 5f);
			SfUtils.Hint(player, $"<color=#c9f>Efecto secundario: <b>{effectType}</b> durante 5s.</color>");
		}
	}

	public override void OnRemoved(Player player)
	{
		pending.Remove(player);
	}
}
