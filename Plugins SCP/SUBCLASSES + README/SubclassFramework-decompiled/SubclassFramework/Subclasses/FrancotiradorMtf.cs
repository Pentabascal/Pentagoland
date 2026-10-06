using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class FrancotiradorMtf : Subclass
{
	public override string Id => "francotirador_mtf";

	public override string Name => "Francotirador MTF";

	public override string Description => "Marca un objetivo durante 10 segundos; los MTF que le disparen infligen un 20% más de daño.";

	public override RoleTypeId[] Roles => MtfSubclasses.Privates;

	public override float Chance => 10f;

	public override int MaxHealth => 100;

	public override float Cooldown => 160f;

	public override bool OverrideInventory => true;

	public override string FailMessage => "Apunta a un objetivo para marcarlo.";

	public override ItemType[] Items => new ItemType[5]
	{
		ItemType.GunFRMG0,
		ItemType.ArmorLight,
		ItemType.Radio,
		ItemType.KeycardMTFOperative,
		ItemType.Medkit
	};

	public override KeyValuePair<AmmoType, ushort>[] Ammo => new KeyValuePair<AmmoType, ushort>[1] { Subclass.A(AmmoType.Nato762, 60) };

	public override bool UseAbility(Player player)
	{
		Player lookingAt = SfUtils.GetLookingAt(player, 120f);
		if (lookingAt == null)
		{
			return false;
		}
		GlobalHooks.MarkTarget(lookingAt, 10f, 1.2f, Team.FoundationForces);
		lookingAt.EnableEffect(EffectType.Burned, 1, 10f);
		SfUtils.Hint(player, "<color=#6cf><b>Objetivo marcado:</b> " + lookingAt.Nickname + "</color>");
		foreach (Player item in Player.List.Where(SfUtils.IsMtf))
		{
			SfUtils.Hint(item, "<color=#6cf>Objetivo marcado por el francotirador: <b>" + lookingAt.Nickname + "</b> (" + SfUtils.RoomName(lookingAt) + ")</color>");
		}
		return true;
	}
}
