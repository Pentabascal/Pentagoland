using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class ZombiTanque : Subclass
{
	public override string Id => "zombi_tanque";

	public override string Name => "Zombi Tanque";

	public override string Description => "Zombi resistente, grande y más lento.";

	public override RoleTypeId[] Roles => ScpSubclasses.Scp0492;

	public override float Chance => 4f;

	public override int MaxHealth => 600;

	public override float Scale => 1.2f;

	public override float SpeedMultiplier => 0.88f;
}
