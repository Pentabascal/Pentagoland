using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class ZombiNocturno : Subclass
{
	public override string Id => "zombi_nocturno";

	public override string Name => "Zombi Nocturno";

	public override string Description => "En la oscuridad gana +15% de velocidad.";

	public override RoleTypeId[] Roles => ScpSubclasses.Scp0492;

	public override float Chance => 10f;

	public override void OnTick(Player player)
	{
		if (SfUtils.IsInDarkness(player))
		{
			SfUtils.SpeedBoost(player, 15f, 2f);
		}
	}
}
