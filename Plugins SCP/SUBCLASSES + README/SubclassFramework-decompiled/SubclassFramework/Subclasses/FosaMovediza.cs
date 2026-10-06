using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class FosaMovediza : Subclass
{
	private sealed class Pozo
	{
		public Vector3 Pos;

		public float Expiry;
	}

	private readonly Dictionary<Player, Pozo> pozos = new Dictionary<Player, Pozo>();

	public override string Id => "106_fosa";

	public override string Name => "SCP-106 Fosa Movediza";

	public override string Description => "Dejas un charco corrosivo donde estás. Quien lo pise se ralentiza y pierde vida poco a poco mientras dure (12s).";

	public override RoleTypeId[] Roles => ScpSubclasses.Scp106;

	public override float Chance => 10f;

	public override float Cooldown => 120f;

	public override bool UseAbility(Player player)
	{
		pozos[player] = new Pozo
		{
			Pos = player.Position,
			Expiry = Time.time + 12f
		};
		SfUtils.Hint(player, "<color=#7a5c3e><b>Fosa movediza</b> creada bajo tus pies.</color>", 3.5f);
		return true;
	}

	public override void OnTick(Player player)
	{
		if (!pozos.TryGetValue(player, out var value))
		{
			return;
		}
		if (Time.time > value.Expiry)
		{
			pozos.Remove(player);
			return;
		}
		foreach (Player item in Player.List.Where((Player p) => p != null && p.IsAlive && p.IsHuman))
		{
			if (!((item.Position - value.Pos).sqrMagnitude > 6.25f))
			{
				SfUtils.Slow(item, 20f, 2f);
				SfUtils.Damage(item, 5f, "Fosa movediza de SCP-106");
			}
		}
	}

	public override void OnRemoved(Player player)
	{
		pozos.Remove(player);
	}
}
