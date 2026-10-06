using System.Linq;
using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class ZombiMiasmatico : Subclass
{
	public override string Id => "zombi_miasmatico";

	public override string Name => "Zombi Miasmático";

	public override string Description => "Los humanos muy cerca reciben 2 de daño por segundo, pero solo si tienes línea de visión.";

	public override RoleTypeId[] Roles => ScpSubclasses.Scp0492;

	public override float Chance => 10f;

	public override int MaxPerRound => 1;

	public override void OnTick(Player player)
	{
		foreach (Player item in from p in SfUtils.AliveNear(player, 2.5f)
			where p.IsHuman && p.CurrentRoom == player.CurrentRoom
			where SfUtils.HasLineOfSight(player, p)
			select p)
		{
			SfUtils.Damage(item, 2f, "Miasma de SCP-049-2");
		}
	}
}
