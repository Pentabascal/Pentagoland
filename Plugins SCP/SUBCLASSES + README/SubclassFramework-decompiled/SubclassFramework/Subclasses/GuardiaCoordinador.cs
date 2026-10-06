using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class GuardiaCoordinador : Subclass
{
	public override string Id => "guardia_coordinador";

	public override string Name => "Guardia Coordinador";

	public override string Description => "Coordina una retirada corta: los aliados de la Fundación cercanos reciben velocidad y un pequeño refuerzo de escudo.";

	public override RoleTypeId[] Roles => FacilitySubclasses.Guard;

	public override float Chance => 10f;

	public override int MaxPerRound => 1;

	public override float Cooldown => 100f;

	public override ItemType[] ExtraItems => new ItemType[1] { ItemType.Radio };

	public override string FailMessage => "No hay aliados de la Fundación cerca.";

	public override bool UseAbility(Player player)
	{
		List<Player> list = (from p in SfUtils.AliveNear(player, 8f)
			where p != player && p.Role.Team == Team.FoundationForces
			select p).ToList();
		if (list.Count == 0)
		{
			return false;
		}
		foreach (Player item in list)
		{
			SfUtils.SpeedBoost(item, 10f, 12f);
			SfUtils.AddAhp(item, 10f, 50f);
			SfUtils.Hint(item, "<color=#8fd3ff>El Guardia Coordinador ordena avanzar.</color>", 4f);
		}
		SfUtils.Hint(player, $"<color=#8fd3ff><b>Retirada coordinada:</b> {list.Count} aliados reforzados durante 12s.</color>");
		return true;
	}
}
