using System.Collections.Generic;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public static class ScpSubclasses
{
	public static readonly RoleTypeId[] Scp939 = new RoleTypeId[1] { RoleTypeId.Scp939 };

	public static readonly RoleTypeId[] Scp0492 = new RoleTypeId[1] { RoleTypeId.Scp0492 };

	public static readonly RoleTypeId[] Scp106 = new RoleTypeId[1] { RoleTypeId.Scp106 };

	public static readonly RoleTypeId[] Scp049 = new RoleTypeId[1] { RoleTypeId.Scp049 };

	public static IEnumerable<Subclass> All()
	{
		return new Subclass[27]
		{
			new Devorador(),
			new Bestia(),
			new CazadorNocturno(),
			new Zomboss(),
			new ZombiBebe(),
			new ZombiTanque(),
			new ZombiInfeccioso(),
			new ZombiRalentizador(),
			new ZombiNocturno(),
			new ZombiCamuflado(),
			new ZombiRegenerador(),
			new ZombiAullador(),
			new ZombiKamikaze(),
			new ZombiUltimoAliento(),
			new ZombiHemorragico(),
			new ZombiResonante(),
			new CazadorSilencioso(),
			new GuardianDeLaFosa(),
			new Acosador(),
			new Corruptor(),
			new FosaMovediza(),
			new Stalker(),
			new Cirujano(),
			new Purista(),
			new Coleccionista(),
			new Plaga(),
			new ZombiMiasmatico()
		};
	}
}
