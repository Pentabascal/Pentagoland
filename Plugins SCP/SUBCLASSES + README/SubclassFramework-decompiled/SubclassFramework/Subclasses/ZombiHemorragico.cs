using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class ZombiHemorragico : Subclass
{
	private readonly Dictionary<(Player Attacker, Player Victim), float> nextBleed = new Dictionary<(Player, Player), float>();

	public override string Id => "zombi_hemorragico";

	public override string Name => "Zombi Hemorrágico";

	public override string Description => "Tus golpes provocan sangrado durante 5 segundos, pero solo puedes aplicarlo al mismo objetivo una vez cada 12 segundos.";

	public override RoleTypeId[] Roles => ScpSubclasses.Scp0492;

	public override float Chance => 10f;

	public override int MaxHealth => 380;

	public override void OnDealingDamage(Player attacker, HurtingEventArgs ev)
	{
		Player player = ev?.Player;
		if (!(attacker == null) && !(player == null) && player.IsHuman)
		{
			(Player, Player) key = (attacker, player);
			if (!nextBleed.TryGetValue(key, out var value) || !(Time.time < value))
			{
				nextBleed[key] = Time.time + 12f;
				player.EnableEffect(EffectType.Bleeding, 1, 5f);
				SfUtils.Hint(player, "<color=#f66><b>Herida abierta.</b> Estás sangrando durante 5 segundos.</color>", 4f);
			}
		}
	}

	public override void OnRemoved(Player player)
	{
		foreach (var item in nextBleed.Keys.Where(((Player Attacker, Player Victim) key) => key.Attacker == player).ToList())
		{
			nextBleed.Remove(item);
		}
	}
}
