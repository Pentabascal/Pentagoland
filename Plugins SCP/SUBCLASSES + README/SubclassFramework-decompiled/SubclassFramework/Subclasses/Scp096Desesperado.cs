using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class Scp096Desesperado : Subclass
{
	public override string Id => "scp096_desesperado";

	public override string Name => "SCP-096 Desesperado";

	public override string Description => "Cuando su vida baja del 45%, se vuelve más rápido y peligroso, pero también recibe más daño.";

	public override RoleTypeId[] Roles => ExtraSubclasses.Scp096;

	public override float Chance => 6f;

	public override int MaxHealth => 2800;

	public override void OnTick(Player player)
	{
		if (player.Health <= player.MaxHealth * 0.45f)
		{
			SfUtils.SpeedBoost(player, 15f, 2f);
		}
	}

	public override void OnDealingDamage(Player attacker, HurtingEventArgs ev)
	{
		if (attacker.Health <= attacker.MaxHealth * 0.45f)
		{
			ev.Amount *= 1.25f;
		}
	}

	public override void OnTakingDamage(Player victim, HurtingEventArgs ev)
	{
		if (victim.Health <= victim.MaxHealth * 0.45f)
		{
			ev.Amount *= 1.1f;
			victim.EnableEffect(EffectType.Burned, 1, 2f);
		}
	}
}
