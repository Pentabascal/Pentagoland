using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class Superviviente : Subclass
{
	public override string Id => "superviviente";

	public override string Name => "Superviviente";

	public override string Description => "Conservas la calma bajo presión y recuperas toda tu stamina con tu habilidad.";

	public override RoleTypeId[] Roles => ChaosSubclasses.AllChaos;

	public override Tier AssignTier => Tier.General;

	public override float Chance => 10f;

	public override float Cooldown => 120f;

	public override bool UseAbility(Player player)
	{
		SfUtils.RefillStamina(player);
		SfUtils.Hint(player, "<color=#8f8>Stamina al máximo.</color>", 3f);
		return true;
	}
}
