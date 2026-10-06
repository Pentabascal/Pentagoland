using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class MedicoDeCombate : Subclass
{
	public override string Id => "medico_combate";

	public override string Name => "Médico de Combate";

	public override string Description => "Cura al escuadrón entero de su sala: 20% de vida repartido en 5s y 10 de escudo al terminar.";

	public override RoleTypeId[] Roles => MtfSubclasses.Sergeants;

	public override float Chance => 10f;

	public override int MaxHealth => 100;

	public override float Cooldown => 105f;

	public override bool OverrideInventory => true;

	public override ItemType[] Items => new ItemType[7]
	{
		ItemType.KeycardMTFOperative,
		ItemType.GunE11SR,
		ItemType.Medkit,
		ItemType.Medkit,
		ItemType.Adrenaline,
		ItemType.Radio,
		ItemType.ArmorCombat
	};

	public override KeyValuePair<AmmoType, ushort>[] Ammo => new KeyValuePair<AmmoType, ushort>[1] { Subclass.A(AmmoType.Nato556, 120) };

	public override bool UseAbility(Player player)
	{
		List<Player> list = SfUtils.InSameRoom(player).Where(SfUtils.IsMtf).ToList();
		foreach (Player item in list)
		{
			int assignmentStamp = SubclassManager.Instance.GetAssignmentStamp(item);
			Timing.RunCoroutine(HealOverTime(item, item.Role.Type, assignmentStamp), $"SubclassFramework.Heal.{item.Id}");
			if (item != player)
			{
				SfUtils.Hint(item, "<color=#8f8>El médico de combate te está tratando.</color>");
			}
		}
		player.AddItem(ItemType.Medkit);
		SfUtils.Hint(player, $"<color=#8f8><b>Triaje de combate:</b> {list.Count} soldados atendidos. +1 botiquín.</color>");
		return true;
	}

	private IEnumerator<float> HealOverTime(Player target, RoleTypeId expectedRole, int stamp)
	{
		for (int i = 0; i < 5; i++)
		{
			yield return Timing.WaitForSeconds(1f);
			if (!IsCurrentAssignment(target, stamp) || target.Role.Type != expectedRole)
			{
				yield break;
			}
			SfUtils.Heal(target, target.MaxHealth * 0.04f);
		}
		if (IsCurrentAssignment(target, stamp) && target.Role.Type == expectedRole)
		{
			SfUtils.AddAhp(target, 10f, 100f, 0f, 15f);
		}
	}
}
