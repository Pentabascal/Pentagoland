using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class ClaseDDeportista : Subclass
{
	public override string Id => "clased_deportista";

	public override string Name => "Clase D Deportista";

	public override string Description => "Recupera toda la barra de stamina al instante.";

	public override RoleTypeId[] Roles => FacilitySubclasses.ClassD;

	public override float Chance => 10f;

	public override float Cooldown => 100f;

	public override bool UseAbility(Player player)
	{
		SfUtils.RefillStamina(player);
		SfUtils.Hint(player, "<color=#8f8>Stamina al máximo.</color>", 3f);
		return true;
	}
}
