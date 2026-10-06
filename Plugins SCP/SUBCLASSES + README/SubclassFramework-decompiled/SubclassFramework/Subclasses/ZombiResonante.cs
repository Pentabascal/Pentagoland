using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class ZombiResonante : Subclass
{
	public override string Id => "zombi_resonante";

	public override string Name => "Zombi Resonante";

	public override string Description => "Emite un pulso que desorienta a los humanos cercanos y acelera brevemente a los zombis que lo oyen.";

	public override RoleTypeId[] Roles => ScpSubclasses.Scp0492;

	public override float Chance => 10f;

	public override float Cooldown => 75f;

	public override int MaxPerWave => 1;

	public override string FailMessage => "No hay humanos cerca para afectar con el pulso.";

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
			item.EnableEffect(EffectType.Concussed, 1, 3f);
			item.EnableEffect(EffectType.Deafened, 1, 4f);
			SfUtils.Hint(item, "<color=#d9a6ff><b>Resonancia.</b> Un pulso zombi te ha desorientado.</color>", 4f);
		}
		foreach (Player item2 in Player.List.Where((Player p) => p != null && p.IsAlive && SfUtils.IsZombie(p)))
		{
			SfUtils.SpeedBoost(item2, 10f, 5f);
		}
		SfUtils.Hint(player, $"<color=#d9a6ff><b>Pulso resonante.</b> Humanos afectados: {list.Count}.</color>", 4f);
		return true;
	}
}
