using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class MedicoCaos : Subclass
{
	public override string Id => "medico_caos";

	public override string Name => "Médico";

	public override string Description => "Empiezas con un botiquín y puedes crear otro cada 90 segundos.";

	public override RoleTypeId[] Roles => ChaosSubclasses.AllChaos;

	public override Tier AssignTier => Tier.General;

	public override float Chance => 10f;

	public override float Cooldown => 90f;

	public override ItemType[] ExtraItems => new ItemType[1] { ItemType.Medkit };

	public override string FailMessage => "No tienes espacio para guardar otro botiquín.";

	public override bool UseAbility(Player player)
	{
		if (player.AddItem(ItemType.Medkit) == null)
		{
			return false;
		}
		SfUtils.Hint(player, "<color=#8cf><b>Botiquín generado.</b> Lo tienes en tu inventario.</color>");
		return true;
	}
}
