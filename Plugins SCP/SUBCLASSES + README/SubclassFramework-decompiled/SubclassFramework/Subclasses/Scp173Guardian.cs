using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class Scp173Guardian : Subclass
{
	public override string Id => "173_guardian";

	public override string Name => "SCP-173 Guardián";

	public override string Description => "Tiene más resistencia y menos movilidad. Su habilidad agrieta el suelo, ralentiza a los humanos cercanos y le da un escudo.";

	public override RoleTypeId[] Roles => Scp173Module.Scp173;

	public override float Chance => 10f;

	public override float Cooldown => 45f;

	public override float SpeedMultiplier => 0.93f;

	public override string FailMessage => "No hay nadie lo bastante cerca para sentir la onda.";

	public override void OnAssigned(Player player)
	{
		int stamp = SubclassManager.Instance.GetAssignmentStamp(player);
		Timing.CallDelayed(1f, () =>
		{
			if (IsCurrentAssignment(player, stamp))
			{
				SfUtils.SetMaxHealth(player, player.MaxHealth * 1.15f, healToFull: true);
			}
		});
	}

	public override bool UseAbility(Player player)
	{
		List<Player> list = (from p in SfUtils.AliveNear(player, 8f)
			where p != player && p.IsHuman
			select p).ToList();
		if (list.Count == 0)
		{
			return false;
		}
		foreach (Player item in list)
		{
			SfUtils.Slow(item, 25f, 4f);
			SfUtils.Hint(item, "<color=#c9c9c9>Algo ha agrietado el suelo muy cerca de ti.</color>", 4f);
		}
		SfUtils.AddAhp(player, 60f, 120f);
		SfUtils.Hint(player, $"<color=#c9c9c9><b>Onda de piedra.</b> Humanos ralentizados: {list.Count}.</color>");
		return true;
	}
}
