using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class ClaseDSuperviviente : Subclass
{
	public override string Id => "clased_superviviente";

	public override string Name => "Clase D Superviviente";

	public override string Description => "Clase D sin objetos que aparece en el antiguo punto de SCP-173.";

	public override RoleTypeId[] Roles => FacilitySubclasses.ClassD;

	public override float Chance => 10f;

	public override int MaxPerRound => 1;

	public override bool OverrideInventory => true;

	public override ItemType[] Items => Array.Empty<ItemType>();

	public override void OnAssigned(Player player)
	{
		int stamp = SubclassManager.Instance.GetAssignmentStamp(player);
		Timing.CallDelayed(0.2f, () =>
		{
			if (IsCurrentAssignment(player, stamp) && player.Role.Type == RoleTypeId.ClassD)
			{
				Room room = Room.List.FirstOrDefault((Room r) => r != null && r.Type == RoomType.Lcz173);
				SfUtils.TryTeleportToRoom(player, room);
			}
		});
	}
}
