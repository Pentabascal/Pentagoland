using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class CientificoBioquimico : Subclass
{
	public override string Id => "cientifico_bioquimico";

	public override string Name => "Bioquímico";

	public override string Description => "Aplica un estabilizante que cura y protege a los aliados de tu sala.";

	public override RoleTypeId[] Roles => ExtraSubclasses.Scientist;

	public override float Chance => 10f;

	public override int MaxPerRound => 1;

	public override float Cooldown => 75f;

	public override ItemType[] ExtraItems => new ItemType[1] { ItemType.Medkit };

	public override string FailMessage => "No tienes a nadie cerca a quien tratar.";

	public override bool UseAbility(Player player)
	{
		if (player == null || !player.IsAlive)
		{
			return false;
		}
		List<Player> list = (from p in SfUtils.InSameRoom(player)
			where p != player && !p.IsScp && !SfUtils.IsZombie(p)
			select p).ToList();
		if (list.Count == 0)
		{
			return false;
		}
		foreach (Player item in list)
		{
			SfUtils.Heal(item, 35f);
			SfUtils.AddAhp(item, 25f, 75f);
			SfUtils.Hint(item, "<color=#FFFF7C>Te han inyectado un estabilizante.</color>", 4f);
		}
		SfUtils.Heal(player, 15f);
		SfUtils.Hint(player, $"<color=#FFFF7C>Estabilizante aplicado a {list.Count} aliados.</color>");
		return true;
	}
}
