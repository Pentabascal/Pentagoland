using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class Plaga : Subclass
{
	public override string Id => "049_plaga";

	public override string Name => "SCP-049 Plaga";

	public override string Description => "Cada vez que revives a un zombi recuperas 50 de vida.";

	public override RoleTypeId[] Roles => ScpSubclasses.Scp049;

	public override float Chance => 10f;

	public override int MaxPerRound => 1;

	public static void OnRevived(Player player)
	{
		if (player == null || !player.IsAlive || player.Role.Type != RoleTypeId.Scp049 || ZombieReviveTracker.Count(player) <= 0)
		{
			return;
		}
		SfUtils.Heal(player, 50f);
		SfUtils.Hint(player, "<color=#8f8><b>La plaga se extiende.</b> Has recuperado 50 de vida.</color>");
	}
}
