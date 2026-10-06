using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class ClaseDCarterista : Subclass
{
	public override string Id => "clased_carterista";

	public override string Name => "Clase D Carterista";

	public override string Description => "Apunta a alguien y tienes un 70% de robarle un objeto que no lleve en la mano.";

	public override RoleTypeId[] Roles => FacilitySubclasses.ClassD;

	public override float Chance => 10f;

	public override float Cooldown => 120f;

	public override string FailMessage => "Apunta a un jugador para robarle.";

	public override bool UseAbility(Player player)
	{
		Player target = SfUtils.GetLookingAt(player, 15f);
		if (target == null)
		{
			return false;
		}
		if (!SfUtils.Roll(70f))
		{
			SfUtils.Hint(player, "<color=#f88>Se te ha escurrido. Te ha pillado.</color>");
			SfUtils.Hint(target, "<color=#f84><b>Alguien ha intentado robarte.</b></color>");
			return true;
		}
		List<Item> list = target.Items.Where((Item i) => i != null && i != target.CurrentItem).ToList();
		if (list.Count == 0)
		{
			SfUtils.Hint(player, "<color=#aaa>No lleva nada que puedas robar.</color>");
			SfUtils.Hint(target, "<color=#f84><b>Alguien ha intentado registrarte los bolsillos.</b></color>");
			return true;
		}
		Item item = SfUtils.Pick(list);
		ItemType type = item.Type;
		target.RemoveItem(item);
		player.AddItem(type);
		SfUtils.Hint(player, $"<color=#9f9>Robado: <b>{type}</b></color>");
		return true;
	}
}
