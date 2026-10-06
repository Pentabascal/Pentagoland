using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class GuardiaArmero : Subclass
{
	public override string Id => "guardia_armero";

	public override string Name => "Guardia Armero";

	public override string Description => "Reparte 20 balas de 9 mm a los guardias que estén en su sala.";

	public override RoleTypeId[] Roles => FacilitySubclasses.Guard;

	public override float Chance => 10f;

	public override float Cooldown => 160f;

	public override void OnAssigned(Player player)
	{
		player.AddAmmo(AmmoType.Nato9, 60);
	}

	public override bool UseAbility(Player player)
	{
		int num = 0;
		foreach (Player item in SfUtils.InSameRoom(player).Where(SfUtils.IsGuard))
		{
			item.AddAmmo(AmmoType.Nato9, 20);
			if (item != player)
			{
				SfUtils.Hint(item, "<color=#8cf>+20 balas de 9mm.</color>", 4f);
			}
			num++;
		}
		SfUtils.Hint(player, $"<color=#8cf><b>Munición repartida</b> a {num} guardias.</color>");
		return true;
	}
}
