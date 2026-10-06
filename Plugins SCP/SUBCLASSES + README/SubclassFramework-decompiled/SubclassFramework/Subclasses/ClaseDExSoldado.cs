using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class ClaseDExSoldado : Subclass
{
	public override string Id => "clased_ex_soldado";

	public override string Name => "Clase D Ex-Soldado";

	public override string Description => "Conserva el entrenamiento militar: maneja mejor las armas y los objetos gracias al efecto SCP-1853.";

	public override RoleTypeId[] Roles => FacilitySubclasses.ClassD;

	public override float Chance => 10f;

	public override void OnTick(Player player)
	{
		player.EnableEffect(EffectType.Scp1853, 1, 2f);
	}

	public override void OnRemoved(Player player)
	{
		player?.DisableEffect(EffectType.Scp1853);
	}
}
