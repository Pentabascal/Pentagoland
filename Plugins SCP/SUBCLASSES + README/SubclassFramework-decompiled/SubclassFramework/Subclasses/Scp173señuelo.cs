using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class Scp173señuelo : Subclass
{
	public override string Id => "173_se";

	public override string Name => "173 Señuelo";

	public override string Description => "Crea un señuelo de 400 HP durante 60 segundos para distraer al escuadrón.";

	public override RoleTypeId[] Roles => Scp173Module.Scp173;

	public override float Chance => 5f;

	public override int MaxHealth => 3500;

	public override float Cooldown => 220f;

	public override string FailMessage => "No se ha podido colocar el señuelo aquí.";

	public override bool UseAbility(Player player)
	{
		return Scp173Module.SpawnDecoy(player, 400f, 60f);
	}
}
