using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class ZombiRalentizador : Subclass
{
	private readonly Dictionary<(Player Attacker, Player Victim), int> stacksOn = new Dictionary<(Player, Player), int>();

	private readonly Dictionary<(Player Attacker, Player Victim), int> stackGeneration = new Dictionary<(Player, Player), int>();

	public override string Id => "zombi_ralentizador";

	public override string Name => "Zombi Ralentizador";

	public override string Description => "Cada golpe ralentiza más a la presa. Es más eficaz en grupo y tiene algo más de vida que un zombi normal.";

	public override RoleTypeId[] Roles => ScpSubclasses.Scp0492;

	public override float Chance => 10f;

	public override float SpeedMultiplier => 0.95f;

	public override void OnAssigned(Player player)
	{
		int stamp = SubclassManager.Instance.GetAssignmentStamp(player);
		Timing.CallDelayed(1f, () =>
		{
			if (IsCurrentAssignment(player, stamp))
			{
				SfUtils.SetMaxHealth(player, player.MaxHealth + 45f, healToFull: true);
			}
		});
	}

	public override void OnDealingDamage(Player attacker, HurtingEventArgs ev)
	{
		Player victim = ev.Player;
		if (victim == null || !victim.IsHuman)
		{
			return;
		}
		(Player Attacker, Player Victim) key = (Attacker: attacker, Victim: victim);
		int num = ((!stacksOn.TryGetValue(key, out var value)) ? 1 : Mathf.Min(value + 1, 6));
		stacksOn[key] = num;
		int generation = ((!stackGeneration.TryGetValue(key, out var value2)) ? 1 : (value2 + 1));
		stackGeneration[key] = generation;
		SfUtils.Slow(victim, 4f * (float)num, 3f * (float)num);
		Timing.CallDelayed(3f * (float)num, () =>
		{
			if (victim != null && stackGeneration.TryGetValue(key, out var value3) && value3 == generation)
			{
				stacksOn.Remove(key);
				stackGeneration.Remove(key);
			}
		});
	}

	public override void OnRemoved(Player player)
	{
		foreach (var item in stacksOn.Keys.Where(((Player Attacker, Player Victim) key) => key.Attacker == player).ToList())
		{
			stacksOn.Remove(item);
			stackGeneration.Remove(item);
		}
	}
}
