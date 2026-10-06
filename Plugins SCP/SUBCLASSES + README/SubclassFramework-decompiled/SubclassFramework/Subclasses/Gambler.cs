using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class Gambler : Subclass
{
	public override string Id => "gambler";

	public override string Name => "Gambler";

	public override string Description => "Tu habilidad activa un efecto aleatorio: puedes obtener escudo, velocidad o curación, pero también recibir daño o quedar cegado.";

	public override RoleTypeId[] Roles => ChaosSubclasses.AllChaos;

	public override Tier AssignTier => Tier.General;

	public override float Chance => 10f;

	public override float Cooldown => 120f;

	public override bool UseAbility(Player player)
	{
		int num = SfUtils.RandomInt(0, 100);
		if (num < 40)
		{
			SfUtils.AddAhp(player, 10f, 300f, 0f, 0f, persistent: true);
			SfUtils.Hint(player, "<color=#8f8>+10 de escudo.</color>", 4f);
		}
		else if (num < 60)
		{
			SfUtils.SpeedBoost(player, 10f, 15f);
			SfUtils.Hint(player, "<color=#8cf>+10% de velocidad durante 15s.</color>", 4f);
		}
		else if (num < 75)
		{
			SfUtils.Heal(player, 30f);
			SfUtils.Hint(player, "<color=#8f8>+30 de vida.</color>", 4f);
		}
		else if (num < 90)
		{
			SfUtils.Damage(player, 30f, "Mala suerte");
			SfUtils.Hint(player, "<color=#f44>-30 de vida. Mala tirada.</color>", 4f);
		}
		else
		{
			player.EnableEffect(EffectType.Stained, 1, 6f);
			SfUtils.Hint(player, "<color=#f84>La mala suerte te deja cegado durante 6s.</color>", 4f);
		}
		return true;
	}
}
