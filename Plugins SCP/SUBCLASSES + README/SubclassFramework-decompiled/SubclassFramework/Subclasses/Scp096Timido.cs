using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class Scp096Timido : Subclass
{
	public override string Id => "scp096_timido";

	public override string Name => "SCP-096 Tímido";

	public override string Description => "Cada humano que matas te devuelve 50 de vida.";

	public override RoleTypeId[] Roles => ExtraSubclasses.Scp096;

	public override float Chance => 10f;

	public override void OnKill(Player killer, Player victim, DiedEventArgs ev)
	{
		if (!(killer == null) && killer.IsAlive && !(victim == null) && victim.IsHuman)
		{
			SfUtils.Heal(killer, 50f);
			SfUtils.Hint(killer, "<color=#FF4040>+50 de vida.</color>", 2.5f);
		}
	}
}
