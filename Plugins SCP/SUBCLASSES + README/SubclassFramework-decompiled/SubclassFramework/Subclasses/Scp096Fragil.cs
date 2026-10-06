using Exiled.API.Features;
using MEC;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class Scp096Fragil : Subclass
{
	public override string Id => "scp096_fr";

	public override string Name => "SCP-096 Frenético";

	public override string Description => "SCP-096 más rápido, pero con menos vida.";

	public override RoleTypeId[] Roles => new RoleTypeId[1] { RoleTypeId.Scp096 };

	public override float Chance => 8f;

	public override int MaxHealth => 2100;

	public override float SpeedMultiplier => 1.16f;

	public override void OnAssigned(Player player)
	{
		if (player == null)
		{
			return;
		}
		int stamp = SubclassManager.Instance.GetAssignmentStamp(player);
		Timing.CallDelayed(2f, () =>
		{
			if (IsCurrentAssignment(player, stamp))
			{
				SfUtils.Hint(player, "<color=#FF4040>Eres frágil, pero muy rápido. Usa la velocidad a tu favor.</color>", 6f);
			}
		});
	}
}
