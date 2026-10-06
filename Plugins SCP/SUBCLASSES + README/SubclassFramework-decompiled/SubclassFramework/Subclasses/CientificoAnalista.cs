using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class CientificoAnalista : Subclass
{
	public override string Id => "cientifico_analista";

	public override string Name => "Científico Analista";

	public override string Description => "Analiza la zona y avisa a los humanos cercanos si detecta SCP.";

	public override RoleTypeId[] Roles => FacilitySubclasses.Scientist;

	public override float Chance => 10f;

	public override int MaxPerRound => 1;

	public override float Cooldown => 110f;

	public override string FailMessage => "No hay ningún SCP cercano que analizar.";

	public override bool UseAbility(Player player)
	{
		List<Player> list = Player.List.Where((Player p) => p != null && p.IsAlive && p.IsScp && (p.Position - player.Position).sqrMagnitude <= 625f).ToList();
		if (list.Count == 0)
		{
			return false;
		}
		foreach (Player item in from p in SfUtils.AliveNear(player, 18f)
			where p != player && p.IsHuman
			select p)
		{
			SfUtils.Hint(item, "<color=#ffe66d><b>Alerta cientifica:</b> presencia SCP en " + SfUtils.RoomName(player) + ".</color>");
		}
		SfUtils.Hint(player, $"<color=#ffe66d>Analisis: {list.Count} SCP detectados.</color>");
		return true;
	}
}
