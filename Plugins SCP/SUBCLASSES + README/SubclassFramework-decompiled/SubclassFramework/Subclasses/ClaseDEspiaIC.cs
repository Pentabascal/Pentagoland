using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class ClaseDEspiaIC : Subclass
{
	public override string Id => "clased_espia_ic";

	public override string Name => "Espía de la IC";

	public override string Description => "Clase D infiltrado con una tarjeta de científico, un botiquín y 105 de vida.";

	public override RoleTypeId[] Roles => FacilitySubclasses.ClassD;

	public override float Chance => 10f;

	public override int MaxHealth => 105;

	public override bool OverrideInventory => true;

	public override ItemType[] Items => new ItemType[2]
	{
		ItemType.KeycardScientist,
		ItemType.Medkit
	};
}
