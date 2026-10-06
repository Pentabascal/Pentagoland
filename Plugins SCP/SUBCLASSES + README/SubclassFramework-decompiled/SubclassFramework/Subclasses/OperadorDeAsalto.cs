using System.Collections.Generic;
using Exiled.API.Enums;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class OperadorDeAsalto : Subclass
{
	public override string Id => "operador_asalto";

	public override string Name => "Operador de Asalto";

	public override string Description => "Soldado de combate mejorado, con equipo reforzado y mayor resistencia.";

	public override RoleTypeId[] Roles => MtfSubclasses.Privates;

	public override float Chance => 10f;

	public override int MaxHealth => 110;

	public override bool OverrideInventory => true;

	public override ItemType[] Items => new ItemType[8]
	{
		ItemType.GunCrossvec,
		ItemType.GunCOM18,
		ItemType.ArmorHeavy,
		ItemType.Radio,
		ItemType.GrenadeHE,
		ItemType.Medkit,
		ItemType.Adrenaline,
		ItemType.KeycardMTFOperative
	};

	public override KeyValuePair<AmmoType, ushort>[] Ammo => new KeyValuePair<AmmoType, ushort>[2]
	{
		Subclass.A(AmmoType.Nato556, 60),
		Subclass.A(AmmoType.Nato9, 200)
	};
}
