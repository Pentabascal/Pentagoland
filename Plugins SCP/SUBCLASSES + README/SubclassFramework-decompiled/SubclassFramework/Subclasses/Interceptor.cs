using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class Interceptor : Subclass
{
	private readonly Dictionary<Player, float> scanUntil = new Dictionary<Player, float>();

	public override string Id => "interceptor";

	public override string Name => "Interceptor";

	public override string Description => "Durante 15 segundos intercepta las comunicaciones y muestra dónde se encuentran los miembros de la Fundación.";

	public override RoleTypeId[] Roles => ChaosSubclasses.AllChaos;

	public override Tier AssignTier => Tier.General;

	public override float Chance => 10f;

	public override float Cooldown => 180f;

	public override ItemType[] ExtraItems => new ItemType[1] { ItemType.Radio };

	public override bool UseAbility(Player player)
	{
		scanUntil[player] = Time.time + 15f;
		SfUtils.Hint(player, "<color=#6cf><b>Escaneando frecuencias...</b></color>", 4f);
		return true;
	}

	public override void OnTick(Player player)
	{
		if (!scanUntil.TryGetValue(player, out var value))
		{
			return;
		}
		if (Time.time > value)
		{
			scanUntil.Remove(player);
			SfUtils.Hint(player, "<color=#6cf>Senal perdida.</color>", 3f);
			return;
		}
		List<string> list = (from p in (from p in Player.List
				where p != null && p.IsAlive && (SfUtils.IsMtf(p) || SfUtils.IsGuard(p))
				orderby (p.Position - player.Position).sqrMagnitude
				select p).Take(3)
			select $"{p.Role.Type} - {SfUtils.RoomName(p)}").ToList();
		string text = ((list.Count == 0) ? "Sin señales de la Fundación." : string.Join("\n", list));
		SfUtils.Hint(player, "<size=18><color=#6cf><b>Intercepción</b>\n" + text + "</color></size>", 1.2f);
	}

	public override void OnRemoved(Player player)
	{
		scanUntil.Remove(player);
	}
}
