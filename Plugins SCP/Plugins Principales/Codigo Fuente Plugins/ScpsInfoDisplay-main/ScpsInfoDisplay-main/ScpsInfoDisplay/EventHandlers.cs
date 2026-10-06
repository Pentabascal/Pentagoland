using Exiled.API.Features;
using Exiled.API.Features.Roles;
using MEC;
using NorthwoodLib.Pools;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using RueI.API;
using RueI.API.Elements;
using RueI.API.Elements.Enums;
using Player = Exiled.API.Features.Player;

namespace ScpsInfoDisplay
{
    internal sealed class EventHandlers
    {
        private static readonly Tag DisplayTag = new Tag("scps_info_display");
        private CoroutineHandle _displayCoroutine;
        
        internal void OnRoundStarted()
        {
            StartDisplayLoop();
        }

        internal void StartDisplayLoop()
        {
            if (!_displayCoroutine.IsRunning)
                _displayCoroutine = Timing.RunCoroutine(ShowDisplay());
        }

        internal void StopDisplayLoop()
        {
            if (_displayCoroutine.IsRunning)
                Timing.KillCoroutines(_displayCoroutine);

            ClearAllDisplays();
        }

        internal static void ClearAllDisplays()
        {
            foreach (Player player in Player.List)
                RueDisplay.Get(player).Remove(DisplayTag);
        }

        private IEnumerator<float> ShowDisplay()
        {
            bool roundWasInProgress = false;
            while (true)
            {
                yield return Timing.WaitForSeconds(1f);

                if (!Round.InProgress)
                {
                    if (roundWasInProgress)
                        ClearAllDisplays();

                    roundWasInProgress = false;
                    continue;
                }

                roundWasInProgress = true;
                List<Player> players = Player.List.Where(player => player != null).ToList();
                Config config = ScpsInfoDisplay.Singleton.Config;

                foreach (Player player in players)
                {
                    try
                    {
                        bool isScp = player.Role.Team == Team.SCPs;
                        bool hasCustomRole = config.CustomRolesIntegrations.Keys
                            .Any(key => player.SessionVariables.ContainsKey(key));

                        if (!isScp && !hasCustomRole)
                        {
                            RueDisplay.Get(player).Remove(DisplayTag);
                            continue;
                        }

                        StringBuilder builder = StringBuilderPool.Shared.Rent(
                            $"<align={config.TextAlignment.ToString().ToLowerInvariant()}><size={config.TextSize}>");
                        HashSet<int> displayedPlayerIds = new();

                        foreach (KeyValuePair<string, string> integration in config.CustomRolesIntegrations)
                        {
                            foreach (Player target in players.Where(target =>
                                         target.SessionVariables.ContainsKey(integration.Key) &&
                                         target.Role.Type != RoleTypeId.Scp0492 &&
                                         !displayedPlayerIds.Contains(target.Id)))
                            {
                                displayedPlayerIds.Add(target.Id);
                                AppendPlayerLine(builder, config, player, target,
                                    ProcessStringVariables(integration.Value, player, target));
                            }
                        }

                        foreach (Player target in players.Where(target =>
                                     target.Role.Team == Team.SCPs &&
                                     target.Role.Type != RoleTypeId.Scp0492 &&
                                     config.DisplayStrings.ContainsKey(target.Role.Type) &&
                                     !displayedPlayerIds.Contains(target.Id)))
                        {
                            displayedPlayerIds.Add(target.Id);
                            AppendPlayerLine(builder, config, player, target,
                                ProcessStringVariables(config.DisplayStrings[target.Role.Type], player, target));
                        }

                        builder.Append("</size></align>");
                        string content = StringBuilderPool.Shared.ToStringReturn(builder);
                        RueDisplay.Get(player).Show(DisplayTag,
                            new BasicElement(config.TextVerticalPosition, content)
                            {
                                ZIndex = 3,
                                ResolutionBasedAlign = true,
                                VerticalAlign = VerticalAlign.Down
                            });
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"ScpsInfoDisplay failed to refresh the display for '{player.Nickname}': {ex}");
                    }
                }
            }
        }

        private static void AppendPlayerLine(StringBuilder builder, Config config, Player observer, Player target, string text)
        {
            int horizontalOffset = Math.Max(0, Math.Min(config.TextHorizontalOffset, 850));
            if (horizontalOffset > 0)
                builder.Append("<space=").Append(horizontalOffset).Append('>');

            if (target == observer && !string.IsNullOrWhiteSpace(config.PlayersMarker))
                builder.Append(config.PlayersMarker).Append(' ');

            builder.Append(text).Append('\n');
        }

        private string ProcessStringVariables(string raw, Player observer, Player target)
        {
            string humeShield = Math.Ceiling(target.HumeShield).ToString(CultureInfo.InvariantCulture);
            string artificialHealth = Math.Floor(target.ArtificialHealth).ToString(CultureInfo.InvariantCulture);
            string scp079Level = string.Empty;
            string scp079Energy = string.Empty;
            string scp079EnergyPercent = string.Empty;

            if (target.Role.Is(out Scp079Role scp079))
            {
                scp079Level = scp079.Level.ToString(CultureInfo.InvariantCulture);
                scp079Energy = Math.Floor(scp079.Energy).ToString(CultureInfo.InvariantCulture);
                double energy = Math.Max(0d, Math.Min(100d, scp079.Energy));
                scp079EnergyPercent = (Math.Floor(energy / 10d) * 10d)
                    .ToString("0", CultureInfo.InvariantCulture);
            }

            return raw
            .Replace("%humeshield%", humeShield)
            .Replace("%arhealth%", artificialHealth)
            .Replace("%healthpercent%", Math.Floor(target.Health / target.MaxHealth * 100).ToString())
            .Replace("%health%", Math.Floor(target.Health).ToString())
            .Replace("%generators%", Generator.List.Count(gen => gen.IsEngaged).ToString())
            .Replace("%engaging%", Generator.List.Count(gen => gen.IsActivating) > 0 ? $" (+{Generator.List.Count(gen => gen.IsActivating)})" : "")
            .Replace("%distance%", target != observer ? Math.Floor(Vector3.Distance(observer.Role.Type != RoleTypeId.Scp079 ? observer.Position : observer.Role.As<Scp079Role>().CameraPosition, target.Position)) + "m" : "")
            .Replace("%zombies%", Player.List.Count(p => p.Role.Type == RoleTypeId.Scp0492).ToString())
            .Replace("%079level%", scp079Level)
            .Replace("%079energy%", scp079Energy)
            .Replace("%079energypercent%", scp079EnergyPercent)
            .Replace("%079experience%", target.Role.Is(out Scp079Role experienceRole)
                ? Math.Floor((double)experienceRole.Experience).ToString(CultureInfo.InvariantCulture)
                : "")
            .Replace("%106vigor%", target.Role.Is(out Scp106Role scp106) ? Math.Floor(scp106.Vigor * 100).ToString() : "")
            .Replace("%096state%", target.Role.Is(out Scp096Role scp096)
                ? GetConfiguredValue(ScpsInfoDisplay.Singleton.Config.Scp096StateIndicator, scp096.RageState.ToString())
                : "")
            .Replace("%096targets%", target.Role.Is(out Scp096Role scp096Targets)
                ? scp096Targets.Targets.Count.ToString()
                : "")
            .Replace("%173stared%", target.Role.Is(out Scp173Role scp173)
                ? GetConfiguredValue(
                    ScpsInfoDisplay.Singleton.Config.Scp173ObservationIndicators,
                    scp173.IsObserved ? "Observed" : "Unobserved")
                : "")
            .Replace("%3114disguisetype%", target.Role.Is(out Scp3114Role scp3114)
                ? GetConfiguredValue(
                    ScpsInfoDisplay.Singleton.Config.Scp3114DisguiseDisplay,
                    scp3114.StolenRole.ToString())
                : "")
            .Replace("%roleColor%", target.Role.Color.ToHex());
        }

        private static string GetConfiguredValue(Dictionary<string, string> values, string key) =>
            values.TryGetValue(key, out string value) ? value : key;
    }
}