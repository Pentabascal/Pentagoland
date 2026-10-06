using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp049;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using MEC;
using PlayerRoles;
using SubclassFramework.Subclasses;
using UnityEngine;

namespace SubclassFramework.Core;

public sealed class SubclassManager
{
	private static readonly AmmoType[] AllAmmo = new AmmoType[5]
	{
		AmmoType.Nato9,
		AmmoType.Nato556,
		AmmoType.Nato762,
		AmmoType.Ammo12Gauge,
		AmmoType.Ammo44Cal
	};

	private static readonly ItemType[] WatchedConsumables = new ItemType[4]
	{
		ItemType.Medkit,
		ItemType.Painkillers,
		ItemType.Adrenaline,
		ItemType.SCP500
	};

	private readonly List<Subclass> registry = new List<Subclass>();

	private readonly Dictionary<Exiled.API.Features.Player, Subclass> active = new Dictionary<Exiled.API.Features.Player, Subclass>();

	private readonly Dictionary<Exiled.API.Features.Player, float> cooldownEnd = new Dictionary<Exiled.API.Features.Player, float>();

	private readonly Dictionary<Exiled.API.Features.Player, float> assignedAt = new Dictionary<Exiled.API.Features.Player, float>();

	private readonly Dictionary<Exiled.API.Features.Player, int> assignmentStamps = new Dictionary<Exiled.API.Features.Player, int>();

	private readonly Dictionary<Exiled.API.Features.Player, int> spawnStamps = new Dictionary<Exiled.API.Features.Player, int>();

	private readonly Dictionary<RoleTypeId, float> baseMaxHealthByRole = new Dictionary<RoleTypeId, float>();

	private readonly Dictionary<string, int> roundCount = new Dictionary<string, int>();

	private readonly Dictionary<string, int> waveCount = new Dictionary<string, int>();

	private readonly Dictionary<RoleTypeId, int> waveSpawnCount = new Dictionary<RoleTypeId, int>();

	private readonly Dictionary<Exiled.API.Features.Player, string> previousNametags = new Dictionary<Exiled.API.Features.Player, string>();

	private readonly HashSet<Exiled.API.Features.Player> suppressNextAssign = new HashSet<Exiled.API.Features.Player>();

	private int nextAssignmentStamp;

	private CoroutineHandle tickHandle;

	private int lifecycleVersion;

	public static SubclassManager Instance { get; private set; }

	public Config Config => Plugin.Instance.Config;

	public IReadOnlyList<Subclass> Registry => registry;

	public void Enable()
	{
		Instance = this;
		lifecycleVersion++;
		registry.Clear();
		registry.AddRange(ChaosSubclasses.All());
		registry.AddRange(FacilitySubclasses.All());
		registry.AddRange(MtfSubclasses.All());
		if (Config.EnableExtraSubclasses)
		{
			registry.AddRange(ExtraSubclasses.All());
		}
		if (Config.EnableScpSubclasses)
		{
			registry.AddRange(ScpSubclasses.All());
		}
		if (Config.EnableScp173Module)
		{
			registry.AddRange(Scp173Module.All());
		}
		if (!Config.EnableScpSubclasses)
		{
			registry.RemoveAll((Subclass s) => s.Roles != null && s.Roles.Length != 0 && s.Roles.All((RoleTypeId r) => r.GetTeam() == Team.SCPs));
		}
		registry.RemoveAll((Subclass s) => Config.DisabledSubclasses != null && Config.DisabledSubclasses.TryGetValue(s.Id, out var value) && !value);
		foreach (IGrouping<string, Subclass> item in from s in registry
			group s by s.Id into g
			where g.Count() > 1
			select g)
		{
			Log.Warn("[SubclassFramework] ID duplicado: '" + item.Key + "'. Solo se usara la primera.");
		}
		Exiled.Events.Handlers.Player.Spawned += new CustomEventHandler<SpawnedEventArgs>(OnSpawned);
		Exiled.Events.Handlers.Player.Hurting += new CustomEventHandler<HurtingEventArgs>(OnHurting);
		Exiled.Events.Handlers.Player.Died += new CustomEventHandler<DiedEventArgs>(OnDied);
		Exiled.Events.Handlers.Player.Shooting += new CustomEventHandler<ShootingEventArgs>(OnShooting);
		Exiled.Events.Handlers.Player.InteractingDoor += new CustomEventHandler<InteractingDoorEventArgs>(OnInteractingDoor);
		Exiled.Events.Handlers.Player.ActivatingGenerator += new CustomEventHandler<ActivatingGeneratorEventArgs>(OnActivatingGenerator);
		Exiled.Events.Handlers.Player.UsedItem += new CustomEventHandler<UsedItemEventArgs>(OnUsedItem);
		Exiled.Events.Handlers.Player.Left += new CustomEventHandler<LeftEventArgs>(OnLeft);
		Scp049.StartingRecall += new CustomEventHandler<StartingRecallEventArgs>(OnStartingRecall);
		LabApi.Events.Handlers.Scp049Events.ResurrectedBody += OnResurrectedBody;
		LabApi.Events.Handlers.Scp106Events.ChangingStalkMode += Stalker.OnChangingStalkMode;
		Exiled.Events.Handlers.Server.RoundStarted += new CustomEventHandler(OnRoundStarted);
		Exiled.Events.Handlers.Server.RestartingRound += new CustomEventHandler(OnRestartingRound);
		Exiled.Events.Handlers.Server.RespawningTeam += new CustomEventHandler<RespawningTeamEventArgs>(OnRespawningTeam);
		tickHandle = Timing.RunCoroutine(TickLoop(), "SubclassFramework.Tick");
		Log.Info($"[SubclassFramework] {registry.Count} subclases registradas.");
	}

	public void Disable()
	{
		Exiled.Events.Handlers.Player.Spawned -= new CustomEventHandler<SpawnedEventArgs>(OnSpawned);
		Exiled.Events.Handlers.Player.Hurting -= new CustomEventHandler<HurtingEventArgs>(OnHurting);
		Exiled.Events.Handlers.Player.Died -= new CustomEventHandler<DiedEventArgs>(OnDied);
		Exiled.Events.Handlers.Player.Shooting -= new CustomEventHandler<ShootingEventArgs>(OnShooting);
		Exiled.Events.Handlers.Player.InteractingDoor -= new CustomEventHandler<InteractingDoorEventArgs>(OnInteractingDoor);
		Exiled.Events.Handlers.Player.ActivatingGenerator -= new CustomEventHandler<ActivatingGeneratorEventArgs>(OnActivatingGenerator);
		Exiled.Events.Handlers.Player.UsedItem -= new CustomEventHandler<UsedItemEventArgs>(OnUsedItem);
		Exiled.Events.Handlers.Player.Left -= new CustomEventHandler<LeftEventArgs>(OnLeft);
		Scp049.StartingRecall -= new CustomEventHandler<StartingRecallEventArgs>(OnStartingRecall);
		LabApi.Events.Handlers.Scp049Events.ResurrectedBody -= OnResurrectedBody;
		LabApi.Events.Handlers.Scp106Events.ChangingStalkMode -= Stalker.OnChangingStalkMode;
		Exiled.Events.Handlers.Server.RoundStarted -= new CustomEventHandler(OnRoundStarted);
		Exiled.Events.Handlers.Server.RestartingRound -= new CustomEventHandler(OnRestartingRound);
		Exiled.Events.Handlers.Server.RespawningTeam -= new CustomEventHandler<RespawningTeamEventArgs>(OnRespawningTeam);
		Timing.KillCoroutines(tickHandle);
		lifecycleVersion++;
		GlobalHooks.Clear();
		Saboteador.ClearAll();
		ZombieReviveTracker.ClearAll();
		Purista.ClearAll();
		Stalker.ClearAll();
		ZombiKamikaze.ClearAll();
		Scp173Module.ClearAll();
		SfUtils.ClearAllMessages();
		foreach (KeyValuePair<Exiled.API.Features.Player, Subclass> pair in active.ToList())
		{
			SafeCall(() =>
			{
				pair.Value.OnRemoved(pair.Key);
			}, pair.Value, "OnRemoved");
		}
		HackerMechanics.ResetRound();
		ClearAll();
		SfUtils.ClearAllMessages();
		registry.Clear();
		Instance = null;
	}

	private void ClearAll()
	{
		foreach (Exiled.API.Features.Player item in active.Keys.ToList())
		{
			ClearNametag(item);
			ResetScale(item);
		}
		active.Clear();
		cooldownEnd.Clear();
		assignedAt.Clear();
		assignmentStamps.Clear();
		spawnStamps.Clear();
		baseMaxHealthByRole.Clear();
		roundCount.Clear();
		waveCount.Clear();
		waveSpawnCount.Clear();
		previousNametags.Clear();
		suppressNextAssign.Clear();
		nextAssignmentStamp = 0;
	}

	public Subclass GetSubclass(Exiled.API.Features.Player player)
	{
		Subclass subclass;
		return player != null &&
			active.TryGetValue(player, out subclass) &&
			subclass.Roles != null &&
			subclass.Roles.Contains(player.Role.Type)
				? subclass
				: null;
	}

	public int GetAssignmentStamp(Exiled.API.Features.Player player)
	{
		int value;
		return (player != null && assignmentStamps.TryGetValue(player, out value)) ? value : 0;
	}

	public bool IsCurrentAssignment(Exiled.API.Features.Player player, Subclass subclass, int stamp)
	{
		Subclass value;
		int value2;
		return player != null && subclass != null && active.TryGetValue(player, out value) && value == subclass && assignmentStamps.TryGetValue(player, out value2) && value2 == stamp && player.IsAlive;
	}

	public bool HasSubclass(Exiled.API.Features.Player player, string id)
	{
		Subclass subclass = GetSubclass(player);
		return subclass != null && string.Equals(subclass.Id, id, StringComparison.OrdinalIgnoreCase);
	}

	public IEnumerable<Exiled.API.Features.Player> PlayersWith(string id)
	{
		return from p in active
			where string.Equals(p.Value.Id, id, StringComparison.OrdinalIgnoreCase) && p.Key != null && p.Key.IsAlive
			select p.Key;
	}

	public void SuppressNextAssign(Exiled.API.Features.Player player)
	{
		if (player != null)
		{
			suppressNextAssign.Add(player);
		}
	}

	public void CancelSuppressNextAssign(Exiled.API.Features.Player player)
	{
		if (player != null)
		{
			suppressNextAssign.Remove(player);
		}
	}

	public bool ForceAssign(Exiled.API.Features.Player player, string id)
	{
		if (player == null || !player.IsAlive)
		{
			return false;
		}
		Subclass subclass = registry.FirstOrDefault((Subclass s) => string.Equals(s.Id, id, StringComparison.OrdinalIgnoreCase));
		if (subclass == null || subclass.Roles == null || !subclass.Roles.Contains(player.Role.Type))
		{
			return false;
		}
		ResetLifeState(player);
		SfUtils.ClearMessages(player);
		RemoveSubclass(player);
		Apply(player, subclass);
		return true;
	}

	public void RemoveSubclass(Exiled.API.Features.Player player)
	{
		if (!(player == null) && active.TryGetValue(player, out var sc))
		{
			ResetLifeState(player);
			GlobalHooks.ClearPlayer(player);
			active.Remove(player);
			cooldownEnd.Remove(player);
			assignedAt.Remove(player);
			assignmentStamps.Remove(player);
			SfUtils.ClearHint(player);
			Scp173Module.ClearDecoys(player);
			ClearNametag(player);
			SfUtils.ClearMessages(player);
			ResetScale(player);
			SafeCall(() =>
			{
				sc.OnRemoved(player);
			}, sc, "OnRemoved");
		}
	}

	public float ChanceOf(Subclass sc)
	{
		if (sc == null)
		{
			return 0f;
		}
		if (sc.Roles != null && sc.Roles.Length != 0 && sc.Roles.All((RoleTypeId role) => role.GetTeam() == Team.SCPs))
		{
			return 10f;
		}
		float num = ((Config.ChanceOverrides != null && Config.ChanceOverrides.TryGetValue(sc.Id, out var value)) ? value : sc.Chance);
		return (float.IsNaN(num) || float.IsInfinity(num)) ? 0f : Mathf.Clamp(num, 0f, 100f);
	}

	public float CooldownOf(Subclass sc)
	{
		if (sc == null)
		{
			return 0f;
		}
		float num = ((Config.CooldownOverrides != null && Config.CooldownOverrides.TryGetValue(sc.Id, out var value)) ? value : sc.Cooldown);
		return (float.IsNaN(num) || float.IsInfinity(num)) ? 0f : Mathf.Clamp(num, 0f, 3600f);
	}

	public int HealthOf(Subclass sc)
	{
		int num = ((Config.HealthOverrides != null && Config.HealthOverrides.TryGetValue(sc.Id, out var value)) ? Mathf.Max(1, value) : sc.MaxHealth);
		if (sc.Id == "zombi_infeccioso")
		{
			return Mathf.Min(num > 0 ? num : 280, 280);
		}
		if (sc.Roles != null && sc.Roles.Contains(RoleTypeId.Scp0492) && num > 0)
		{
			int minimum = sc.Id == "zombi_kamikaze" ? 280 : 300;
			return Mathf.Clamp(num, minimum, 600);
		}
		return num;
	}

	public float RemainingCooldown(Exiled.API.Features.Player player)
	{
		if (player == null || !cooldownEnd.TryGetValue(player, out var value))
		{
			return 0f;
		}
		return Mathf.Max(0f, value - Time.time);
	}

	public void StartCooldown(Exiled.API.Features.Player player, float seconds)
	{
		if (!(player == null))
		{
			cooldownEnd[player] = Time.time + seconds;
		}
	}

	public bool TryUseAbility(Exiled.API.Features.Player player)
	{
		if (player == null || !player.IsAlive)
		{
			return false;
		}
		Subclass sc = GetSubclass(player);
		if (sc == null)
		{
			AbilityErrorFormatter.Show(player, "No tienes ninguna subclase con habilidad activa.", 3f);
			return false;
		}
		if (!sc.HasActiveAbility)
		{
			AbilityErrorFormatter.Show(player, "<b>" + sc.Name + "</b> no tiene ninguna habilidad activa.", 4f);
			return false;
		}
		float num = RemainingCooldown(player);
		if (num > 0.05f)
		{
			AbilityErrorFormatter.Show(player, $"Habilidad en enfriamiento: <b>{Mathf.CeilToInt(num)}s</b>", 2f);
			return false;
		}
		bool used = false;
		SafeCall(() =>
		{
			used = sc.UseAbility(player);
		}, sc, "UseAbility");
		if (used)
		{
			StartCooldown(player, CooldownOf(sc));
		}
		else
		{
			AbilityErrorFormatter.Show(player, sc.FailMessage, 3f);
		}
		return used;
	}

	private void OnSpawned(SpawnedEventArgs ev)
	{
		Exiled.API.Features.Player player = ev?.Player;
		if (player == null || player is Npc)
		{
			return;
		}
		ResetLifeState(player);
		GlobalHooks.ClearPlayer(player);
		SfUtils.ClearMessages(player);
		RemoveSubclass(player);
		RoleTypeId role = player.Role.Type;
		int spawnStamp = ((!spawnStamps.TryGetValue(player, out var value)) ? 1 : (value + 1));
		spawnStamps[player] = spawnStamp;
		if (suppressNextAssign.Remove(player) || role == RoleTypeId.Spectator || role == RoleTypeId.None || role == RoleTypeId.Tutorial || role == RoleTypeId.Overwatch)
		{
			return;
		}
		waveSpawnCount[role] = ((!waveSpawnCount.TryGetValue(role, out var value2)) ? 1 : (value2 + 1));
		int assignmentLifecycle = lifecycleVersion;
		Timing.CallDelayed(0.6f, () =>
		{
			if (lifecycleVersion == assignmentLifecycle && !(player == null) && player.IsAlive && player.Role.Type == role && spawnStamps.TryGetValue(player, out var value3) && value3 == spawnStamp)
			{
				if (!active.ContainsKey(player))
				{
					ResetScale(player);
				}
				if (player.MaxHealth > 0f && !baseMaxHealthByRole.ContainsKey(role))
				{
					baseMaxHealthByRole[role] = player.MaxHealth;
				}
				SfUtils.ClearMessages(player);
				AssignFor(player, role);
			}
		});
	}

	private void AssignFor(Exiled.API.Features.Player player, RoleTypeId expectedRole)
	{
		if (!(player == null) && player.IsAlive && player.Role.Type == expectedRole && !active.ContainsKey(player))
		{
			List<Subclass> pool = GetPool(expectedRole, Tier.Specific);
			float num = PoolSuccessChance(expectedRole, pool.Count);
			Subclass subclass = Draw(pool, num);
			if (subclass == null)
			{
				List<Subclass> pool2 = GetPool(expectedRole, Tier.General);
				float num2 = WaveSubclassChance(expectedRole);
				float b = ((num >= num2 - 0.001f) ? 0f : Mathf.Clamp((num2 - num) / (1f - num / 100f), 0f, num2));
				subclass = Draw(pool2, Mathf.Min(PoolSuccessChance(expectedRole, pool2.Count), b));
			}
			if (subclass != null)
			{
				Apply(player, subclass);
			}
		}
	}

	private List<Subclass> GetPool(RoleTypeId role, Tier tier)
	{
		return registry.Where((Subclass s) => s.AssignTier == tier && s.Roles != null && s.Roles.Contains(role) && IsAvailable(s) && ChanceOf(s) > 0f).ToList();
	}

	private Subclass Draw(List<Subclass> pool, float successChance)
	{
		if (pool == null || pool.Count == 0 || successChance <= 0f || !SfUtils.Roll(successChance))
		{
			return null;
		}
		float num = pool.Sum((Subclass sc) => Mathf.Max(0.1f, ChanceOf(sc)));
		float num2 = (float)SfUtils.RandomInt(0, 100000) / 100000f * num;
		foreach (Subclass item in pool)
		{
			num2 -= Mathf.Max(0.1f, ChanceOf(item));
			if (num2 <= 0f)
			{
				return item;
			}
		}
		return pool[pool.Count - 1];
	}

	private float PoolSuccessChance(RoleTypeId role, int poolSize)
	{
		if (poolSize <= 0)
		{
			return 0f;
		}
		bool flag = role.GetTeam() == Team.SCPs;
		float num = (flag ? Config.ScpBaseSubclassChance : Config.BaseSubclassChance);
		float num2 = (flag ? Config.ScpTwoOptionSubclassChance : Config.TwoOptionSubclassChance);
		float num3 = WaveSubclassChance(role);
		if (poolSize == 1)
		{
			return Mathf.Min(num, num3);
		}
		return Mathf.Clamp(num2 + (float)(poolSize - 2) * Config.SubclassChancePerOption, num, num3);
	}

	private float WaveSubclassChance(RoleTypeId role)
	{
		int num = (waveSpawnCount.TryGetValue(role, out var value) ? value : 0);
		float num2 = Mathf.Clamp((float)(num - 4) * 0.75f, 0f, 4f);
		bool flag = role.GetTeam() == Team.SCPs;
		float min = (flag ? Config.ScpBaseSubclassChance : Config.BaseSubclassChance);
		float num3 = (flag ? Config.ScpMaxSubclassChance : Config.MaxSubclassChance);
		return Mathf.Clamp(num3 + num2, min, 99f);
	}

	private bool IsAvailable(Subclass sc)
	{
		if (sc.MinRoundTime > 0f && Round.ElapsedTime.TotalSeconds < (double)sc.MinRoundTime)
		{
			return false;
		}
		if (sc.MaxPerRound >= 0 && roundCount.TryGetValue(sc.Id, out var value) && value >= sc.MaxPerRound)
		{
			return false;
		}
		if (sc.MaxPerWave >= 0 && waveCount.TryGetValue(sc.Id, out var value2) && value2 >= sc.MaxPerWave)
		{
			return false;
		}
		return true;
	}

	private void Apply(Exiled.API.Features.Player player, Subclass sc)
	{
		ResetLifeState(player);
		active[player] = sc;
		assignedAt[player] = Time.time;
		assignmentStamps[player] = ++nextAssignmentStamp;
		cooldownEnd.Remove(player);
		float initialCooldown = CooldownOf(sc) * 0.5f;
		roundCount[sc.Id] = ((!roundCount.TryGetValue(sc.Id, out var value)) ? 1 : (value + 1));
		waveCount[sc.Id] = ((!waveCount.TryGetValue(sc.Id, out var value2)) ? 1 : (value2 + 1));
		SafeCall(() =>
		{
			ApplyLoadout(player, sc);
		}, sc, "ApplyLoadout");
		SafeCall(() =>
		{
			sc.OnAssigned(player);
		}, sc, "OnAssigned");
		if (sc.HasActiveAbility)
		{
			StartCooldown(player, initialCooldown);
		}
		if (Config.ShowSubclassOnSpawn && SfUtils.ShowSubclassIntro(player))
		{
			string text = (sc.HasActiveAbility ? $"\n<size=18><color=#8fd3ff><b>USO:</b> pulsa tu tecla o escribe <b>.hab</b>  |  <b>CD INICIAL:</b> {Mathf.RoundToInt(initialCooldown)}s</color></size>" : string.Empty);
			string message = "<size=26><color=" + BannerColor(sc) + "><b>" + sc.Name + "</b></color></size>\n<size=18><color=#d6dee8><b>QUE HACE:</b> " + sc.Description + "</color></size>" + text;
			string plainText = sc.Name + " Que hace " + sc.Description + " " + (sc.HasActiveAbility ? "Usa tu habilidad con la tecla o hab" : string.Empty);
			SfUtils.BroadcastTop(player, message, SfUtils.RecommendedTextDuration(plainText, Config.SpawnHintDuration));
			if (sc.HasActiveAbility)
			{
				DrawHud(player, sc);
			}
		}
		UpdateNametag(player, sc);
		if (Config.Debug)
		{
			Log.Debug($"[SubclassFramework] {player.Nickname} -> {sc}");
		}
	}

	private static string BannerColor(Subclass sc)
	{
		if (sc?.Roles == null || sc.Roles.Length == 0)
		{
			return "#ffd166";
		}
		if (sc.Roles.Any((RoleTypeId role) => role.GetTeam() == Team.SCPs))
		{
			return "#ff6b8a";
		}
		if (sc.Roles.Any((RoleTypeId role) => role.GetTeam() == Team.ChaosInsurgency))
		{
			return "#5ee37a";
		}
		if (sc.Roles.Any((RoleTypeId role) => role.GetTeam() == Team.FoundationForces))
		{
			return "#63b3ff";
		}
		if (sc.Roles.Any((RoleTypeId role) => role == RoleTypeId.ClassD))
		{
			return "#ff9f43";
		}
		if (sc.Roles.Any((RoleTypeId role) => role == RoleTypeId.Scientist))
		{
			return "#ffe66d";
		}
		return "#d6dee8";
	}

	private void ApplyLoadout(Exiled.API.Features.Player player, Subclass sc)
	{
		int num = HealthOf(sc);
		if (num > 0)
		{
			SfUtils.SetMaxHealth(player, num, healToFull: true);
		}
		if (sc.OverrideInventory)
		{
			player.ClearInventory();
			AmmoType[] allAmmo = AllAmmo;
			foreach (AmmoType ammoType in allAmmo)
			{
				player.SetAmmo(ammoType, 0);
			}
			ItemType[] array = sc.Items ?? Array.Empty<ItemType>();
			foreach (ItemType itemType in array)
			{
				player.AddItem(itemType);
			}
		}
		ItemType[] array2 = sc.ExtraItems ?? Array.Empty<ItemType>();
		foreach (ItemType itemType2 in array2)
		{
			player.AddItem(itemType2);
		}
		KeyValuePair<AmmoType, ushort>[] array3 = sc.Ammo ?? Array.Empty<KeyValuePair<AmmoType, ushort>>();
		for (int l = 0; l < array3.Length; l++)
		{
			KeyValuePair<AmmoType, ushort> keyValuePair = array3[l];
			player.SetAmmo(keyValuePair.Key, keyValuePair.Value);
		}
		if (!Mathf.Approximately(sc.Scale, 1f))
		{
			try
			{
				player.Scale = Vector3.one * sc.Scale;
			}
			catch (Exception ex)
			{
				Log.Debug("[SubclassFramework] No se pudo cambiar la escala: " + ex.Message);
			}
		}
	}

	private void ResetLifeState(Exiled.API.Features.Player player)
	{
		if (!(player == null))
		{
			Cirujano.ClearPlayer(player);
			RoleTypeId type = player.Role.Type;
			if (baseMaxHealthByRole.TryGetValue(type, out var value) && value > 0f)
			{
				player.MaxHealth = value;
				player.Health = Mathf.Min(player.Health, value);
			}
			ResetScale(player);
			EffectType[] array = new EffectType[14]
			{
				EffectType.MovementBoost,
				EffectType.Slowness,
				EffectType.Burned,
				EffectType.Ensnared,
				EffectType.Invisible,
				EffectType.Scp1344,
				EffectType.Scp1853,
				EffectType.Blurred,
				EffectType.Concussed,
				EffectType.Scp207,
				EffectType.Invigorated,
				EffectType.Deafened,
				EffectType.Disabled,
				EffectType.Stained
			};
			EffectType[] array2 = array;
			foreach (EffectType effect in array2)
			{
				player.DisableEffect(effect);
			}
		}
	}

	private static void ResetScale(Exiled.API.Features.Player player)
	{
		if (player == null)
		{
			return;
		}
		try
		{
			player.Scale = Vector3.one;
		}
		catch (Exception ex)
		{
			Log.Debug("[SubclassFramework] No se pudo restaurar la escala de " + player.Nickname + ": " + ex.Message);
		}
	}

	private void UpdateNametag(Exiled.API.Features.Player player, Subclass sc)
	{
		if (player == null || sc == null || sc.Roles == null || sc.Roles.Length == 0 || !sc.Roles.All((RoleTypeId r) => r.GetTeam() == Team.SCPs))
		{
			return;
		}
		try
		{
			if (!previousNametags.ContainsKey(player))
			{
				previousNametags[player] = player.CustomInfo;
			}
			player.CustomInfo = sc.Name;
		}
		catch (Exception ex)
		{
			Log.Debug("[SubclassFramework] Nametag: " + ex.Message);
		}
	}

	private void ClearNametag(Exiled.API.Features.Player player)
	{
		if (player == null)
		{
			return;
		}
		try
		{
			if (previousNametags.TryGetValue(player, out var value))
			{
				player.CustomInfo = value;
				previousNametags.Remove(player);
			}
		}
		catch (Exception ex)
		{
			Log.Debug("[SubclassFramework] Nametag (limpiar): " + ex.Message);
		}
	}

	private IEnumerator<float> TickLoop()
	{
		while (true)
		{
			yield return Timing.WaitForSeconds(1f);
			GlobalHooks.Tick();
			List<KeyValuePair<Exiled.API.Features.Player, Subclass>> snapshot = active.ToList();
			foreach (KeyValuePair<Exiled.API.Features.Player, Subclass> pair in snapshot)
			{
				Exiled.API.Features.Player player = pair.Key;
				Subclass sc = pair.Value;
				if (player == null || !player.IsConnected)
				{
					RemoveSubclass(player);
				}
				else if (sc?.Roles == null || !sc.Roles.Contains(player.Role.Type))
				{
					RemoveSubclass(player);
				}
				else
				{
					if (!player.IsAlive)
					{
						continue;
					}
					SafeCall(() =>
					{
						NormalizeZombieHealth(player, sc);
						if (!Mathf.Approximately(sc.SpeedMultiplier, 1f))
						{
							float num = (assignedAt.TryGetValue(player, out var value) ? (Time.time - value) : 999f);
							if (num >= sc.SpeedDelay)
							{
								SfUtils.ApplySpeedMultiplier(player, sc.SpeedMultiplier);
							}
						}
						sc.OnTick(player);
						DrawHud(player, sc);
					}, sc, "OnTick");
				}
			}
		}
	}

	private static void NormalizeZombieHealth(Exiled.API.Features.Player player, Subclass sc)
	{
		if (!(player == null) && sc?.Roles != null && sc.Roles.Contains(RoleTypeId.Scp0492))
		{
			if (sc.Id == "zombi_infeccioso")
			{
				float infectedMaxHealth = Mathf.Min(player.MaxHealth, 280f);
				if (!Mathf.Approximately(player.MaxHealth, infectedMaxHealth))
				{
					player.MaxHealth = infectedMaxHealth;
					player.Health = Mathf.Min(player.Health, infectedMaxHealth);
				}
				return;
			}
			float minimum = sc.Id == "zombi_kamikaze" || sc.Id == "zombi_infeccioso" ? 280f : 300f;
			float num = Mathf.Clamp(player.MaxHealth, minimum, 600f);
			if (!Mathf.Approximately(player.MaxHealth, num))
			{
				player.MaxHealth = num;
				player.Health = Mathf.Min(player.Health, num);
			}
		}
	}

	private void DrawHud(Exiled.API.Features.Player player, Subclass sc)
	{
		if (Config.ShowAbilityHud && SfUtils.ShowAbilityHud(player) && (!player.SessionVariables.TryGetValue("ShowAbilityHud", out var value) || !(value is bool) || (bool)value) && sc.HasActiveAbility && !SfUtils.HasActiveHint(player))
		{
			float num = RemainingCooldown(player);
			float num2 = CooldownOf(sc);
			float num3 = ((num2 <= 0f) ? 1f : Mathf.Clamp01(1f - num / num2));
			int num4 = Mathf.Clamp(Mathf.RoundToInt(num3 * 10f), 0, 10);
			string text = new string('|', num4) + new string('.', 10 - num4);
			string text2 = ((num > 0.05f) ? "#d88" : "#8d8");
			string text3 = ((num > 0.05f) ? $"{Mathf.CeilToInt(num)}s" : "LISTA");
			float num5 = -0.35f - (float)Plugin.Instance.Config.AbilityHudOffset * 0.55f;
			string text4 = "<voffset=" + num5.ToString(CultureInfo.InvariantCulture) + "em><color=" + text2 + "><b>" + sc.Name + "</b> | " + text3 + "\n<size=16>[" + text + "]</size></color></voffset>";
			SfUtils.HudHint(player, "<size=16>" + text4 + "</size>", 1.15f);
		}
	}

	private void OnHurting(HurtingEventArgs ev)
	{
		if (ev?.Player == null || !ev.IsAllowed)
		{
			return;
		}
		GlobalHooks.OnHurting(ev);
		if (!ev.IsAllowed)
		{
			return;
		}
		Subclass victimSc = GetSubclass(ev.Player);
		if (victimSc != null)
		{
			SafeCall(() =>
			{
				victimSc.OnTakingDamage(ev.Player, ev);
			}, victimSc, "OnTakingDamage");
		}
		if (!ev.IsAllowed)
		{
			return;
		}
		if (ev.Attacker == null || ev.Attacker == ev.Player)
		{
			return;
		}
		Subclass attackerSc = GetSubclass(ev.Attacker);
		if (attackerSc != null)
		{
			SafeCall(() =>
			{
				attackerSc.OnDealingDamage(ev.Attacker, ev);
			}, attackerSc, "OnDealingDamage");
		}
	}

	private void OnDied(DiedEventArgs ev)
	{
		if (ev?.Player == null)
		{
			return;
		}
		ZombieReviveTracker.Clear(ev.Player);
		GlobalHooks.ClearPlayer(ev.Player);
		ResetLifeState(ev.Player);
		Subclass victimSc = GetSubclass(ev.Player);
		if (victimSc != null)
		{
			SafeCall(() =>
			{
				victimSc.OnDeath(ev.Player, ev);
			}, victimSc, "OnDeath");
		}
		if (ev.Attacker != null && ev.Attacker != ev.Player)
		{
			Subclass killerSc = GetSubclass(ev.Attacker);
			if (killerSc != null)
			{
				SafeCall(() =>
				{
					killerSc.OnKill(ev.Attacker, ev.Player, ev);
				}, killerSc, "OnKill");
			}
			if (Config.Scp096HealPerKill > 0f && ev.Attacker.Role.Type == RoleTypeId.Scp096 && ev.Attacker.IsAlive)
			{
				float scp096HealPerKill = Config.Scp096HealPerKill;
				Exiled.API.Features.Player attacker = ev.Attacker;
				attacker.Health = Mathf.Min(attacker.MaxHealth, attacker.Health + scp096HealPerKill);
				SfUtils.Hint(attacker, $"<color=#f88>+{Mathf.RoundToInt(scp096HealPerKill)} de vida</color>", 2.5f);
			}
		}
		RemoveSubclass(ev.Player);
	}

	private void OnShooting(ShootingEventArgs ev)
	{
		Subclass sc = GetSubclass(ev?.Player);
		if (sc != null)
		{
			SafeCall(() =>
			{
				sc.OnShooting(ev.Player, ev);
			}, sc, "OnShooting");
		}
	}

	private void OnInteractingDoor(InteractingDoorEventArgs ev)
	{
		Subclass sc = GetSubclass(ev?.Player);
		if (sc != null)
		{
			SafeCall(() =>
			{
				sc.OnInteractingDoor(ev.Player, ev);
			}, sc, "OnInteractingDoor");
		}
	}

	private void OnActivatingGenerator(ActivatingGeneratorEventArgs ev)
	{
		if (ev?.Player == null)
		{
			return;
		}
		Subclass sc = GetSubclass(ev.Player);
		if (sc != null)
		{
			SafeCall(() =>
			{
				sc.OnActivatingGenerator(ev.Player, ev);
			}, sc, "OnActivatingGenerator");
		}
		try
		{
			Saboteador.PunishActivator(ev.Player, ev.Generator);
		}
		catch (Exception ex)
		{
			Log.Debug("[SubclassFramework] Saboteador: " + ex.Message);
		}
	}

	private void OnUsedItem(UsedItemEventArgs ev)
	{
		if (ev?.Player == null || ev.Item == null || ev.CausedByHolstering)
		{
			return;
		}
		Subclass sc = GetSubclass(ev.Player);
		if (sc != null && WatchedConsumables.Contains(ev.Item.Type))
		{
			SafeCall(() =>
			{
				sc.OnItemConsumed(ev.Player, ev.Item);
			}, sc, "OnItemConsumed");
		}
	}

	private void OnStartingRecall(StartingRecallEventArgs ev)
	{
		if (ev?.Player == null || ev.Ragdoll == null)
		{
			return;
		}
		if (GetSubclass(ev.Player) is Purista && Purista.IsBlocked(ev.Ragdoll))
		{
			ev.IsAllowed = false;
			try
			{
				ev.Ragdoll.Destroy();
			}
			catch (Exception ex)
			{
				Log.Debug("[SubclassFramework] Purista: no se pudo limpiar el cadaver bloqueado: " + ex.Message);
			}
		}
	}

	private void OnResurrectedBody(LabApi.Events.Arguments.Scp049Events.Scp049ResurrectedBodyEventArgs ev)
	{
		if (ev?.Player?.ReferenceHub == null || ev.Target?.ReferenceHub?.roleManager.CurrentRole.RoleTypeId != RoleTypeId.Scp0492)
		{
			return;
		}

		Exiled.API.Features.Player doctor = Exiled.API.Features.Player.Get(ev.Player.ReferenceHub);
		if (doctor != null && doctor.IsAlive && doctor.Role.Type == RoleTypeId.Scp049)
		{
			ZombieReviveTracker.Notify(doctor);
			if (GetSubclass(doctor) is Plaga)
			{
				Plaga.OnRevived(doctor);
			}
		}
	}

	private void OnLeft(LeftEventArgs ev)
	{
		GlobalHooks.ClearPlayer(ev?.Player);
		ZombieReviveTracker.Clear(ev?.Player);
		Stalker.RemoveLock(ev?.Player);
		SfUtils.ClearMessages(ev?.Player);
		spawnStamps.Remove(ev?.Player);
		RemoveSubclass(ev?.Player);
	}

	private void OnRoundStarted()
	{
		lifecycleVersion++;
		HackerMechanics.ResetRound();
		roundCount.Clear();
		waveCount.Clear();
		waveSpawnCount.Clear();
		ZombieReviveTracker.ClearAll();
		ZombiKamikaze.ClearAll();
		Stalker.ClearAll();
	}

	private void OnRestartingRound()
	{
		lifecycleVersion++;
		GlobalHooks.Clear();
		HackerMechanics.ResetRound();
		Saboteador.ClearAll();
		ZombieReviveTracker.ClearAll();
		ZombiKamikaze.ClearAll();
		Stalker.ClearAll();
		Scp173Module.ClearAll();
		foreach (KeyValuePair<Exiled.API.Features.Player, Subclass> pair in active.ToList())
		{
			SafeCall(() =>
			{
				pair.Value.OnRemoved(pair.Key);
			}, pair.Value, "OnRemoved");
		}
		ClearAll();
	}

	private void OnRespawningTeam(RespawningTeamEventArgs ev)
	{
		waveCount.Clear();
		waveSpawnCount.Clear();
	}

	private static void SafeCall(Action action, Subclass sc, string where)
	{
		try
		{
			action();
		}
		catch (Exception arg)
		{
			Log.Error(string.Format("[SubclassFramework] Error en {0}.{1}: {2}", sc?.Id ?? "?", where, arg));
		}
	}
}
