using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class Acosador : Subclass
{
	private readonly Dictionary<Player, int> traumaKills = new Dictionary<Player, int>();

	private readonly Dictionary<(Player Killer, Player Victim), float> traumatizedVictims = new Dictionary<(Player, Player), float>();

	public override string Id => "106_acosador";

	public override string Name => "SCP-106 Acosador";

	public override string Description => "Cada presa traumatizada que caiga te hace más fuerte. Pasadas 5, empiezas a acumular escudo que no decae. Empiezas algo más duro.";

	public override RoleTypeId[] Roles => ScpSubclasses.Scp106;

	public override float Chance => 10f;

	public override void OnAssigned(Player player)
	{
		int stamp = SubclassManager.Instance.GetAssignmentStamp(player);
		Timing.CallDelayed(1f, () =>
		{
			if (IsCurrentAssignment(player, stamp))
			{
				SfUtils.SetMaxHealth(player, player.MaxHealth * 1.05f, healToFull: true);
			}
		});
	}

	public override void OnKill(Player killer, Player victim, DiedEventArgs ev)
	{
		if (killer == null || victim == null)
		{
			return;
		}

		bool wasRecordedTraumatized = traumatizedVictims.TryGetValue((killer, victim), out float traumaExpiry) && Time.time <= traumaExpiry;
		traumatizedVictims.Remove((killer, victim));
		bool wasTraumatized = SfUtils.HasTraumatized(victim) || wasRecordedTraumatized;
		if (wasTraumatized && (victim.IsHuman || wasRecordedTraumatized))
		{
			int num = ((!traumaKills.TryGetValue(killer, out var value)) ? 1 : (value + 1));
			traumaKills[killer] = num;
			if (num <= 5)
			{
				SfUtils.SetMaxHealth(killer, killer.MaxHealth + 35f, healToFull: false);
				SfUtils.SpeedBoost(killer, 3f * (float)num, 600f);
				SfUtils.Hint(killer, $"<color=#a6f><b>Acoso {num}/5.</b> +35 de vida máxima y más velocidad.</color>", 6f);
			}
			else
			{
				SfUtils.AddPermanentAhp(killer, 200f, 3000f);
				SfUtils.Hint(killer, $"<color=#a6f><b>Acoso {num}.</b> +200 de escudo permanente.</color>", 6f);
			}
		}
	}

	public override void OnDealingDamage(Player attacker, HurtingEventArgs ev)
	{
		if (attacker == null || ev?.Player == null)
		{
			return;
		}

		if (ev.Player.IsHuman && ev.Player.TryGetEffect(Exiled.API.Enums.EffectType.Traumatized, out var traumaEffect) &&
			traumaEffect != null && traumaEffect.IsEnabled)
		{
			traumatizedVictims[(attacker, ev.Player)] = Time.time + Mathf.Max(0f, traumaEffect.TimeLeft);
		}
	}

	public override void OnRemoved(Player player)
	{
		traumaKills.Remove(player);
		foreach (var pair in new List<(Player Killer, Player Victim)>(traumatizedVictims.Keys))
		{
			if (pair.Killer == player || pair.Victim == player)
			{
				traumatizedVictims.Remove(pair);
			}
		}
	}
}
