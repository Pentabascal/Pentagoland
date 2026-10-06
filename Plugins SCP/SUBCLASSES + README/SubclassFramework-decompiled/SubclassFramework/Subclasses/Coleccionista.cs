using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class Coleccionista : Subclass
{
	public override string Id => "049_coleccionista";

	public override string Name => "SCP-049 Coleccionista";

	public override string Description => "Cada zombi que tú hayas revivido durante la ronda te da un 1% de velocidad, hasta un máximo del 15%.";

	public override RoleTypeId[] Roles => ScpSubclasses.Scp049;

	public override float Chance => 10f;

	public override int MaxPerRound => 1;

	public override void OnTick(Player player)
	{
		float num = Mathf.Min(ZombieReviveTracker.Count(player), 15);
		if (num > 0f)
		{
			SfUtils.SpeedBoost(player, num, 2f);
		}
	}
}
