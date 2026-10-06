using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class CaosReclutador : Subclass
{
	public override string Id => "caos_reclutador";

	public override string Name => "Reclutador del Caos";

	public override string Description => "Da un impulso breve a los combatientes del Caos y a los Clase D cercanos.";

	public override RoleTypeId[] Roles => ChaosSubclasses.AllChaos;

	public override Tier AssignTier => Tier.General;

	public override float Chance => 10f;

	public override float Cooldown => 130f;

	public override string FailMessage => "No hay aliados del Caos o Clase D cerca.";

	public override bool UseAbility(Player player)
	{
		List<Player> list = (from p in SfUtils.AliveNear(player, 12f)
			where p != player && (SfUtils.IsChaos(p) || p.Role.Type == RoleTypeId.ClassD)
			select p).ToList();
		if (list.Count == 0)
		{
			return false;
		}
		foreach (Player item in list)
		{
			SfUtils.SpeedBoost(item, 12f, 6f);
			SfUtils.Hint(item, "<color=#5ee37a><b>El Caos avanza.</b> Te han dado un impulso.</color>", 4f);
		}
		SfUtils.SpeedBoost(player, 12f, 6f);
		return true;
	}
}
