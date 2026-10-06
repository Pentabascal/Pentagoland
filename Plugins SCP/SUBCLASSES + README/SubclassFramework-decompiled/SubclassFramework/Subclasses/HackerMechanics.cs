using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

internal static class HackerMechanics
{
	private const int ActivationProgressSeconds = 25;
	private const int SpeedPerActivatedGenerator = 6;
	private const float MovementEffectRefreshDuration = 2f;

	private sealed class MovementBoostState
	{
		public byte BaseIntensity;

		public bool BaseIsPermanent;

		public float BaseExpiresAt;

		public byte AppliedIntensity;

		public float AppliedExpiresAt;
	}

	private static readonly HashSet<Generator> AcceleratedGenerators = new HashSet<Generator>();

	private static readonly Dictionary<Player, MovementBoostState> MovementBoosts = new Dictionary<Player, MovementBoostState>();

	public static void ScheduleGeneratorAcceleration(Player player, Generator generator)
	{
		if (player == null || generator == null)
		{
			return;
		}

		Timing.CallDelayed(0.1f, () =>
		{
			if (player == null || !player.IsAlive || generator == null || !generator.IsActivating || generator.IsEngaged)
			{
				return;
			}

			Subclass assigned = SubclassManager.Instance?.GetSubclass(player);
			if (assigned is MtfHacker or CaosHacker)
			{
				AccelerateGenerator(player, generator);
			}
		});
	}

	public static bool AccelerateGenerator(Player player, Generator generator)
	{
		if (player == null || generator == null || AcceleratedGenerators.Contains(generator))
		{
			return false;
		}

		var baseGenerator = generator.Base;
		float totalActivationTime = baseGenerator.TotalActivationTime;
		int remainingTime = baseGenerator.RemainingTime;

		if (!generator.IsActivating || generator.IsEngaged)
		{
			return false;
		}

		if (remainingTime == 0)
		{
			remainingTime = Mathf.FloorToInt(totalActivationTime);
		}

		if (remainingTime < ActivationProgressSeconds || remainingTime > short.MaxValue)
		{
			return false;
		}

		baseGenerator.RemainingTime = (short)(remainingTime - ActivationProgressSeconds);
		AcceleratedGenerators.Add(generator);
		SfUtils.Hint(player, "<color=#8cf><b>Hackeo activo.</b> Has adelantado 25 segundos la activación de este generador.</color>");
		return true;
	}

	public static void Tick(Player player)
	{
		if (player == null)
		{
			return;
		}

		int activatedGenerators = Generator.List.Count(generator => generator != null && generator.IsEngaged);
		SetMovementBonus(player, activatedGenerators * SpeedPerActivatedGenerator);
	}

	public static void Remove(Player player)
	{
		SetMovementBonus(player, 0);
	}

	public static void ResetRound()
	{
		foreach (Player player in MovementBoosts.Keys.ToList())
		{
			SetMovementBonus(player, 0);
		}

		MovementBoosts.Clear();
		AcceleratedGenerators.Clear();
	}

	private static void SetMovementBonus(Player player, int bonusPercent)
	{
		if (player == null)
		{
			return;
		}

		var effect = player.GetEffect(EffectType.MovementBoost);
		if (!MovementBoosts.TryGetValue(player, out MovementBoostState state))
		{
			if (bonusPercent <= 0 || effect == null)
			{
				return;
			}

			state = new MovementBoostState();
			CaptureBaseEffect(state, effect);
			MovementBoosts.Add(player, state);
		}
		else if (!IsOwnedEffect(effect, state))
		{
			CaptureBaseEffect(state, effect);
			state.AppliedIntensity = 0;
			if (bonusPercent <= 0)
			{
				MovementBoosts.Remove(player);
				return;
			}
		}

		if (bonusPercent <= 0)
		{
			RestoreBaseEffect(player, effect, state);
			MovementBoosts.Remove(player);
			return;
		}

		if (state.BaseIntensity > 0 && !state.BaseIsPermanent && Time.time >= state.BaseExpiresAt)
		{
			state.BaseIntensity = 0;
			state.BaseExpiresAt = 0f;
		}

		byte combinedIntensity = (byte)Mathf.Min(byte.MaxValue, state.BaseIntensity + bonusPercent);
		player.EnableEffect(EffectType.MovementBoost, combinedIntensity, MovementEffectRefreshDuration);
		state.AppliedIntensity = combinedIntensity;
		state.AppliedExpiresAt = Time.time + MovementEffectRefreshDuration;
	}

	private static void CaptureBaseEffect(MovementBoostState state, CustomPlayerEffects.StatusEffectBase effect)
	{
		state.BaseIntensity = effect != null && effect.IsEnabled ? effect.Intensity : (byte)0;
		state.BaseIsPermanent = state.BaseIntensity > 0 && effect.Duration <= 0f;
		state.BaseExpiresAt = state.BaseIntensity > 0 && !state.BaseIsPermanent
			? Time.time + Mathf.Max(0f, effect.TimeLeft)
			: 0f;
	}

	private static void RestoreBaseEffect(Player player, CustomPlayerEffects.StatusEffectBase effect, MovementBoostState state)
	{
		if (!IsOwnedEffect(effect, state))
		{
			return;
		}

		if (state.BaseIntensity == 0)
		{
			player.DisableEffect(EffectType.MovementBoost);
		}
		else if (state.BaseIsPermanent)
		{
			player.EnableEffect(EffectType.MovementBoost, state.BaseIntensity);
		}
		else
		{
			float remainingDuration = state.BaseExpiresAt - Time.time;
			if (remainingDuration > 0f)
			{
				player.EnableEffect(EffectType.MovementBoost, state.BaseIntensity, remainingDuration);
			}
			else
			{
				player.DisableEffect(EffectType.MovementBoost);
			}
		}
	}

	private static bool IsOwnedEffect(CustomPlayerEffects.StatusEffectBase effect, MovementBoostState state)
	{
		if (effect == null || !effect.IsEnabled || effect.Intensity != state.AppliedIntensity)
		{
			return false;
		}

		float expectedTimeLeft = Mathf.Max(0f, state.AppliedExpiresAt - Time.time);
		return Mathf.Abs(effect.TimeLeft - expectedTimeLeft) <= 0.25f;
	}
}
