using System.Collections.Generic;
using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class CientificoAnsioso : Subclass
{
	private readonly Dictionary<Player, float> nextPanic = new Dictionary<Player, float>();

	public override string Id => "cientifico_ansioso";

	public override string Name => "Científico Ansioso";

	public override string Description => "Cuando está herido, el pánico le obliga a moverse más rápido.";

	public override RoleTypeId[] Roles => FacilitySubclasses.Scientist;

	public override float Chance => 10f;

	public override int MaxHealth => 95;

	public override void OnTick(Player player)
	{
		if (!(player.Health > player.MaxHealth * 0.3f) && (!nextPanic.TryGetValue(player, out var value) || !(Time.time < value)))
		{
			nextPanic[player] = Time.time + 120f;
			SfUtils.SpeedBoost(player, 10f, 8f);
			SfUtils.Hint(player, "<color=#fc6><b>Panico.</b> Corre.</color>", 4f);
		}
	}

	public override void OnRemoved(Player player)
	{
		nextPanic.Remove(player);
	}
}
