using System.Collections.Generic;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public static class ChaosSubclasses
{
	public static readonly RoleTypeId[] AllChaos = new RoleTypeId[4]
	{
		RoleTypeId.ChaosConscript,
		RoleTypeId.ChaosRifleman,
		RoleTypeId.ChaosRepressor,
		RoleTypeId.ChaosMarauder
	};

	public static readonly RoleTypeId[] Repressor = new RoleTypeId[1] { RoleTypeId.ChaosRepressor };

	public static readonly RoleTypeId[] Rifleman = new RoleTypeId[1] { RoleTypeId.ChaosRifleman };

	public static readonly RoleTypeId[] Marauder = new RoleTypeId[1] { RoleTypeId.ChaosMarauder };

	public static IEnumerable<Subclass> All()
	{
		return new Subclass[22]
		{
			new Demoledor(),
			new Heavy(),
			new Saboteador(),
			new Verdugo(),
			new Asesino(),
			new Cazador(),
			new Rastreador(),
			new Duelista(),
			new Acechador(),
			new Trampero(),
			new Asaltante(),
			new CaosFrenetico(),
			new CaosReclutador(),
			new MedicoCaos(),
			new Saqueador(),
			new CaosHacker(),
			new CaosUltimoRecurso(),
			new Superviviente(),
			new Gambler(),
			new Veterano(),
			new Infiltrado(),
			new Interceptor()
		};
	}
}
