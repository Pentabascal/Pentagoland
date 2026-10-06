using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class GuardiaRadiotelegrafista : Subclass
{
	public override string Id => "guardia_radiotelegrafista";

	public override string Name => "Radiotelegrafista";

	public override string Description => "Envía una petición de apoyo a los MTF y científicos, indicando la zona en la que está.";

	public override RoleTypeId[] Roles => ExtraSubclasses.Guard;

	public override float Chance => 10f;

	public override int MaxPerRound => 1;

	public override float Cooldown => 60f;

	public override ItemType[] ExtraItems => new ItemType[1] { ItemType.Radio };

	public override string FailMessage => "No hay nadie de la Fundación que pueda oírte.";

	public override bool UseAbility(Player player)
	{
		if (player == null || !player.IsAlive)
		{
			return false;
		}
		string text = SfUtils.RoomName(player);
		string nickname = player.Nickname;
		List<Player> list = Player.List.Where((Player p) => p != null && p.IsAlive && p != player && (SfUtils.IsMtf(p) || p.Role.Type == RoleTypeId.Scientist)).ToList();
		if (list.Count == 0)
		{
			return false;
		}
		foreach (Player item in list)
		{
			SfUtils.Hint(item, "<size=22><color=#5DA5FF><b>RADIO</b></color></size>\n<size=18>" + nickname + " solicita apoyo en <b>" + text + "</b></size>", 7f);
		}
		SfUtils.Hint(player, $"<color=#5DA5FF>Petición enviada a {list.Count} miembros de la Fundación.</color>");
		return true;
	}
}
