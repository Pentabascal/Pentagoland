using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class Bestia : Subclass
{
	public override string Id => "939_bestia";

	public override string Name => "SCP-939 Bestia";

	public override string Description => "Mucho más duro, pero más lento. Cambias movilidad por stamina bruta.";

	public override RoleTypeId[] Roles => ScpSubclasses.Scp939;

	public override float Chance => 10f;

	public override float SpeedMultiplier => 0.87f;

	public override int MaxHealth => 3150;
}
