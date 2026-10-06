using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using MEC;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public static class Scp173Module
{
	private sealed class Decoy
	{
		public Npc Bot;

		public Exiled.API.Features.Player Owner;

		public float Expiry;

		public float LastWarning;

		public int LastAttackerId = -1;
	}

	public static readonly RoleTypeId[] Scp173 = new RoleTypeId[1] { RoleTypeId.Scp173 };

	private static readonly List<Decoy> Decoys = new List<Decoy>();

	private static readonly Dictionary<Exiled.API.Features.Player, float> LastTantrumWarning = new Dictionary<Exiled.API.Features.Player, float>();

	private static bool registered;

	public static IEnumerable<Subclass> All()
	{
		return new Subclass[3]
		{
			new Scp173señuelo(),
			new Scp173Fisura(),
			new Scp173Guardian()
		};
	}

	public static void Register()
	{
		if (!registered)
		{
			Exiled.Events.Handlers.Player.ReceivingEffect += new CustomEventHandler<ReceivingEffectEventArgs>(OnReceivingEffect);
			Exiled.Events.Handlers.Player.Hurting += new CustomEventHandler<HurtingEventArgs>(OnHurting);
			Exiled.Events.Handlers.Player.Died += new CustomEventHandler<DiedEventArgs>(OnDied);
			Exiled.Events.Handlers.Player.Left += new CustomEventHandler<LeftEventArgs>(OnLeft);
			registered = true;
		}
	}

	public static void Unregister()
	{
		if (registered)
		{
			Exiled.Events.Handlers.Player.ReceivingEffect -= new CustomEventHandler<ReceivingEffectEventArgs>(OnReceivingEffect);
			Exiled.Events.Handlers.Player.Hurting -= new CustomEventHandler<HurtingEventArgs>(OnHurting);
			Exiled.Events.Handlers.Player.Died -= new CustomEventHandler<DiedEventArgs>(OnDied);
			Exiled.Events.Handlers.Player.Left -= new CustomEventHandler<LeftEventArgs>(OnLeft);
			ClearAll();
			registered = false;
		}
	}

	public static void ClearAll()
	{
		foreach (Decoy item in Decoys.ToList())
		{
			Kill(item);
		}
		Decoys.Clear();
		LastTantrumWarning.Clear();
	}

	public static void ClearDecoys(Exiled.API.Features.Player owner)
	{
		if (owner == null)
		{
			return;
		}
		foreach (Decoy item in Decoys.Where((Decoy d) => d.Owner == owner).ToList())
		{
			Kill(item);
		}
	}

	public static bool SpawnDecoy(Exiled.API.Features.Player owner, float health, float duration)
	{
		if (owner == null || !owner.IsAlive)
		{
			return false;
		}
		if (!Plugin.Instance.Config.EnableScp173Decoy)
		{
			SfUtils.Hint(owner, "<color=#f88>El señuelo está desactivado en este servidor.</color>", 4f);
			return false;
		}
		try
		{
			Npc npc = Npc.Spawn(owner.Nickname, RoleTypeId.Scp173);
			if (npc == null)
			{
				return false;
			}
			npc.Position = owner.Position;
			npc.MaxHealth = health;
			npc.Health = health;
			npc.ArtificialHealth = 0f;
			npc.CustomInfo = string.Empty;
			Decoy decoy = new Decoy
			{
				Bot = npc,
				Owner = owner,
				Expiry = Time.time + duration
			};
			Decoys.Add(decoy);
			Timing.CallDelayed(0.1f, () =>
			{
				TeleportDecoy(decoy);
			});
			Timing.CallDelayed(0.4f, () =>
			{
				TeleportDecoy(decoy);
			});
			Timing.CallDelayed(0.8f, () =>
			{
				TeleportDecoy(decoy);
			});
			Timing.CallDelayed(duration, () =>
			{
				Kill(decoy);
			});
			SfUtils.Hint(owner, $"<color=#dd8><b>Señuelo colocado.</b> Dura {Mathf.RoundToInt(duration)} segundos y tiene {Mathf.RoundToInt(health)} HP.</color>", 6f);
			return true;
		}
		catch (Exception arg)
		{
			Log.Error($"[SubclassFramework] No se pudo crear el señuelo de SCP-173: {arg}");
			SfUtils.Hint(owner, "<color=#f88>El señuelo ha fallado. Avisa al staff.</color>");
			return false;
		}
	}

	private static void TeleportDecoy(Decoy decoy)
	{
		if (decoy == null || decoy.Bot == null || decoy.Owner == null || !decoy.Owner.IsAlive || !Decoys.Contains(decoy))
		{
			return;
		}
		try
		{
			decoy.Bot.Position = decoy.Owner.Position;
		}
		catch (Exception ex)
		{
			Log.Debug("[SubclassFramework] No se pudo reposicionar el se: " + ex.Message);
		}
	}

	private static void Kill(Decoy decoy)
	{
		if (decoy == null)
		{
			return;
		}
		Decoys.Remove(decoy);
		try
		{
			decoy.Bot?.Destroy();
		}
		catch (Exception ex)
		{
			Log.Debug("[SubclassFramework] Se: " + ex.Message);
		}
	}

	private static Decoy Find(Exiled.API.Features.Player possibleBot)
	{
		return (possibleBot == null) ? null : Decoys.FirstOrDefault((Decoy d) => d.Bot != null && d.Bot.Id == possibleBot.Id);
	}

	private static void OnHurting(HurtingEventArgs ev)
	{
		Decoy decoy = Find(ev?.Player);
		if (decoy != null && !(decoy.Bot == null) && ev.IsAllowed)
		{
			if (ev.Attacker != null && decoy.Owner != null && decoy.Owner.IsConnected && (Time.time - decoy.LastWarning >= 5f || decoy.LastAttackerId != ev.Attacker.Id))
			{
				decoy.LastWarning = Time.time;
				decoy.LastAttackerId = ev.Attacker.Id;
				string hexColor = SfUtils.ColorFor(ev.Attacker);
				string faction = ev.Attacker.Role.Team switch
				{
					Team.FoundationForces => "NTF",
					Team.ChaosInsurgency => "Insurgencia del Caos",
					Team.ClassD => "Clase-D",
					_ => ev.Attacker.Role.Type.ToString()
				};
				string text = "Tu señuelo fue disparado por " + ev.Attacker.Nickname + " (" + faction + ") en " + SfUtils.RoomName(decoy.Bot) + ".";
				SfUtils.Hint(decoy.Owner, "<size=22>" + SfUtils.Colored(text, hexColor) + "</size>", 6f);
			}
			decoy.Bot.ArtificialHealth = 0f;
			if (ev.Amount >= decoy.Bot.Health)
			{
				ev.IsAllowed = false;
				Kill(decoy);
			}
		}
	}

	private static void OnReceivingEffect(ReceivingEffectEventArgs ev)
	{
		if (ev?.Player == null || ev.Effect == null || !ev.Effect.TryGetEffectType(out var effect) || effect != EffectType.Stained)
		{
			return;
		}
		Exiled.API.Features.Player player = ev.Player;
		foreach (Exiled.API.Features.Player item in Exiled.API.Features.Player.List.Where((Exiled.API.Features.Player p) => p != null && !(p is Npc) && p.IsAlive && p.Role.Type == RoleTypeId.Scp173))
		{
			if (!LastTantrumWarning.TryGetValue(item, out var value) || !(Time.time - value < 30f))
			{
				LastTantrumWarning[item] = Time.time;
				string hexColor = SfUtils.ColorFor(player);
				string text = "Un jugador ha pisado un Tantrum en " + SfUtils.RoomName(player);
				SfUtils.Hint(item, "<size=22>" + SfUtils.Colored(text, hexColor) + "</size>");
			}
		}
	}

	private static void OnDied(DiedEventArgs ev)
	{
		if (!(ev?.Player == null))
		{
			ClearDecoys(ev.Player);
			LastTantrumWarning.Remove(ev.Player);
		}
	}

	private static void OnLeft(LeftEventArgs ev)
	{
		if (!(ev?.Player == null))
		{
			ClearDecoys(ev.Player);
			LastTantrumWarning.Remove(ev.Player);
		}
	}
}
