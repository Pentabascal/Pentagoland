using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class TiradorDeContencion : Subclass
{
	public override string Id => "tirador_contencion";

	public override string Name => "Tirador de Contención";

	public override string Description => "Cada bala de revólver ralentiza al objetivo y le deja un veneno de 60 de daño en 15s.";

	public override RoleTypeId[] Roles => MtfSubclasses.Sergeants;

	public override float Chance => 10f;

	public override int MaxHealth => 110;

	public override bool OverrideInventory => true;

	public override ItemType[] Items => new ItemType[6]
	{
		ItemType.KeycardMTFOperative,
		ItemType.GunCrossvec,
		ItemType.GunRevolver,
		ItemType.ArmorCombat,
		ItemType.Medkit,
		ItemType.Adrenaline
	};

	public override KeyValuePair<AmmoType, ushort>[] Ammo => new KeyValuePair<AmmoType, ushort>[2]
	{
		Subclass.A(AmmoType.Nato9, 160),
		Subclass.A(AmmoType.Ammo44Cal, 1)
	};

	public override void OnAssigned(Player player)
	{
		Firearm firearm = player.Items.OfType<Firearm>().FirstOrDefault((Firearm f) => f.Type == ItemType.GunRevolver);
		if (firearm != null)
		{
			firearm.MaxMagazineAmmo = 1;
			firearm.MagazineAmmo = 1;
		}
	}

	public override void OnDealingDamage(Player attacker, HurtingEventArgs ev)
	{
		if (attacker.CurrentItem != null && attacker.CurrentItem.Type == ItemType.GunRevolver)
		{
			Player player = ev.Player;
			if (!(player == null) && player.IsAlive)
			{
				SfUtils.Slow(player, 15f, 5f);
				int assignmentStamp = SubclassManager.Instance.GetAssignmentStamp(player);
				Timing.RunCoroutine(Poison(player, player.Role.Type, assignmentStamp), $"SubclassFramework.Poison.{player.Id}");
				SfUtils.Hint(player, "<color=#8f4><b>Envenenado.</b> El daño seguirá durante 15 segundos.</color>");
				SfUtils.Hint(attacker, "<color=#8f4>Impacto de contención aplicado.</color>", 3f);
			}
		}
	}

	private IEnumerator<float> Poison(Player target, RoleTypeId expectedRole, int stamp)
	{
		for (int i = 0; i < 3; i++)
		{
			yield return Timing.WaitForSeconds(5f);
			if (!IsCurrentAssignment(target, stamp) || target.Role.Type != expectedRole)
			{
				break;
			}
			SfUtils.Damage(target, 20f, "Munición de contención");
		}
	}
}
