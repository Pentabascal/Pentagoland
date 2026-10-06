using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class Scp096Lamento : Subclass
{
	public override string Id => "scp096_lamento";

	public override string Name => "SCP-096 Lamento";

	public override string Description => "Un grito que desordena a los humanos cercanos: los deja aturdidos y borrosos durante unos segundos.";

	public override RoleTypeId[] Roles => ExtraSubclasses.Scp096;

	public override float Chance => 10f;

	public override float Cooldown => 150f;

	public override string FailMessage => "No hay humanos lo bastante cerca para oír tu lamento.";

	public override bool UseAbility(Player player)
	{
		List<Player> list = (from p in SfUtils.AliveNear(player, 10f)
			where p != player && p.IsHuman
			select p).ToList();
		if (list.Count == 0)
		{
			return false;
		}
		foreach (Player item in list)
		{
			item.EnableEffect(EffectType.Concussed, 1, 3.5f);
			item.EnableEffect(EffectType.Blurred, 1, 5f);
			SfUtils.Slow(item, 15f, 5f);
			SfUtils.Hint(item, "<color=#f66><b>El lamento de SCP-096 te desorienta.</b></color>");
		}
		SfUtils.Hint(player, $"<color=#f66><b>Lamento.</b> {list.Count} humanos desorientados.</color>");
		return true;
	}
}
