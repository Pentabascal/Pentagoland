using System.Collections.Generic;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public static class MtfSubclasses
{
	public static readonly RoleTypeId[] Privates = new RoleTypeId[2]
	{
		RoleTypeId.NtfPrivate,
		RoleTypeId.NtfSpecialist
	};

	public static readonly RoleTypeId[] Sergeants = new RoleTypeId[1] { RoleTypeId.NtfSergeant };

	public static readonly RoleTypeId[] Captains = new RoleTypeId[1] { RoleTypeId.NtfCaptain };

	public static readonly RoleTypeId[] AllMtf = new RoleTypeId[4]
	{
		RoleTypeId.NtfPrivate,
		RoleTypeId.NtfSpecialist,
		RoleTypeId.NtfSergeant,
		RoleTypeId.NtfCaptain
	};

	public static IEnumerable<Subclass> All()
	{
		return new Subclass[16]
		{
			new OperadorDeAsalto(),
			new EspecialistaTecnico(),
			new Corredor(),
			new Juggernaut(),
			new MtfHacker(),
			new FrancotiradorMtf(),
			new Zapador(),
			new MtfFrenetico(),
			new MtfRescatista(),
			new MedicoDeCombate(),
			new SargentoExplorador(),
			new TiradorDeContencion(),
			new SargentoDeArtilleria(),
			new CapitanDeMando(),
			new CapitanTactico(),
			new CapitanDeRescate()
		};
	}
}
