using Exiled.API.Features;
using MEC;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class CazadorNocturno : Subclass
{
	public override string Id => "939_nocturno";

	public override string Name => "SCP-939 Cazador Nocturno";

	public override string Description => "A oscuras gana +10% de velocidad.";

	public override RoleTypeId[] Roles => ScpSubclasses.Scp939;

	public override float Chance => 10f;

	public override void OnAssigned(Player player)
	{
		int stamp = SubclassManager.Instance.GetAssignmentStamp(player);
		Timing.CallDelayed(1f, () =>
		{
			if (IsCurrentAssignment(player, stamp))
			{
				SfUtils.SetMaxHealth(player, player.MaxHealth * 1.05f, healToFull: true);
			}
		});
	}

	public override void OnTick(Player player)
	{
		if (SfUtils.IsInDarkness(player))
		{
			SfUtils.SpeedBoost(player, 10f, 2f);
		}
	}
}
