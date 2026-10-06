using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class CaosFrenetico : Subclass
{
	public override string Id => "caos_frenetico";

	public override string Name => "Caos Frenético";

	public override string Description => "Combatiente del Caos con un +12% de velocidad permanente.";

	public override RoleTypeId[] Roles => ChaosSubclasses.AllChaos;

	public override float Chance => 10f;

	public override float SpeedMultiplier => 1.12f;
}
