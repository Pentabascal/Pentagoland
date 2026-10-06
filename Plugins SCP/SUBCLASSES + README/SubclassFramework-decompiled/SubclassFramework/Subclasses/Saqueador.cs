using System.Linq;
using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class Saqueador : Subclass
{
	private static readonly ItemType[] Loot = new ItemType[7]
	{
		ItemType.Medkit,
		ItemType.Painkillers,
		ItemType.Adrenaline,
		ItemType.KeycardJanitor,
		ItemType.KeycardScientist,
		ItemType.Radio,
		ItemType.Flashlight
	};

	public override string Id => "saqueador";

	public override string Name => "Saqueador";

	public override string Description => "Registra un cadáver cercano y recupera un objeto útil. A veces encuentra un segundo objeto.";

	public override RoleTypeId[] Roles => ChaosSubclasses.AllChaos;

	public override Tier AssignTier => Tier.General;

	public override float Chance => 9f;

	public override int MaxPerWave => 1;

	public override float Cooldown => 90f;

	public override string FailMessage => "No hay ningún cadáver lo bastante cerca para registrarlo.";

	public override bool UseAbility(Player player)
	{
		Ragdoll ragdoll = (from r in Ragdoll.List
			where r != null && (r.Position - player.Position).sqrMagnitude <= 9f
			orderby (r.Position - player.Position).sqrMagnitude
			select r).FirstOrDefault();
		if (ragdoll == null)
		{
			return false;
		}
		ItemType itemType = SfUtils.Pick(Loot);
		player.AddItem(itemType);
		int num = 1;
		if (SfUtils.Roll(35f))
		{
			player.AddItem(SfUtils.Pick(Loot));
			num++;
		}
		ragdoll.Destroy();
		SfUtils.Hint(player, string.Format("<color=#9f9><b>Registro completado.</b> Has encontrado {0} objeto{1}.</color>", num, (num == 1) ? "" : "s"));
		return true;
	}
}
