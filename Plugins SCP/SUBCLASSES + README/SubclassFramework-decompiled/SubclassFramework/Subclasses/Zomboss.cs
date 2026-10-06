using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class Zomboss : Subclass
{
	public override string Id => "zomboss";

	public override string Name => "Zomboss";

	public override string Description => "Cuantos más zombis vivos haya, más rápido se mueve parte de la plaga. No aumenta tu vida.";

	public override RoleTypeId[] Roles => ScpSubclasses.Scp0492;

	public override float Chance => 10f;

	public override int MaxPerRound => 1;

	public override void OnTick(Player player)
	{
		List<Player> list = Player.List.Where((Player p) => p != null && p.IsAlive && SfUtils.IsZombie(p)).ToList();
		if (list.Count <= 1)
		{
			return;
		}
		float percent = Mathf.Min(1.5f * (float)list.Count, 12f);
		foreach (Player item in list)
		{
			Subclass subclass = SubclassManager.Instance.GetSubclass(item);
			if (subclass == null || (!(subclass.Id == "zombi_bebe") && !(subclass.Id == "zombi_tanque") && !(subclass.Id == "zombi_ralentizador") && !(subclass.Id == "zombi_nocturno") && !(subclass.Id == "zombi_aullador")))
			{
				SfUtils.SpeedBoost(item, percent, 2f);
			}
		}
	}
}
