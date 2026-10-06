using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class MtfRescatista : Subclass
{
	public override string Id => "mtf_rescatista";

	public override string Name => "MTF Rescatista";

	public override string Description => "Estabiliza a los MTF cercanos con una cura pequeña y un escudo temporal.";

	public override RoleTypeId[] Roles => MtfSubclasses.AllMtf;

	public override float Chance => 10f;

	public override float Cooldown => 140f;

	public override ItemType[] ExtraItems => new ItemType[1] { ItemType.Medkit };

	public override string FailMessage => "No hay MTF heridos cerca.";

	public override bool UseAbility(Player player)
	{
		List<Player> list = (from p in SfUtils.AliveNear(player, 10f)
			where p != player && SfUtils.IsMtf(p) && p.Health < p.MaxHealth
			select p).ToList();
		if (list.Count == 0)
		{
			return false;
		}
		foreach (Player item in list)
		{
			SfUtils.Heal(item, 20f);
			SfUtils.AddAhp(item, 15f, 50f, 0f, 12f);
			SfUtils.Hint(item, "<color=#8fd3ff>Un rescatista te ha estabilizado.</color>", 4f);
		}
		SfUtils.Hint(player, $"<color=#8fd3ff>Has estabilizado a {list.Count} MTF.</color>", 4f);
		return true;
	}
}
