using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class ClaseDFugitivo : Subclass
{
	public override string Id => "clased_fugitivo";

	public override string Name => "Fugitivo";

	public override string Description => "Si estás esposado, intenta escapar: tienes un 40% de éxito y ganas velocidad durante 6 segundos. Un intento fallido cuesta 25 de vida.";

	public override RoleTypeId[] Roles => ExtraSubclasses.ClassD;

	public override float Chance => 10f;

	public override float Cooldown => 60f;

	public override string FailMessage => "No estás esposado.";

	public override bool UseAbility(Player player)
	{
		if (player == null || !player.IsAlive || !player.IsCuffed)
		{
			return false;
		}
		if (SfUtils.Roll(40f))
		{
			player.RemoveHandcuffs();
			SfUtils.SpeedBoost(player, 10f, 6f);
			SfUtils.Hint(player, "<color=#8f8><b>Te has librado de las esposas.</b> Corre.</color>");
		}
		else
		{
			SfUtils.Damage(player, 25f, "Intento de fuga fallido");
			SfUtils.Hint(player, "<color=#f66><b>Fuga fallida.</b> Las esposas resisten y pierdes 25 de vida.</color>");
		}
		return true;
	}
}
