using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using PlayerRoles;
using UnityEngine;

namespace SubclassFramework.Core;

public static class SfUtils
{
	private sealed class ActiveHint
	{
		public string Message;

		public float Expiry;
	}

	private static readonly System.Random Rng = new System.Random();

	private static readonly Dictionary<Player, ActiveHint> ActiveHints = new Dictionary<Player, ActiveHint>();

	private static readonly HashSet<string> HiddenSubclassIntro = new HashSet<string>(StringComparer.Ordinal);

	private static readonly HashSet<string> HiddenAbilityHud = new HashSet<string>(StringComparer.Ordinal);

	public static bool Roll(float percent)
	{
		return Rng.NextDouble() * 100.0 < (double)percent;
	}

	public static bool ShowSubclassIntro(Player player)
	{
		return player == null || string.IsNullOrEmpty(player.UserId) || !HiddenSubclassIntro.Contains(player.UserId);
	}

	public static bool ShowAbilityHud(Player player)
	{
		return player == null || string.IsNullOrEmpty(player.UserId) || !HiddenAbilityHud.Contains(player.UserId);
	}

	public static bool ToggleSubclassIntro(Player player)
	{
		return TogglePreference(HiddenSubclassIntro, player);
	}

	public static bool ToggleAbilityHud(Player player)
	{
		return TogglePreference(HiddenAbilityHud, player);
	}

	private static bool TogglePreference(HashSet<string> preference, Player player)
	{
		if (player == null || string.IsNullOrEmpty(player.UserId))
		{
			return false;
		}
		if (!preference.Add(player.UserId))
		{
			preference.Remove(player.UserId);
			return true;
		}
		return false;
	}

	public static float TargetOverallChance(int poolSize)
	{
		if (poolSize <= 1)
		{
			return 55f;
		}
		float value = 60f + 6.6666665f * (float)(poolSize - 2);
		return Mathf.Clamp(value, 40f, 90f);
	}

	public static float PoolChance(int poolSize)
	{
		if (poolSize <= 0)
		{
			return 0f;
		}
		float num = TargetOverallChance(poolSize) / 100f;
		double num2 = 1.0 - Math.Pow(1.0 - (double)num, 1.0 / (double)poolSize);
		return Mathf.Clamp((float)(num2 * 100.0), 5f, 95f);
	}

	public static int RandomInt(int minInclusive, int maxExclusive)
	{
		return Rng.Next(minInclusive, maxExclusive);
	}

	public static T Pick<T>(IList<T> list)
	{
		return (list == null || list.Count == 0) ? default(T) : list[Rng.Next(list.Count)];
	}

	public static byte ToIntensity(float value)
	{
		return (byte)Mathf.Clamp(Mathf.RoundToInt(value), 1, 255);
	}

	public static void AddAhp(Player player, float amount, float limit = 200f, float decay = 1.2f, float sustain = 0f, bool persistent = false)
	{
		if (player == null || amount <= 0f)
		{
			return;
		}
		try
		{
			player.AddAhp(amount, limit, decay, 0.7f, sustain, persistent);
		}
		catch (Exception ex)
		{
			Log.Debug("[SubclassFramework] AddAhp fallo: " + ex.Message);
		}
	}

	public static void AddPermanentAhp(Player player, float amount, float limit = 1000f)
	{
		AddAhp(player, amount, limit, 0f, 0f, persistent: true);
	}

	public static void Heal(Player player, float amount)
	{
		if (!(player == null) && player.IsAlive)
		{
			player.Health = Mathf.Min(player.MaxHealth, player.Health + amount);
		}
	}

	public static void Damage(Player player, float amount, string reason = "Subclase")
	{
		if (!(player == null) && player.IsAlive)
		{
			player.Hurt(amount, reason);
		}
	}

	public static void SetMaxHealth(Player player, float value, bool healToFull)
	{
		if (!(player == null))
		{
			player.MaxHealth = value;
			if (healToFull || player.Health > value)
			{
				player.Health = value;
			}
		}
	}

	public static void RefillStamina(Player player)
	{
		if (player == null)
		{
			return;
		}
		try
		{
			player.ResetStamina();
		}
		catch (Exception ex)
		{
			Log.Debug("[SubclassFramework] RefillStamina fallo: " + ex.Message);
		}
	}

	public static void ApplySpeedMultiplier(Player player, float multiplier)
	{
		if (!(player == null) && player.IsAlive && !Mathf.Approximately(multiplier, 1f))
		{
			float value = Mathf.Abs((multiplier - 1f) * 100f);
			byte intensity = ToIntensity(value);
			if (multiplier > 1f)
			{
				player.EnableEffect(EffectType.MovementBoost, intensity, 2f);
			}
			else
			{
				player.EnableEffect(EffectType.Slowness, intensity, 2f);
			}
		}
	}

	public static void SpeedBoost(Player player, float percent, float duration)
	{
		if (!(player == null) && !(percent <= 0f))
		{
			player.EnableEffect(EffectType.MovementBoost, ToIntensity(percent), duration);
		}
	}

	public static void Slow(Player player, float percent, float duration)
	{
		if (!(player == null) && !(percent <= 0f))
		{
			player.EnableEffect(EffectType.Slowness, ToIntensity(percent), duration);
		}
	}

	public static bool IsInDarkness(Player player)
	{
		try
		{
			if (player == null)
			{
				return false;
			}
			Room room = Room.Get(player.Position) ?? player.CurrentRoom;
			return room != null && room.AreLightsOff;
		}
		catch
		{
			return false;
		}
	}

	public static string RoomName(Player player)
	{
		try
		{
			Room room = ((player == null) ? null : (Room.Get(player.Position) ?? player.CurrentRoom));
			if (room == null)
			{
				return "zona desconocida";
			}
			string text = room.Type.ToString();
			if (text.IndexOf("HczTestRoom", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return "914, en HCZ";
			}
			if (text.IndexOf("HczHid", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return "Micro HID, en HCZ";
			}
			if (text.IndexOf("Hcz079", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return "079, en HCZ";
			}
			if (text.IndexOf("Hcz049", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return "049, en HCZ";
			}
			if (text.IndexOf("Hcz106", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return "106, en HCZ";
			}
			if (text.IndexOf("HczArmory", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return "el armamento, en HCZ";
			}
			if (text.IndexOf("HczNuke", StringComparison.OrdinalIgnoreCase) >= 0 || text.IndexOf("HczWarhead", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return "la sala de la ojiva, en HCZ";
			}
			if (text.IndexOf("Hcz", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return "un pasillo de HCZ";
			}
			if (text.IndexOf("Lcz", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return "un pasillo de LCZ";
			}
			if (text.IndexOf("Ez", StringComparison.OrdinalIgnoreCase) >= 0 || text.IndexOf("Entrance", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return "un pasillo de Entrada";
			}
			if (text.IndexOf("Surface", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return "la Superficie";
			}
			return room.Zone switch
			{
				ZoneType.LightContainment => "Contención Ligera", 
				ZoneType.HeavyContainment => "Contención Pesada", 
				ZoneType.Entrance => "Entrada", 
				ZoneType.Surface => "la Superficie", 
				_ => "una zona desconocida", 
			};
		}
		catch
		{
			return "zona desconocida";
		}
	}

	public static bool TryTeleportToRoom(Player player, Room room)
	{
		if (player == null || room == null || !player.IsAlive)
		{
			return false;
		}
		Vector3 origin = room.Position + Vector3.up * 2.5f;
		if (!Physics.Raycast(origin, Vector3.down, out var hitInfo, 6f))
		{
			Log.Debug("[SubclassFramework] No se encontro suelo seguro para el teletransporte.");
			return false;
		}
		Vector3 vector = hitInfo.point + Vector3.up * 1.1f;
		if (float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z) || float.IsInfinity(vector.x) || float.IsInfinity(vector.y) || float.IsInfinity(vector.z) || vector.y < -100f || vector.y > 1000f)
		{
			return false;
		}
		try
		{
			if (Physics.CheckCapsule(vector + Vector3.up * 0.25f, vector + Vector3.up * 1.5f, 0.35f))
			{
				Log.Debug("[SubclassFramework] La posicion de teletransporte esta ocupada.");
				return false;
			}
			player.Position = vector;
			return true;
		}
		catch (Exception ex)
		{
			Log.Debug("[SubclassFramework] Teletransporte a sala fallido: " + ex.Message);
			return false;
		}
	}

	private static string Prettify(string raw)
	{
		if (string.IsNullOrEmpty(raw))
		{
			return raw;
		}
		StringBuilder stringBuilder = new StringBuilder(raw.Length + 6);
		for (int i = 0; i < raw.Length; i++)
		{
			char c = raw[i];
			if (i > 0 && char.IsUpper(c) && !char.IsUpper(raw[i - 1]))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append(c);
		}
		return stringBuilder.ToString();
	}

	public static IEnumerable<Player> AliveNear(Player origin, float radius)
	{
		if (origin == null)
		{
			yield break;
		}
		float sqr = radius * radius;
		Vector3 pos = origin.Position;
		foreach (Player p in Player.List)
		{
			if (!(p == null) && p.IsAlive && (p.Position - pos).sqrMagnitude <= sqr)
			{
				yield return p;
			}
		}
	}

	public static IEnumerable<Player> InSameRoom(Player origin)
	{
		if (origin == null)
		{
			yield break;
		}
		Room room = origin.CurrentRoom;
		if (room == null)
		{
			yield break;
		}
		foreach (Player p in Player.List)
		{
			if (p != null && p.IsAlive && p.CurrentRoom == room)
			{
				yield return p;
			}
		}
	}

	public static bool IsMtf(Player p)
	{
		if (p == null)
		{
			return false;
		}
		RoleTypeId type = p.Role.Type;
		RoleTypeId roleTypeId = type;
		if (roleTypeId == RoleTypeId.NtfSpecialist || (uint)(roleTypeId - 11) <= 2u)
		{
			return true;
		}
		return false;
	}

	public static bool IsChaos(Player p)
	{
		return p != null && p.Role.Team == Team.ChaosInsurgency;
	}

	public static bool IsGuard(Player p)
	{
		return p != null && p.Role.Type == RoleTypeId.FacilityGuard;
	}

	public static bool IsZombie(Player p)
	{
		return p != null && p.Role.Type == RoleTypeId.Scp0492;
	}

	public static bool HasTraumatized(Player p)
	{
		try
		{
			StatusEffectBase statusEffect;
			return p != null && p.TryGetEffect(EffectType.Traumatized, out statusEffect) && statusEffect != null && statusEffect.IsEnabled;
		}
		catch
		{
			return false;
		}
	}

	public static Player GetLookingAt(Player player, float maxDistance = 60f)
	{
		if (player == null || !player.IsAlive)
		{
			return null;
		}
		try
		{
			Transform cameraTransform = player.CameraTransform;
			if (cameraTransform == null)
			{
				return null;
			}
			if (!Physics.Raycast(cameraTransform.position, cameraTransform.forward, out var hitInfo, maxDistance))
			{
				return null;
			}
			Player player2 = null;
			Transform transform = hitInfo.collider?.transform;
			while (transform != null && player2 == null)
			{
				player2 = Player.Get(transform.gameObject);
				transform = transform.parent;
			}
			return (player2 != null && player2.IsAlive && player2 != player) ? player2 : null;
		}
		catch
		{
			return null;
		}
	}

	public static bool HasLineOfSight(Player origin, Player target)
	{
		if (origin == null || target == null || !origin.IsAlive || !target.IsAlive)
		{
			return false;
		}
		Vector3 vector = origin.Position + Vector3.up;
		Vector3 vector2 = target.Position + Vector3.up;
		Vector3 vector3 = vector2 - vector;
		float magnitude = vector3.magnitude;
		if (magnitude <= 0.01f)
		{
			return true;
		}
		RaycastHit[] source = Physics.RaycastAll(vector, vector3.normalized, magnitude);
		foreach (RaycastHit item in source.OrderBy((RaycastHit h) => h.distance))
		{
			Transform transform = item.collider?.transform;
			bool flag = false;
			while (transform != null)
			{
				if (transform.gameObject == origin.GameObject)
				{
					flag = true;
					break;
				}
				if (transform.gameObject == target.GameObject)
				{
					return true;
				}
				transform = transform.parent;
			}
			if (flag)
			{
				continue;
			}
			return false;
		}
		return false;
	}

	public static string ColorFor(Player player)
	{
		if (player == null)
		{
			return "#FFFFFF";
		}
		return player.Role.Type switch
		{
			RoleTypeId.ClassD => "#FF8000", 
			RoleTypeId.Scientist => "#FFFF7C", 
			RoleTypeId.FacilityGuard => "#C0C0C0", 
			_ => player.Role.Team switch
			{
				Team.FoundationForces => "#5DA5FF", 
				Team.ChaosInsurgency => "#4CE04C", 
				_ => "#FFFFFF", 
			}, 
		};
	}

	public static string Colored(string text, string hexColor)
	{
		return "<color=" + hexColor + ">" + text + "</color>";
	}

	public static void Hint(Player player, string message, float duration = 5f)
	{
		if (player == null || string.IsNullOrEmpty(message))
		{
			return;
		}
		try
		{
			duration = Mathf.Max(4f, duration);
			ActiveHints[player] = new ActiveHint
			{
				Message = message,
				Expiry = Time.time + duration
			};
			RenderHint(player, duration);
		}
		catch (Exception ex)
		{
			Log.Debug("[SubclassFramework] Hint fallo: " + ex.Message);
		}
	}

	public static void HudHint(Player player, string hudMessage, float duration)
	{
		if (player == null || string.IsNullOrEmpty(hudMessage))
		{
			return;
		}
		try
		{
			RenderHint(player, duration, hudMessage);
		}
		catch (Exception ex)
		{
			Log.Debug("[SubclassFramework] HUD fallo: " + ex.Message);
		}
	}

	public static void TransientHint(Player player, string message, float duration = 1.2f)
	{
		if (player == null || string.IsNullOrEmpty(message))
		{
			return;
		}
		try
		{
			player.ShowHint(message, duration);
		}
		catch (Exception ex)
		{
			Log.Debug("[SubclassFramework] Aviso transitorio fallo: " + ex.Message);
		}
	}

	public static void ClearHint(Player player)
	{
		if (player != null)
		{
			ActiveHints.Remove(player);
		}
	}

	public static bool HasActiveHint(Player player)
	{
		if (player == null || !ActiveHints.TryGetValue(player, out var value))
		{
			return false;
		}
		if (value.Expiry <= Time.time)
		{
			ActiveHints.Remove(player);
			return false;
		}
		return true;
	}

	public static void ClearMessages(Player player)
	{
		if (player == null)
		{
			return;
		}
		ActiveHints.Remove(player);
		try
		{
			player.ShowHint(string.Empty, 0.1f);
		}
		catch (Exception ex)
		{
			Log.Debug("[SubclassFramework] No se pudieron limpiar los mensajes: " + ex.Message);
		}
	}

	public static void ClearAllMessages()
	{
		foreach (Player item in ActiveHints.Keys.ToList())
		{
			ClearMessages(item);
		}
		ActiveHints.Clear();
	}

	public static float RecommendedTextDuration(string plainText, float maximum = 9f)
	{
		int num = Regex.Replace(plainText ?? string.Empty, "<.*?>", string.Empty).Split(new char[4] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
		float num2 = Mathf.Clamp(((float)num - 4f) / 16f, 0f, 1f);
		return Mathf.Clamp(5f + num2 * 4f, 5f, Mathf.Max(5f, maximum));
	}

	public static void BroadcastTop(Player player, string message, float duration)
	{
		if (player != null && !string.IsNullOrEmpty(message))
		{
			Hint(player, message, duration);
		}
	}

	public static void SpectatorHint(Player spectator, string message)
	{
		if (spectator == null || string.IsNullOrEmpty(message))
		{
			return;
		}
		string message2 = "<size=17><color=#d6dee8>" + message + "</color></size>";
		try
		{
			spectator.ShowHint(message2, 2.2f);
		}
		catch (Exception ex)
		{
			Log.Debug("[SubclassFramework] Aviso de espectador fallo: " + ex.Message);
		}
	}

	private static void RenderHint(Player player, float duration, string hudMessage = null)
	{
		ActiveHints.TryGetValue(player, out var value);
		string text = hudMessage;
		if (string.IsNullOrEmpty(text) && value != null && value.Expiry > Time.time)
		{
			text = value.Message;
		}
		if (!string.IsNullOrEmpty(text))
		{
			player.ShowHint(text, duration);
		}
	}

	public static int CountItem(Player player, ItemType type)
	{
		return (player?.Items != null) ? player.Items.Count((Item i) => i.Type == type) : 0;
	}

	public static void GiveAmmoForHeldWeapon(Player player, ushort amount)
	{
		if (!(player == null))
		{
			AmmoType ammoType = AmmoTypeOf(player.CurrentItem?.Type ?? ItemType.None);
			if (ammoType != AmmoType.None)
			{
				player.AddAmmo(ammoType, amount);
			}
		}
	}

	public static AmmoType AmmoTypeOf(ItemType weapon)
	{
		switch (weapon)
		{
		case ItemType.GunCOM15:
		case ItemType.GunCrossvec:
		case ItemType.GunFSP9:
		case ItemType.GunCOM18:
		case ItemType.GunCom45:
			return AmmoType.Nato9;
		case ItemType.GunE11SR:
		case ItemType.GunA7:
			return AmmoType.Nato556;
		case ItemType.GunLogicer:
		case ItemType.GunAK:
		case ItemType.GunFRMG0:
			return AmmoType.Nato762;
		case ItemType.GunShotgun:
			return AmmoType.Ammo12Gauge;
		case ItemType.GunRevolver:
			return AmmoType.Ammo44Cal;
		default:
			return AmmoType.None;
		}
	}
}
