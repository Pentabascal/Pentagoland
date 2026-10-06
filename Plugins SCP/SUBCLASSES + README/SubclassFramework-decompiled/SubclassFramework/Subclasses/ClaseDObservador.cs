using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class ClaseDObservador : Subclass
{
	public override string Id => "clased_observador";

	public override string Name => "Clase D Observador";

	public override string Description => "Clase D atento y orientado, capaz de moverse un poco más rápido durante una fuga.";

	public override RoleTypeId[] Roles => FacilitySubclasses.ClassD;

	public override float Chance => 10f;

	public override float SpeedMultiplier => 1.05f;
}
