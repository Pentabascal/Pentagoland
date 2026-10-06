using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;

namespace UltimateHUD
{
    public static class ServerSettings
    {
        public static TwoButtonsSetting HUD { get; private set; }
        public static TwoButtonsSetting Clock { get; private set; }
        public static TwoButtonsSetting Tps { get; private set; }
        public static TwoButtonsSetting RoundTime { get; private set; }
        public static TwoButtonsSetting PlayerHUD { get; private set; }
        public static TwoButtonsSetting SpectatorList { get; private set; }
        public static TwoButtonsSetting AmmoCounter { get; private set; }
        public static TwoButtonsSetting SpectatorHUD { get; private set; }
        public static void RegisterSettings()
        {
            HUD = new TwoButtonsSetting(
                id: 5554,
                label: "Mostrar HUD",
                firstOption: "Activado",
                secondOption: "Desactivado",
                defaultIsSecond: false,
                hintDescription: "Activa o desactiva todos los elementos del HUD.",
                onChanged: (player, setting) =>
                {
                    var showHUD = (setting as TwoButtonsSetting)?.IsFirst ?? true;
                    player.SessionVariables["ShowHUD"] = showHUD;
                });

            SettingBase.Register(new[] { HUD });

            if (Plugin.Instance.Config.EnableClock)
            {
                Clock = new TwoButtonsSetting(
                    id: 5555,
                    label: "Reloj",
                    firstOption: "Activado",
                    secondOption: "Desactivado",
                    defaultIsSecond: false,
                    hintDescription: "Muestra u oculta el reloj.",
                    onChanged: (player, setting) =>
                    {
                        var showClock = (setting as TwoButtonsSetting)?.IsFirst ?? true;
                        player.SessionVariables["ShowClock"] = showClock;
                    });

                SettingBase.Register(new[] { Clock });
            }

            if (Plugin.Instance.Config.EnableTps)
            {
                Tps = new TwoButtonsSetting(
                    id: 5556,
                    label: "TPS",
                    firstOption: "Activado",
                    secondOption: "Desactivado",
                    defaultIsSecond: false,
                    hintDescription: "Muestra u oculta los TPS.",
                    onChanged: (player, setting) =>
                    {
                        var showTps = (setting as TwoButtonsSetting)?.IsFirst ?? true;
                        player.SessionVariables["ShowTps"] = showTps;
                    });

                SettingBase.Register(new[] { Tps });
            }

            if (Plugin.Instance.Config.EnableRoundTime)
            {
                RoundTime = new TwoButtonsSetting(
                    id: 5557,
                    label: "Duración de ronda",
                    firstOption: "Activado",
                    secondOption: "Desactivado",
                    defaultIsSecond: false,
                    hintDescription: "Muestra u oculta la duración de la ronda.",
                    onChanged: (player, setting) =>
                    {
                        var showRoundTime = (setting as TwoButtonsSetting)?.IsFirst ?? true;
                        player.SessionVariables["ShowRoundTime"] = showRoundTime;
                    });

                SettingBase.Register(new[] { RoundTime });
            }

            if (Plugin.Instance.Config.EnablePlayerHud)
            {
                PlayerHUD = new TwoButtonsSetting(
                    id: 5558,
                    label: "HUD de jugador",
                    firstOption: "Activado",
                    secondOption: "Desactivado",
                    defaultIsSecond: false,
                    hintDescription: "Muestra u oculta la información del jugador.",
                    onChanged: (player, setting) =>
                    {
                        var showPlayerHUD = (setting as TwoButtonsSetting)?.IsFirst ?? true;
                        player.SessionVariables["ShowPlayerHUD"] = showPlayerHUD;
                    });

                SettingBase.Register(new[] { PlayerHUD });
            }

            if (Plugin.Instance.Config.EnableSpectatorList)
            {
                SpectatorList = new TwoButtonsSetting(
                    id: 5559,
                    label: "Lista de espectadores",
                    firstOption: "Activado",
                    secondOption: "Desactivado",
                    defaultIsSecond: false,
                    hintDescription: "Muestra u oculta la lista de espectadores.",
                    onChanged: (player, setting) =>
                    {
                        var showSpectatorList = (setting as TwoButtonsSetting)?.IsFirst ?? true;
                        player.SessionVariables["ShowSpectatorList"] = showSpectatorList;
                    });

                SettingBase.Register(new[] { SpectatorList });
            }

            if (Plugin.Instance.Config.EnableAmmoCounter)
            {
                AmmoCounter = new TwoButtonsSetting(
                    id: 5560,
                    label: "Contador de munición",
                    firstOption: "Activado",
                    secondOption: "Desactivado",
                    defaultIsSecond: false,
                    hintDescription: "Muestra u oculta el contador de munición.",
                    onChanged: (player, setting) =>
                    {
                        var showAmmoCounter = (setting as TwoButtonsSetting)?.IsFirst ?? true;
                        player.SessionVariables["ShowAmmoCounter"] = showAmmoCounter;
                    });

                SettingBase.Register(new[] { AmmoCounter });
            }

            if (Plugin.Instance.Config.EnableSpectatorHud)
            {
                SpectatorHUD = new TwoButtonsSetting(
                    id: 5561,
                    label: "HUD de espectador",
                    firstOption: "Activado",
                    secondOption: "Desactivado",
                    defaultIsSecond: false,
                    hintDescription: "Muestra u oculta la información del espectador.",
                    onChanged: (player, setting) =>
                    {
                        var showSpectatorHUD = (setting as TwoButtonsSetting)?.IsFirst ?? true;
                        player.SessionVariables["ShowSpectatorHUD"] = showSpectatorHUD;
                    });

                SettingBase.Register(new[] { SpectatorHUD });
            }
        }
        public static void UnregisterSettings()
        {
            SettingBase.Unregister(settings: new[] { HUD });

            if (Clock != null)
                SettingBase.Unregister(settings: new[] { Clock });

            if (Tps != null)
                SettingBase.Unregister(settings: new[] { Tps });

            if (RoundTime != null)
                SettingBase.Unregister(settings: new[] { RoundTime });

            if (PlayerHUD != null)
                SettingBase.Unregister(settings: new[] { PlayerHUD });

            if (SpectatorList != null)
                SettingBase.Unregister(settings: new[] { SpectatorList });

            if (AmmoCounter != null)
                SettingBase.Unregister(settings: new[] { AmmoCounter });

            if (SpectatorHUD != null)
                SettingBase.Unregister(settings: new[] { SpectatorHUD });
        }

        public static bool ShouldShowHUD(Player player) => !(player.SessionVariables.TryGetValue("ShowHUD", out var value) && value is bool enabled && !enabled);
        public static bool ShouldShowClock(Player player) => !(player.SessionVariables.TryGetValue("ShowClock", out var value) && value is bool enabled && !enabled);
        public static bool ShouldShowTps(Player player) => !(player.SessionVariables.TryGetValue("ShowTps", out var value) && value is bool enabled && !enabled);
        public static bool ShouldShowRoundTime(Player player) => !(player.SessionVariables.TryGetValue("ShowRoundTime", out var value) && value is bool enabled && !enabled);
        public static bool ShouldShowPlayerHUD(Player player) => !(player.SessionVariables.TryGetValue("ShowPlayerHUD", out var value) && value is bool enabled && !enabled);
        public static bool ShouldShowSpectatorList(Player player) => !(player.SessionVariables.TryGetValue("ShowSpectatorList", out var value) && value is bool enabled && !enabled);
        public static bool ShouldShowAmmoCounter(Player player) => !(player.SessionVariables.TryGetValue("ShowAmmoCounter", out var value) && value is bool enabled && !enabled);
        public static bool ShouldShowSpectatorHUD(Player player) => !(player.SessionVariables.TryGetValue("ShowSpectatorHUD", out var value) && value is bool enabled && !enabled);
    
    }
}