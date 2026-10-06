using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class Cirujano : Subclass
{
	private static readonly HashSet<Player> Boosted = new HashSet<Player>();

	public override string Id => "049_cirujano";

	public override string Name => "SCP-049 Cirujano";

	public override string Description => "Sus 049-2 nacen con un 25% más de vida máxima. Un ejército más duro de tumbar.";

	public override RoleTypeId[] Roles => ScpSubclasses.Scp049;

	public override float Chance => 10f;

	public override void OnTick(Player player)
	{
		foreach (Player item in Player.List.Where((Player p) => p != null && p.IsAlive && SfUtils.IsZombie(p)))
		{
			if (Boosted.Add(item))
			{
				SfUtils.SetMaxHealth(item, item.MaxHealth * 1.25f, healToFull: true);
				SfUtils.Hint(item, "<color=#8f8>El Doctor te ha cosido bien. Tienes más vida de lo normal.</color>");
			}
		}
		Boosted.RemoveWhere((Player p) => p == null || !p.IsAlive);
	}

	public static void ClearPlayer(Player player)
	{
		Boosted.Remove(player);
	}

	public override void OnAssigned(Player player)
	{
		ClearPlayer(player);
	}
}
