using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class ZombiBebe : Subclass
{
	public override string Id => "zombi_bebe";

	public override string Name => "Zombi Bebé";

	public override string Description => "Es pequeño y rápido, pero tiene menos vida y resiste peor los golpes.";

	public override RoleTypeId[] Roles => ScpSubclasses.Scp0492;

	public override float Chance => 7f;

	public override int MaxHealth => 320;

	public override float Scale => 0.8f;

	public override float SpeedMultiplier => 1.1f;
}
