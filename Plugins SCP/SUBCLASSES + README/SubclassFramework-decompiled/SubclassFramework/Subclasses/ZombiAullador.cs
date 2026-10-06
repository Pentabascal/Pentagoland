using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class ZombiAullador : Subclass
{
	public override string Id => "zombi_aullador";

	public override string Name => "Zombi Aullador";

	public override string Description => "Tu aullido delata a los humanos cercanos al resto de la plaga y te acelera un momento.";

	public override RoleTypeId[] Roles => ScpSubclasses.Scp0492;

	public override float Chance => 10f;

	public override float Cooldown => 50f;

	public override string FailMessage => "No hay ningún humano lo bastante cerca al que delatar.";

	public override bool UseAbility(Player player)
	{
		List<Player> list = (from p in SfUtils.AliveNear(player, 20f)
			where p.IsHuman
			select p).ToList();
		if (list.Count == 0)
		{
			return false;
		}
		List<Player> list2 = Player.List.Where((Player p) => p != null && p.IsAlive && p != player && SfUtils.IsZombie(p)).ToList();
		string text = SfUtils.RoomName(player);
		foreach (Player item in list2)
		{
			SfUtils.Hint(item, "<color=#8f8><b>Aullido:</b> hay presas en <b>" + text + "</b>.</color>", 3.5f);
		}
		SfUtils.SpeedBoost(player, 10f, 4f);
		SfUtils.Hint(player, $"<color=#8f8>Has aullado. Presas delatadas: {list.Count}.</color>", 3.5f);
		return true;
	}
}
