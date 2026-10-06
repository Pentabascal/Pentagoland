using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class ZombiUltimoAliento : Subclass
{
	private readonly Dictionary<Player, float> nextEscape = new Dictionary<Player, float>();

	private readonly HashSet<Player> hidden = new HashSet<Player>();

	public override string Id => "zombi_ultimo_aliento";

	public override string Name => "Zombi de Último Aliento";

	public override string Description => "Cuando baja del 20% de vida, se vuelve invisible durante 6 segundos. Solo puede ocurrir una vez cada 90 segundos.";

	public override RoleTypeId[] Roles => ScpSubclasses.Scp0492;

	public override float Chance => 10f;

	public override int MaxPerWave => 1;

	public override void OnTick(Player player)
	{
		if (player == null || !player.IsAlive || hidden.Contains(player) || player.Health > player.MaxHealth * 0.2f || (nextEscape.TryGetValue(player, out var value) && Time.time < value))
		{
			return;
		}
		hidden.Add(player);
		nextEscape[player] = Time.time + 90f;
		player.EnableEffect(EffectType.Invisible, 1, 6f);
		SfUtils.Hint(player, "<color=#9f9><b>Último aliento.</b> Te has vuelto invisible durante 6 segundos.</color>");
		Timing.CallDelayed(6.2f, () =>
		{
			hidden.Remove(player);
			if (player != null)
			{
				player.DisableEffect(EffectType.Invisible);
			}
		});
	}

	public override void OnDeath(Player player, DiedEventArgs ev)
	{
		hidden.Remove(player);
		player?.DisableEffect(EffectType.Invisible);
	}

	public override void OnRemoved(Player player)
	{
		hidden.Remove(player);
		nextEscape.Remove(player);
		player?.DisableEffect(EffectType.Invisible);
	}
}
