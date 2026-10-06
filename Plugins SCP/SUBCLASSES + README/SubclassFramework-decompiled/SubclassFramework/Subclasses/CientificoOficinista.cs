using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class CientificoOficinista : Subclass
{
	public override string Id => "cientifico_oficinista";

	public override string Name => "Científico Oficinista";

	public override string Description => "Científico de oficina con tarjeta de gerente de zona, COM-15 y un botiquín.";

	public override RoleTypeId[] Roles => FacilitySubclasses.Scientist;

	public override float Chance => 10f;

	public override int MaxPerRound => 1;

	public override bool OverrideInventory => true;

	public override ItemType[] Items => new ItemType[3]
	{
		ItemType.KeycardZoneManager,
		ItemType.GunCOM15,
		ItemType.Medkit
	};

	public override KeyValuePair<AmmoType, ushort>[] Ammo => new KeyValuePair<AmmoType, ushort>[1] { Subclass.A(AmmoType.Nato9, 18) };

	public override void OnAssigned(Player player)
	{
		int stamp = SubclassManager.Instance.GetAssignmentStamp(player);
		Timing.CallDelayed(0.8f, () =>
		{
			if (IsCurrentAssignment(player, stamp) && player.Role.Type == RoleTypeId.Scientist)
			{
				Room room = Room.List.FirstOrDefault((Room room2) => room2 != null && room2.Zone == ZoneType.Entrance && room2.Type.ToString().IndexOf("Office", StringComparison.OrdinalIgnoreCase) >= 0);
				SfUtils.TryTeleportToRoom(player, room);
			}
		});
	}
}
