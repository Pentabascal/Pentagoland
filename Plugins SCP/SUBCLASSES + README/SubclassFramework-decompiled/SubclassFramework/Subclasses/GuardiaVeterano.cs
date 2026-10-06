using System.Collections.Generic;
using Exiled.API.Enums;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class GuardiaVeterano : Subclass
{
	public override string Id => "guardia_veterano";

	public override string Name => "Guardia Veterano";

	public override string Description => "Guardia veterano con mejor equipamiento y 110 de vida.";

	public override RoleTypeId[] Roles => new RoleTypeId[1] { RoleTypeId.FacilityGuard };

	public override float Chance => 9f;

	public override int MaxHealth => 110;

	public override int MaxPerRound => 2;

	public override bool OverrideInventory => true;

	public override ItemType[] Items => new ItemType[5]
	{
		ItemType.GunCrossvec,
		ItemType.ArmorCombat,
		ItemType.KeycardGuard,
		ItemType.Medkit,
		ItemType.Medkit
	};

	public override KeyValuePair<AmmoType, ushort>[] Ammo => new KeyValuePair<AmmoType, ushort>[1] { Subclass.A(AmmoType.Nato9, 90) };
}
