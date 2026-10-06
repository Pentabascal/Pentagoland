using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class ClaseDLadron : Subclass
{
	public override string Id => "clased_ladron";

	public override string Name => "Clase D Ladrón";

	public override string Description => "Al apuntar a un jugador tienes un 40% de robarle un objeto que no lleve en la mano.";

	public override RoleTypeId[] Roles => FacilitySubclasses.ClassD;

	public override float Chance => 10f;

	public override float Cooldown => 90f;

	public override string FailMessage => "Apunta a un jugador que lleve algún objeto robable.";

	public override bool UseAbility(Player player)
	{
		Player target = SfUtils.GetLookingAt(player, 15f);
		if (target == null)
		{
			return false;
		}
		List<Item> list = target.Items.Where((Item item2) => item2 != null && item2 != target.CurrentItem).ToList();
		if (list.Count == 0)
		{
			SfUtils.Hint(target, "<color=#f84><b>Alguien ha intentado registrarte los bolsillos.</b></color>", 4f);
			return false;
		}
		if (!SfUtils.Roll(40f))
		{
			SfUtils.Hint(player, "<color=#f88>El robo ha fallado.</color>", 4f);
			SfUtils.Hint(target, "<color=#f84><b>Alguien ha intentado robarte.</b></color>", 4f);
			return true;
		}
		Item item = SfUtils.Pick(list);
		ItemType type = item.Type;
		target.RemoveItem(item);
		player.AddItem(type);
		SfUtils.Hint(player, $"<color=#9f9>Has robado: <b>{type}</b>.</color>", 4f);
		SfUtils.Hint(target, "<color=#f84><b>Te han robado un objeto.</b></color>", 4f);
		return true;
	}
}
