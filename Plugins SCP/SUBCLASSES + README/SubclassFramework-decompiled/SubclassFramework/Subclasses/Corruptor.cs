using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class Corruptor : Subclass
{
	private readonly Dictionary<(Player Owner, Player Victim), float> nextCorruption = new Dictionary<(Player, Player), float>();

	public override string Id => "106_corruptor";

	public override string Name => "SCP-106 Corruptor";

	public override string Description => "Quien toques sufrirá 20 segundos de corrupción y recibirá la mitad de curación. Tienes algo menos de resistencia que el 106 normal.";

	public override RoleTypeId[] Roles => ScpSubclasses.Scp106;

	public override float Chance => 10f;

	public override void OnAssigned(Player player)
	{
		int stamp = SubclassManager.Instance.GetAssignmentStamp(player);
		Timing.CallDelayed(1f, () =>
		{
			if (IsCurrentAssignment(player, stamp))
			{
				SfUtils.SetMaxHealth(player, player.MaxHealth * 0.95f, healToFull: true);
			}
		});
	}

	public override void OnDealingDamage(Player attacker, HurtingEventArgs ev)
	{
		Player player = ev.Player;
		if (!(player == null) && player.IsHuman)
		{
			(Player, Player) key = (attacker, player);
			if (!nextCorruption.TryGetValue(key, out var value) || !(Time.time < value))
			{
				nextCorruption[key] = Time.time + 45f;
				GlobalHooks.Corrupt(player, 20f);
				SfUtils.Hint(player, "<color=#a6f><b>Corrupto.</b> Has sido corrompido por el 106; tus curaciones valen la mitad durante 20s.</color>", 6f);
			}
		}
	}

	public override void OnRemoved(Player player)
	{
		foreach (var item in nextCorruption.Keys.Where(((Player Owner, Player Victim) key) => key.Owner == player).ToList())
		{
			nextCorruption.Remove(item);
		}
	}
}
