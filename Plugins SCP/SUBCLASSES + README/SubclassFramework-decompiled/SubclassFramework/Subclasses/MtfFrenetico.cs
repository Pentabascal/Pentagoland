using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class MtfFrenetico : Subclass
{
	public override string Id => "mtf_frenetico";

	public override string Name => "MTF Frenético";

	public override string Description => "Miembro de los MTF con un 10% más de velocidad de forma permanente.";

	public override RoleTypeId[] Roles => MtfSubclasses.AllMtf;

	public override float Chance => 10f;

	public override float SpeedMultiplier => 1.1f;
}
