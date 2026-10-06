using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class TecnicoDeBrecha : Subclass
{
	public override string Id => "tecnico_brecha";

	public override string Name => "Técnico de Brecha";

	public override string Description => "Sabe qué SCP han escapado y puede consultar el parte de contención.";

	public override RoleTypeId[] Roles => FacilitySubclasses.Guard;

	public override float Chance => 10f;

	public override int MaxPerRound => 1;

	public override float Cooldown => 30f;

	public override void OnAssigned(Player player)
	{
		int stamp = SubclassManager.Instance.GetAssignmentStamp(player);
		Timing.CallDelayed(3f, () =>
		{
			if (IsCurrentAssignment(player, stamp))
			{
				Report(player);
			}
		});
	}

	public override bool UseAbility(Player player)
	{
		Report(player);
		return true;
	}

	private static void Report(Player player)
	{
		if (!(player == null) && player.IsAlive)
		{
			List<string> list = (from p in Player.List
				where p != null && p.IsAlive && p.IsScp
				select Describe(p.Role.Type)).Distinct().ToList();
			string text = ((list.Count == 0) ? "Ningún SCP fuera de contención." : string.Join("\n", list));
			SfUtils.Hint(player, "<size=22><color=#f88><b>PARTE DE CONTENCION</b></color></size>\n<size=19>" + text + "</size>", 8f);
		}
	}

	private static string Describe(RoleTypeId role)
	{
		return role switch
		{
			RoleTypeId.Scp049 => "SCP-049 - brecha confirmada", 
			RoleTypeId.Scp0492 => "SCP-049-2 - infectados sueltos", 
			RoleTypeId.Scp079 => "SCP-079 - control de sistem", 
			RoleTypeId.Scp096 => "SCP-096 - brecha confirmada", 
			RoleTypeId.Scp106 => "SCP-106 - brecha confirmada", 
			RoleTypeId.Scp173 => "SCP-173 - brecha confirmada", 
			RoleTypeId.Scp939 => "SCP-939 - brecha confirmada", 
			_ => $"{role} - brecha confirmada", 
		};
	}
}
