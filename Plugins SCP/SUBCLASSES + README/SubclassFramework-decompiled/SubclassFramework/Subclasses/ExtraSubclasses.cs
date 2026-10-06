using System.Collections.Generic;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public static class ExtraSubclasses
{
	public static readonly RoleTypeId[] Guard = new RoleTypeId[1] { RoleTypeId.FacilityGuard };

	public static readonly RoleTypeId[] Scientist = new RoleTypeId[1] { RoleTypeId.Scientist };

	public static readonly RoleTypeId[] ClassD = new RoleTypeId[1] { RoleTypeId.ClassD };

	public static readonly RoleTypeId[] Scp096 = new RoleTypeId[1] { RoleTypeId.Scp096 };

	public static readonly RoleTypeId[] AllMtf = new RoleTypeId[4]
	{
		RoleTypeId.NtfPrivate,
		RoleTypeId.NtfSergeant,
		RoleTypeId.NtfSpecialist,
		RoleTypeId.NtfCaptain
	};

	public static readonly RoleTypeId[] AllChaos = new RoleTypeId[4]
	{
		RoleTypeId.ChaosConscript,
		RoleTypeId.ChaosRepressor,
		RoleTypeId.ChaosRifleman,
		RoleTypeId.ChaosMarauder
	};

	public static IEnumerable<Subclass> All()
	{
		return new Subclass[11]
		{
			new GuardiaRadiotelegrafista(),
			new GuardiaVeterano(),
			new CientificoBioquimico(),
			new ClaseDSoplon(),
			new ClaseDFugitivo(),
			new ClaseDFalsificador(),
			new MtfEscudero(),
			new Scp096Timido(),
			new Scp096Fragil(),
			new Scp096Lamento(),
			new Scp096Desesperado()
		};
	}
}
