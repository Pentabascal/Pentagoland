using System.Collections.Generic;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public static class FacilitySubclasses
{
	public static readonly RoleTypeId[] Guard = new RoleTypeId[1] { RoleTypeId.FacilityGuard };

	public static readonly RoleTypeId[] Scientist = new RoleTypeId[1] { RoleTypeId.Scientist };

	public static readonly RoleTypeId[] ClassD = new RoleTypeId[1] { RoleTypeId.ClassD };

	public static IEnumerable<Subclass> All()
	{
		return new Subclass[22]
		{
			new TecnicoDeBrecha(),
			new Sanitario(),
			new GuardiaCoordinador(),
			new GuardiaArmero(),
			new GuardiaExplorador(),
			new CientificoExperto(),
			new CientificoLoco(),
			new CientificoDeCampo(),
			new CientificoAnsioso(),
			new CientificoOficinista(),
			new CientificoAnalista(),
			new ClaseDDeportista(),
			new ClaseDChatarrero(),
			new ClaseDCarterista(),
			new ClaseDAmotinado(),
			new ClaseDResiliente(),
			new ClaseDEspiaIC(),
			new ClaseDLadron(),
			new ClaseDExSoldado(),
			new ClaseDObservador(),
			new ClaseDConserje(),
			new ClaseDSuperviviente()
		};
	}
}
