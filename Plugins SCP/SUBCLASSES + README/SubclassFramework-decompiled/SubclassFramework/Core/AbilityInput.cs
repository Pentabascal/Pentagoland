using System;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using Exiled.Events.EventArgs.Player;
using UnityEngine;
using UserSettings.ServerSpecific;

namespace SubclassFramework.Core;

public static class AbilityInput
{
	private const int HeaderId = 774204;

	public const int KeybindId = 774203;

	private static SettingBase[] registeredSettings = Array.Empty<SettingBase>();

	private static bool settingsRegistered;

	public static SettingBase[] GetSettings()
	{
		Plugin instance = Plugin.Instance;
		if (instance == null || instance.Config?.UseServerSpecificKeybind != true)
		{
			return Array.Empty<SettingBase>();
		}
		return new SettingBase[2]
		{
			new HeaderSetting(HeaderId, "Subclases", string.Empty),
			new KeybindSetting(KeybindId, "Activar habilidad de Subclase", ParseDefaultKey(), preventInteractionOnGUI: true, allowSpectatorTrigger: false, "Pulsa esta tecla para usar la habilidad activa de tu subclase.", 0)
		};
	}

	public static void Register()
	{
		if (settingsRegistered)
		{
			return;
		}
		SettingBase[] settings = GetSettings();
		if (settings.Length == 0)
		{
			return;
		}
		SettingBase.Register(settings);
		registeredSettings = settings;
		Exiled.Events.Handlers.Player.Verified += OnVerified;
		ServerSpecificSettingsSync.ServerOnSettingValueReceived += OnSettingValueReceived;
		settingsRegistered = true;
		foreach (Player player in Player.List)
		{
			if (player?.ReferenceHub != null)
			{
				ServerSpecificSettingsSync.SendToPlayer(player.ReferenceHub);
			}
		}
		Log.Info("[SubclassFramework] Ajustes SSSS registrados.");
	}

	public static void Unregister()
	{
		if (!settingsRegistered)
		{
			return;
		}
		Exiled.Events.Handlers.Player.Verified -= OnVerified;
		ServerSpecificSettingsSync.ServerOnSettingValueReceived -= OnSettingValueReceived;
		SettingBase.Unregister(settings: registeredSettings);
		registeredSettings = Array.Empty<SettingBase>();
		settingsRegistered = false;
	}

	private static void OnVerified(VerifiedEventArgs ev)
	{
		if (settingsRegistered && ev?.Player?.ReferenceHub != null)
		{
			ServerSpecificSettingsSync.SendToPlayer(ev.Player.ReferenceHub);
		}
	}

	private static void OnSettingValueReceived(ReferenceHub hub, ServerSpecificSettingBase setting)
	{
		if (Plugin.Instance?.Config?.UseServerSpecificKeybind == true && setting is SSKeybindSetting { SyncIsPressed: not false, SettingId: KeybindId } && Player.TryGet(hub, out var player) && SubclassManager.Instance != null)
		{
			SubclassManager.Instance.TryUseAbility(player);
		}
	}

	private static KeyCode ParseDefaultKey()
	{
		string value = Plugin.Instance?.Config?.DefaultAbilityKey;
		KeyCode result;
		return Enum.TryParse<KeyCode>(value, ignoreCase: true, out result) ? result : KeyCode.H;
	}
}
