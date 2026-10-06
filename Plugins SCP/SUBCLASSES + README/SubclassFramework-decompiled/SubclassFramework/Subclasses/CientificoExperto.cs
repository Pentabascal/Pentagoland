using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class CientificoExperto : Subclass
{
	public override string Id => "cientifico_experto";

	public override string Name => "Científico Experto";

	public override string Description => "Científico con mejor preparación y 110 de vida.";

	public override RoleTypeId[] Roles => FacilitySubclasses.Scientist;

	public override float Chance => 10f;

	public override int MaxHealth => 110;

	public override bool OverrideInventory => true;

	public override ItemType[] Items => new ItemType[4]
	{
		ItemType.KeycardResearchCoordinator,
		ItemType.Radio,
		ItemType.Medkit,
		ItemType.Painkillers
	};
}
