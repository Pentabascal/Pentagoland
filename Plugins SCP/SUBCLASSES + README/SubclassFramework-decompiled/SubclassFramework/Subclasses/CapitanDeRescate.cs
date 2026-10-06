using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class CapitanDeRescate : Subclass
{
	public override string Id => "capitan_rescate";

	public override string Name => "Capitán de Rescate";

	public override string Description => "Reagrupa al escuadron: recupera vida, escudo y stamina de los MTF cercanos para volver a entrar en combate.";

	public override RoleTypeId[] Roles => MtfSubclasses.Captains;

	public override float Chance => 10f;

	public override int MaxHealth => 115;

	public override float Cooldown => 180f;

	public override bool OverrideInventory => true;

	public override ItemType[] Items => new ItemType[6]
	{
		ItemType.GunCrossvec,
		ItemType.ArmorCombat,
		ItemType.Radio,
		ItemType.KeycardMTFCaptain,
		ItemType.Medkit,
		ItemType.Adrenaline
	};

	public override KeyValuePair<AmmoType, ushort>[] Ammo => new KeyValuePair<AmmoType, ushort>[1] { Subclass.A(AmmoType.Nato9, 180) };

	public override string FailMessage => "No hay MTF suficientes cerca para reagruparlos.";

	public override bool UseAbility(Player player)
	{
		List<Player> list = SfUtils.AliveNear(player, 12f).Where(SfUtils.IsMtf).ToList();
		if (list.Count == 0)
		{
			return false;
		}
		foreach (Player item in list)
		{
			SfUtils.Heal(item, 20f);
			SfUtils.AddAhp(item, 25f, 75f, 0f, 15f);
			SfUtils.RefillStamina(item);
			if (item != player)
			{
				SfUtils.Hint(item, "<color=#8cf><b>Reagrupación.</b> El Capitán te ha devuelto al combate.</color>");
			}
		}
		SfUtils.Hint(player, $"<color=#8cf><b>Rescate completado.</b> {list.Count} MTF reagrupados.</color>");
		return true;
	}
}
