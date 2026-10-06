using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using PlayerRoles;
using RueI.API;
using RueI.API.Elements;
using RueI.API.Elements.Enums;
using RueI.Utils;
using SubclassFramework.Core;

namespace UltimateHUD
{
    public static class Hints
    {
        private static Config Config => Plugin.Instance.Config;
        private static Translations Translation => Plugin.Instance.Translation;

        private static readonly Tag ClockTag = new("hud_clock");
        private static readonly Tag TpsTag = new("hud_tps");
        private static readonly Tag RoundTimeTag = new("hud_roundtime");
        private static readonly Tag PlayerInfoTag = new("hud_playerinfo");
        private static readonly Tag SubclassInfoTag = new("hud_subclassinfo");
        private static readonly Tag AmmoTag = new("hud_ammo");
        private static readonly Tag SpectatorsListTag = new("hud_spectatorslist");
        private static readonly Tag SpectatorPlayerInfoTag = new("hud_spec_playerinfo");
        private static readonly Tag SpectatorBottomInfoTag = new("hud_spec_bottominfo");
        private static readonly Tag LegacySpectatorServerInfoTag = new("hud_spec_serverinfo");
        private static readonly Tag LegacySpectatorMapInfoTag = new("hud_spec_mapinfo");
        private static readonly Regex RichTextTagRegex = new("<[^>]*>", RegexOptions.Compiled);
        private static readonly Regex HudPositioningTagRegex = new("</?align(?:\\s*=[^>]*)?>|<space\\s*=[^>]*>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static void RefreshAll(Player player)
        {
            if (!CanDisplay(player))
                return;

            RefreshClock(player);
            RefreshTps(player);
            RefreshRoundTime(player);
            RefreshPlayerInfo(player);
            RefreshSubclassInfo(player);
            RefreshAmmo(player);
            RefreshSpectatorList(player);
            RefreshSpectatorBottomInfo(player);
        }

        public static void RemoveAll(Player player)
        {
            RueDisplay display = RueDisplay.Get(player);
            display.Remove(ClockTag);
            display.Remove(TpsTag);
            display.Remove(RoundTimeTag);
            display.Remove(PlayerInfoTag);
            display.Remove(SubclassInfoTag);
            display.Remove(AmmoTag);
            display.Remove(SpectatorsListTag);
            display.Remove(SpectatorPlayerInfoTag);
            display.Remove(SpectatorBottomInfoTag);
            display.Remove(LegacySpectatorServerInfoTag);
            display.Remove(LegacySpectatorMapInfoTag);
        }

        // =============== AUTO-REFRESH ===============

        public static void RefreshClock(Player player)
        {
            if (!CanDisplay(player))
                return;

            if (!Config.EnableClock) { RueDisplay.Get(player).Remove(ClockTag); return; }

            var element = new DynamicElement(
                position: Config.ClockYCordinate,
                contentGetter: rh =>
                {
                    var p = Player.Get(rh);

                    if (p == null ||
                        !Round.InProgress ||
                        !Options.ShouldShow(Config.ClockVisual, p) ||
                        !ServerSettings.ShouldShowClock(p) ||
                        !ServerSettings.ShouldShowHUD(p))
                        return string.Empty;

                    DateTime utc = DateTime.UtcNow.AddHours(Config.TimeZone);
                    string color = Options.GetRoleColor(p);

                    return Config.Clock
                        .Replace("{color}", color)
                        .Replace("{time}", utc.ToString("HH:mm"));
                })
            {
                ZIndex = 2,
                ResolutionBasedAlign = true,
                VerticalAlign = VerticalAlign.Down,
                UpdateInterval = TimeSpan.FromSeconds(1),
                ShowToSpectators = true
            };

            RueDisplay.Get(player).Show(ClockTag, element);
        }

        public static void RefreshTps(Player player)
        {
            if (!CanDisplay(player))
                return;

            if (!Config.EnableTps) { RueDisplay.Get(player).Remove(TpsTag); return; }

            var element = new DynamicElement(
                position: Config.TpsYCordinate,
                contentGetter: rh =>
                {
                    var p = Player.Get(rh);
                    if (p == null ||
                        !Round.InProgress ||
                        !Options.ShouldShow(Config.TpsVisual, p) ||
                        !ServerSettings.ShouldShowTps(p) ||
                        !ServerSettings.ShouldShowHUD(p))
                        return string.Empty;

                    int tps = (int)Server.Tps;
                    int maxTps = (int)Server.MaxTps;
                    string color = Options.GetRoleColor(p);

                    return Config.Tps
                        .Replace("{color}", color)
                        .Replace("{tps}", tps.ToString())
                        .Replace("{maxTps}", maxTps.ToString());
                })
            {
                ZIndex = 2,
                ResolutionBasedAlign = true,
                VerticalAlign = VerticalAlign.Down,
                UpdateInterval = TimeSpan.FromSeconds(1),
                ShowToSpectators = true
            };

            RueDisplay.Get(player).Show(TpsTag, element);
        }

        public static void RefreshRoundTime(Player player)
        {
            if (!CanDisplay(player))
                return;

            if (!Config.EnableRoundTime) { RueDisplay.Get(player).Remove(RoundTimeTag); return; }

            var element = new DynamicElement(
                position: Config.RoundTimeYCordinate,
                contentGetter: rh =>
                {
                    var p = Player.Get(rh);

                    if (p == null ||
                        !Round.InProgress ||
                        !Options.ShouldShow(Config.RoundTimeVisual, p) ||
                        !ServerSettings.ShouldShowRoundTime(p) ||
                        !ServerSettings.ShouldShowHUD(p))
                        return string.Empty;

                    TimeSpan elapsed = Round.ElapsedTime;
                    string formatted = elapsed.ToString(@"mm\:ss");
                    string color = Options.GetRoleColor(p);

                    return Config.RoundTime
                        .Replace("{color}", color)
                        .Replace("{round_time}", formatted);
                })
            {
                ZIndex = 2,
                ResolutionBasedAlign = true,
                VerticalAlign = VerticalAlign.Down,
                UpdateInterval = TimeSpan.FromSeconds(1),
                ShowToSpectators = true
            };

            RueDisplay.Get(player).Show(RoundTimeTag, element);
        }

        // =============== EVENTBASE-REFRESH  ===============

        public static void RefreshPlayerInfo(Player player)
        {
            if (!CanDisplay(player))
                return;

            if (!Config.EnablePlayerHud)
            {
                RueDisplay.Get(player).Remove(PlayerInfoTag);
                return;
            }

            var element = new DynamicElement(
                position: Config.PlayerHudYCordinate,
                contentGetter: rh =>
                {
                    var p = Player.Get(rh);
                    if (p == null ||
                        p.Role is SpectatorRole ||
                        !ServerSettings.ShouldShowPlayerHUD(p) ||
                        !ServerSettings.ShouldShowHUD(p))
                        return string.Empty;

                    return BuildPlayerInfo(p);
                })
            {
                ZIndex = 5,
                ResolutionBasedAlign = true,
                VerticalAlign = VerticalAlign.Up,
                UpdateInterval = TimeSpan.FromSeconds(1),
                ShowToSpectators = false
            };

            RueDisplay.Get(player).Show(PlayerInfoTag, element);
        }

        public static void RefreshSubclassInfo(Player player)
        {
            if (!CanDisplay(player))
                return;

            if (!Config.EnablePlayerHud)
            {
                RueDisplay.Get(player).Remove(SubclassInfoTag);
                return;
            }

            var element = new DynamicElement(
                position: Config.SubclassHudYCordinate,
                contentGetter: rh =>
                {
                    var p = Player.Get(rh);
                    if (p == null ||
                        p.Role is SpectatorRole ||
                        !ServerSettings.ShouldShowPlayerHUD(p) ||
                        !ServerSettings.ShouldShowHUD(p))
                        return string.Empty;

                    var subclass = SubclassManager.Instance?.GetSubclass(p);
                    if (subclass == null || string.IsNullOrWhiteSpace(subclass.Name))
                        return string.Empty;

                    return Config.SubclassHud.Replace(
                        "{subclass}",
                        HintBuilding.Sanitize(subclass.Name));
                })
            {
                ZIndex = 5,
                ResolutionBasedAlign = true,
                VerticalAlign = VerticalAlign.Up,
                UpdateInterval = TimeSpan.FromSeconds(1),
                ShowToSpectators = false
            };

            RueDisplay.Get(player).Show(SubclassInfoTag, element);
        }

        public static void RefreshAmmo(Player player)
        {
            if (!CanDisplay(player))
                return;

            if (!Config.EnableAmmoCounter) { RueDisplay.Get(player).Remove(AmmoTag); return; }

            var p = Player.Get(player);

            if (p == null ||
                p.Role is SpectatorRole ||
                p.CurrentItem is not Firearm firearm ||
                !ServerSettings.ShouldShowAmmoCounter(p) ||
                !ServerSettings.ShouldShowHUD(p))
            {
                RueDisplay.Get(player).Remove(AmmoTag);
                return;
            }

            string color = Options.GetRoleColor(p);
            string weapon = HintBuilding.Sanitize(Translation.GetWeaponDisplayName(firearm));

            string weaponLine = Config.WeaponName
                .Replace("{color}", color)
                .Replace("{weapon}", weapon);

            string ammoLine = Config.AmmoCounter
                .Replace("{color}", color)
                .Replace("{current}", firearm.TotalAmmo.ToString())
                .Replace("{max}", firearm.TotalMaxAmmo.ToString());

            string final = weaponLine + "\n" + ammoLine;

            var element = new BasicElement(Config.AmmoCounterYCordinate, final)
            {
                ZIndex = 6,
                ResolutionBasedAlign = true,
                VerticalAlign = VerticalAlign.Down
            };

            RueDisplay.Get(player).Show(AmmoTag, element);
        }

        public static void RefreshSpectatorList(Player player)
        {
            if (!CanDisplay(player))
                return;

            if (!Config.EnableSpectatorList) { RueDisplay.Get(player).Remove(SpectatorsListTag); return; }

            var p = Player.Get(player);

            if (p == null ||
                !p.IsAlive ||
                p.Role is SpectatorRole ||
                Config.HiddenForRoles.Contains(p.Role.Type) ||
                !ServerSettings.ShouldShowSpectatorList(p) ||
                !ServerSettings.ShouldShowHUD(p))
            {
                RueDisplay.Get(player).Remove(SpectatorsListTag);
                return;
            }

            var spectators = p.CurrentSpectatingPlayers
                .Where(s => s.Role != PlayerRoles.RoleTypeId.Overwatch)
                .ToList();

            if (spectators.Count == 0)
            {
                RueDisplay.Get(player).Remove(SpectatorsListTag);
                return;
            }

            string color = Options.GetRoleColor(p);
            var sb = new StringBuilder();
            sb.AppendLine(
                Config.SpectatorListHeader
                    .Replace("{count}", spectators.Count.ToString())
                    .Replace("{color}", color)
            );

            foreach (var spec in spectators)
            {
                string nick = TruncAndSanitize(spec.Nickname, 25);
                sb.AppendLine(
                    Config.SpectatorListPlayers
                        .Replace("{nickname}", nick)
                        .Replace("{color}", color)
                );
            }

            string text = sb.ToString().TrimEnd();

            var element = new BasicElement(Config.SpectatorListYCordinate, text)
            {
                ZIndex = 4,
                ResolutionBasedAlign = true,
                VerticalAlign = VerticalAlign.Down
            };

            RueDisplay.Get(player).Show(SpectatorsListTag, element);
        }

        public static void RefreshSpectatorPlayerInfo(Player player)
        {
            RefreshSpectatorBottomInfo(player);
        }

        public static void RefreshSpectatorServerInfo(Player player)
        {
            RefreshSpectatorBottomInfo(player);
        }

        public static void RefreshSpectatorMapInfo(Player player)
        {
            RefreshSpectatorBottomInfo(player);
        }

        private static void RefreshSpectatorBottomInfo(Player player)
        {
            if (!CanDisplay(player))
                return;

            RueDisplay display = RueDisplay.Get(player);
            display.Remove(SpectatorPlayerInfoTag);
            display.Remove(LegacySpectatorServerInfoTag);
            display.Remove(LegacySpectatorMapInfoTag);

            if (!Config.EnableSpectatorHud &&
                !Config.EnableSpectatorServerInfo &&
                !Config.EnableSpectatorMapInfo)
            {
                display.Remove(SpectatorBottomInfoTag);
                return;
            }

            var element = new DynamicElement(
                position: Config.SpectatorHudYCordinate,
                contentGetter: rh =>
                {
                    var p = Player.Get(rh);
                    if (p?.Role is not SpectatorRole specRole ||
                        !ServerSettings.ShouldShowSpectatorHUD(p) ||
                        !ServerSettings.ShouldShowHUD(p))
                        return string.Empty;

                    var sections = new System.Collections.Generic.List<string>(2);
                    var serverAndMapInfo = new System.Collections.Generic.List<string>(2);

                    if (Config.EnableSpectatorHud)
                    {
                        Player observed = specRole.SpectatedPlayer;
                        if (observed != null && observed.IsConnected)
                        {
                            string observedInfo = RemoveHudPositioningTags(BuildSpectatorObservedInfo(observed));
                            if (!string.IsNullOrWhiteSpace(observedInfo))
                                sections.Add(observedInfo);
                        }
                    }

                    if (Config.EnableSpectatorServerInfo)
                    {
                        int totalPlayers = Player.List.Count(pl => !pl.IsHost);
                        int maxPlayers = Server.MaxPlayerCount;
                        int spectators = Player.List.Count(pl => pl.Role is SpectatorRole && !pl.IsHost);
                        string serverInfo = Config.SpectatorServerInfo ?? string.Empty;

                        if (!string.IsNullOrWhiteSpace(serverInfo))
                            serverAndMapInfo.Add(serverInfo
                            .Replace("{players}", totalPlayers.ToString())
                            .Replace("{maxPlayers}", maxPlayers.ToString())
                            .Replace("{spectators}", spectators.ToString()));
                    }

                    if (Config.EnableSpectatorMapInfo)
                    {
                        int engaged = Generator.List.Count(g => g.IsEngaged);
                        const int maxGenerators = 3;
                        string warheadStatus = Translation.GetWarheadStatusName(Warhead.Status);
                        string warheadColor = Translation.GetWarheadStatusColor(Warhead.Status);
                        string mapInfo = Config.SpectatorMapInfo ?? string.Empty;

                        if (!string.IsNullOrWhiteSpace(mapInfo))
                            serverAndMapInfo.Add(mapInfo
                            .Replace("{engaged}", engaged.ToString())
                            .Replace("{maxGenerators}", maxGenerators.ToString())
                            .Replace("{warheadColor}", warheadColor)
                            .Replace("{warheadStatus}", warheadStatus));
                    }

                    if (serverAndMapInfo.Count > 0)
                        sections.Add(string.Join(" <size=20><color=#94A3B8>|</color></size> ", serverAndMapInfo));

                    if (sections.Count == 0)
                        return string.Empty;

                    return $"<align=center>{string.Join("\n", sections.Select(RemoveHudPositioningTags))}</align>";
                })
            {
                ZIndex = 4,
                ResolutionBasedAlign = true,
                VerticalAlign = VerticalAlign.Down,
                UpdateInterval = TimeSpan.FromSeconds(1),
                ShowToSpectators = true
            };

            display.Show(SpectatorBottomInfoTag, element);
        }

        // =============== BUILDERS ===============

        private static string BuildPlayerInfo(Player p)
        {
            string roleColor = Options.GetRoleColor(p);
            string nickname = TruncAndSanitize(p.Nickname, 20);
            string displayName = TruncAndSanitize(RemoveStarSuffix(p.DisplayNickname), 20);

            uint id = (uint)p.Id;
            string roleDisplay = StripRichTextTags(Translation.GetRoleDisplayName(p));
            string coloredRole = $"<color={roleColor}>{HintBuilding.Sanitize(roleDisplay)}</color>";
            int kills = EventHandlers.GetKills(p);

            return Config.PlayerHud
                .Replace("{nickname}", nickname)
                .Replace("{displayname}", displayName)
                .Replace("{id}", id.ToString())
                .Replace("{role}", coloredRole)
                .Replace("{kills}", kills.ToString())
                .Replace("{subclass}", string.Empty);
        }

        private static string BuildSpectatorObservedInfo(Player observed)
        {
            string roleColor = Options.GetRoleColor(observed);
            string nickname = TruncAndSanitize(observed.Nickname, 16);
            string displayName = TruncAndSanitize(RemoveStarSuffix(observed.DisplayNickname), 16);
            uint id = (uint)observed.Id;

            if (Config.HideSkeletonNickname && observed.Role.Type == RoleTypeId.Scp3114)
                nickname = TruncAndSanitize(StripRichTextTags(Translation.GetRoleDisplayName(observed)), 16);

            string roleDisplay = StripRichTextTags(Translation.GetRoleDisplayName(observed));
            string coloredRole = $"<color={roleColor}>{HintBuilding.Sanitize(roleDisplay)}</color>";
            int kills = EventHandlers.GetKills(observed);

            return Config.SpectatorHud
                .Replace("{nickname}", nickname)
                .Replace("{displayname}", displayName)
                .Replace("{id}", id.ToString())
                .Replace("{role}", coloredRole)
                .Replace("{kills}", kills.ToString())
                .Replace("{subclass}", string.Empty);
        }

        // =============== HELPERS ===============

        private static bool CanDisplay(Player player)
        {
            if (player != null && Round.InProgress)
                return true;

            if (player != null)
                RemoveAll(player);

            return false;
        }

        private static string RemoveStarSuffix(string display) => Regex.Replace(display, "<color=#855439>\\*</color>$", "");

        private static string TruncAndSanitize(string str, int max)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            if (str.Length > max)
                str = str.Substring(0, max) + "...";

            return HintBuilding.Sanitize(str);
        }

        private static string StripRichTextTags(string text) =>
            string.IsNullOrEmpty(text) ? string.Empty : RichTextTagRegex.Replace(text, string.Empty);

        private static string RemoveHudPositioningTags(string text) =>
            string.IsNullOrEmpty(text) ? string.Empty : HudPositioningTagRegex.Replace(text, string.Empty);
    }
}