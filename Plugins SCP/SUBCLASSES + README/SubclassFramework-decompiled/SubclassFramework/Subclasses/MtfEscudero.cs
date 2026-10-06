using System.Collections.Generic;
using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class MtfEscudero : Subclass
{
	private const float Radio = 12f;

	private const float EscudoPorPulso = 6f;

	private readonly Dictionary<Player, int> pulso = new Dictionary<Player, int>();

	public override string Id => "mtf_escudero";

	public override string Name => "Escudero";

	public override string Description => "Tus compañeros cercanos regeneran escudo poco a poco mientras sigas vivo.";

	public override RoleTypeId[] Roles => ExtraSubclasses.AllMtf;

	public override float Chance => 7f;

	public override int MaxPerWave => 1;

	public override int MaxHealth => 120;

	public override ItemType[] ExtraItems => new ItemType[1] { ItemType.ArmorCombat };

	public override void OnAssigned(Player player)
	{
		if (player != null)
		{
			pulso[player] = 0;
		}
	}

	public override void OnRemoved(Player player)
	{
		if (player != null)
		{
			pulso.Remove(player);
		}
	}

	public override void OnTick(Player player)
	{
		if (player == null || !player.IsAlive)
		{
			return;
		}
		if (!pulso.TryGetValue(player, out var value))
		{
			value = 0;
		}
		value++;
		if (value < 5)
		{
			pulso[player] = value;
			return;
		}
		pulso[player] = 0;
		foreach (Player item in SfUtils.AliveNear(player, 12f))
		{
			if (!(item == player) && SfUtils.IsMtf(item))
			{
				SfUtils.AddAhp(item, 6f, 50f);
			}
		}
	}
}
