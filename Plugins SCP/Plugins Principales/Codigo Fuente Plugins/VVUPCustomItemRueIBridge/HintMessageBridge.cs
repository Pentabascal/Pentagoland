using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Exiled.API.Features;
using HarmonyLib;
using MEC;
using RueI.API;
using RueI.API.Elements;
using RueI.API.Elements.Enums;

namespace VVUPCustomItemRueIBridge
{
    internal enum HintPlacement
    {
        Default,
        CustomItem,
        CustomItemFeedback,
        EffectDisplay,
        Subclass,
        ProximityChat,
        RagdollCleanup
    }

    [HarmonyPatch]
    internal static class NativeHintPatch
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            return typeof(Player).GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(method =>
                {
                    if (method.Name != nameof(Player.ShowHint))
                        return false;

                    ParameterInfo[] parameters = method.GetParameters();
                    if (parameters.Length == 1 && parameters[0].ParameterType == typeof(Hint))
                        return true;

                    return parameters.Length > 0 &&
                           parameters[0].ParameterType == typeof(string) &&
                           parameters.Any(parameter => parameter.ParameterType == typeof(float)) &&
                           !parameters.Any(parameter => parameter.ParameterType.IsArray);
                });
        }

        private static bool Prefix(Player __instance, object[] __args, MethodBase __originalMethod)
        {
            if (__instance == null || !__instance.IsConnected || __args.Length == 0)
                return true;

            if (!HintMessageBridge.TryGetCallingChannel(out string channel, out HintPlacement placement))
                return true;

            return RouteHint(__instance, channel, placement, __args, __originalMethod);
        }

        internal static bool RouteHint(
            Player player,
            string channel,
            HintPlacement placement,
            object[] args,
            MethodBase originalMethod)
        {
            if (args[0] is Hint hint)
            {
                if (hint.Show)
                {
                    if (float.IsNaN(hint.Duration) || hint.Duration < 0f)
                        return true;

                    HintMessageBridge.ShowPluginHint(player, channel, placement, hint.Content, hint.Duration);
                }
                else
                    HintMessageBridge.ClearPluginHint(player, channel, placement);

                return false;
            }

            string content = args[0] as string ?? string.Empty;
            ParameterInfo[] parameters = originalMethod.GetParameters();
            int durationIndex = Array.FindLastIndex(parameters, parameter => parameter.ParameterType == typeof(float));

            if (durationIndex < 0 || durationIndex >= args.Length || !(args[durationIndex] is float duration))
                return true;

            if (float.IsNaN(duration) || duration < 0f)
                return true;

            HintMessageBridge.ShowPluginHint(player, channel, placement, content, duration);
            return false;
        }
    }

    [HarmonyPatch]
    internal static class NativeBroadcastPatch
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            return typeof(Player).GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(method =>
                {
                    if (method.Name != nameof(Player.Broadcast))
                        return false;

                    ParameterInfo[] parameters = method.GetParameters();
                    return parameters.Any(parameter => parameter.ParameterType == typeof(string)) &&
                           parameters.Any(parameter =>
                               parameter.ParameterType == typeof(float) ||
                               parameter.ParameterType == typeof(ushort) ||
                               parameter.ParameterType == typeof(uint) ||
                               parameter.ParameterType == typeof(int)) &&
                           !parameters.Any(parameter => parameter.ParameterType.IsArray);
                });
        }

        private static bool Prefix(Player __instance, object[] __args, MethodBase __originalMethod)
        {
            if (__instance == null ||
                !__instance.IsConnected ||
                __args.Length == 0 ||
                !HintMessageBridge.TryGetCallingChannel(out string channel, out HintPlacement placement) ||
                placement != HintPlacement.CustomItemFeedback)
                return true;

            ParameterInfo[] parameters = __originalMethod.GetParameters();
            int contentIndex = Array.FindIndex(parameters, parameter => parameter.ParameterType == typeof(string));
            int durationIndex = Array.FindIndex(parameters, parameter =>
                parameter.ParameterType == typeof(float) ||
                parameter.ParameterType == typeof(ushort) ||
                parameter.ParameterType == typeof(uint) ||
                parameter.ParameterType == typeof(int));

            if (contentIndex < 0 ||
                contentIndex >= __args.Length ||
                durationIndex < 0 ||
                durationIndex >= __args.Length ||
                __args[durationIndex] == null)
            {
                Log.Error($"VVUP Custom Item RueI Bridge: Could not read custom-item broadcast arguments for '{__originalMethod}'. Keeping the original message.");
                return true;
            }

            float duration = __args[durationIndex] switch
            {
                float value => value,
                ushort value => value,
                uint value => value,
                int value => value,
                _ => float.NaN
            };

            if (float.IsNaN(duration) || duration < 0f)
                return true;

            string content = __args[contentIndex] as string ?? string.Empty;
            HintMessageBridge.ShowPluginHint(__instance, channel, placement, content, duration);
            return false;
        }
    }

    [HarmonyPatch]
    internal static class LabApiHintPatch
    {
        private static readonly MethodInfo GetExiledPlayerByUserId =
            typeof(Player).GetMethod("Get", new[] { typeof(string) });

        private static IEnumerable<MethodBase> TargetMethods()
        {
            Type playerType = AccessTools.TypeByName("LabApi.Features.Wrappers.Player");
            if (playerType == null)
            {
                Log.Warn("VVUP Custom Item RueI Bridge: LabAPI Player type was not found; LabAPI hints will not be routed through RueI.");
                return Enumerable.Empty<MethodBase>();
            }

            return playerType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(method =>
                {
                    if (method.Name != "SendHint")
                        return false;

                    ParameterInfo[] parameters = method.GetParameters();
                    return parameters.Length > 1 &&
                           parameters[0].ParameterType == typeof(string) &&
                           parameters.Any(parameter => parameter.ParameterType == typeof(float)) &&
                           !parameters.Any(parameter => parameter.ParameterType.IsArray);
                });
        }

        private static bool Prefix(object __instance, object[] __args, MethodBase __originalMethod)
        {
            if (__instance == null || __args.Length == 0)
                return true;

            if (!HintMessageBridge.TryGetCallingChannel(out string channel, out HintPlacement placement))
                return true;

            string userId = AccessTools.Property(__instance.GetType(), "UserId")?.GetValue(__instance) as string;
            if (string.IsNullOrWhiteSpace(userId))
            {
                Log.Error("VVUP Custom Item RueI Bridge: Could not resolve the LabAPI player for a hint; keeping the original message.");
                return true;
            }

            if (GetExiledPlayerByUserId == null)
            {
                Log.Error("VVUP Custom Item RueI Bridge: EXILED Player.Get(string) was not found; keeping the original LabAPI hint.");
                return true;
            }

            Player player = GetExiledPlayerByUserId.Invoke(null, new object[] { userId }) as Player;
            if (player == null || !player.IsConnected)
                return true;

            return NativeHintPatch.RouteHint(player, channel, placement, __args, __originalMethod);
        }
    }

    [HarmonyPatch]
    internal static class NativeHintDisplayPatch
    {
        private static readonly ConcurrentDictionary<Type, byte> UnreadableParameterTypes =
            new ConcurrentDictionary<Type, byte>();
        private static readonly MethodInfo GetExiledPlayerByNetworkId =
            typeof(Player).GetMethod("Get", new[] { typeof(uint) });

        private static IEnumerable<MethodBase> TargetMethods()
        {
            Type hintDisplayType = AccessTools.TypeByName("Hints.HintDisplay");
            Type hintType = AccessTools.TypeByName("Hints.Hint");
            if (hintDisplayType == null || hintType == null)
            {
                Log.Warn("VVUP Custom Item RueI Bridge: Native hint types were not found; direct game hint calls will not be routed.");
                return Enumerable.Empty<MethodBase>();
            }

            MethodInfo showMethod = hintDisplayType.GetMethod(
                "Show",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                binder: null,
                types: new[] { hintType },
                modifiers: null);

            if (showMethod == null)
            {
                Log.Warn("VVUP Custom Item RueI Bridge: Native HintDisplay.Show(Hint) was not found; direct game hint calls will not be routed.");
                return Enumerable.Empty<MethodBase>();
            }

            return new MethodBase[] { showMethod };
        }

        private static bool Prefix(object __instance, object[] __args)
        {
            if (__instance == null || __args.Length == 0 || __args[0] == null ||
                !HintMessageBridge.TryGetCallingChannel(out string channel, out HintPlacement placement))
                return true;

            object networkIdValue = AccessTools.Property(__instance.GetType(), "netId")?.GetValue(__instance);
            if (!(networkIdValue is uint networkId))
            {
                Log.Error("VVUP Custom Item RueI Bridge: Could not resolve the network ID for a native hint; keeping the original message.");
                return true;
            }

            if (GetExiledPlayerByNetworkId == null)
            {
                Log.Error("VVUP Custom Item RueI Bridge: EXILED Player.Get(uint) was not found; keeping the original native hint.");
                return true;
            }

            Player player = GetExiledPlayerByNetworkId.Invoke(null, new object[] { networkId }) as Player;
            if (player == null || !player.IsConnected)
                return true;

            object hint = __args[0];
            PropertyInfo parametersProperty = AccessTools.Property(hint.GetType(), "Parameters");
            PropertyInfo durationProperty = AccessTools.Property(hint.GetType(), "DurationScalar");
            if (!(parametersProperty?.GetValue(hint) is IEnumerable parameters) ||
                !(durationProperty?.GetValue(hint) is float duration))
            {
                Log.Error("VVUP Custom Item RueI Bridge: Could not read native hint contents or duration; keeping the original message.");
                return true;
            }

            var content = new StringBuilder();
            foreach (object parameter in parameters)
            {
                if (parameter == null)
                    continue;

                if (!TryReadParameterText(parameter, out string text))
                {
                    LogUnreadableParameter(parameter.GetType());

                    return true;
                }

                content.Append(text);
            }

            HintMessageBridge.ShowPluginHint(player, channel, placement, content.ToString(), duration);
            return false;
        }

        private static bool TryReadParameterText(object parameter, out string text)
        {
            text = null;

            try
            {
                Type parameterType = parameter.GetType();
                object formattedValue = AccessTools.Property(parameterType, "Formatted")?.GetValue(parameter);
                if (formattedValue is string formatted && !string.IsNullOrEmpty(formatted))
                {
                    text = formatted;
                    return true;
                }

                object rawValue = AccessTools.Property(parameterType, "Value")?.GetValue(parameter);
                if (rawValue is string raw)
                {
                    text = raw;
                    return true;
                }

                if (string.Equals(parameterType.FullName, "Hints.SSKeybindHintParameter", StringComparison.Ordinal))
                {
                    object keybindValue = AccessTools.Property(parameterType, "Value")?.GetValue(parameter);
                    object formatValue = AccessTools.Property(parameterType, "Format")?.GetValue(parameter);
                    if (keybindValue is int settingId)
                    {
                        MethodInfo formatter = AccessTools.Method(
                            parameterType,
                            "GetAndFormatKeybind",
                            new[] { typeof(int), typeof(string) });
                        object formattedKeybind = formatter?.Invoke(
                            null,
                            new[] { (object)settingId, formatValue as string ?? "[{0}]" });
                        if (formattedKeybind is string keybindText && !string.IsNullOrEmpty(keybindText))
                        {
                            text = keybindText;
                            return true;
                        }
                    }

                    return false;
                }

                if (formattedValue is string emptyFormatted)
                {
                    text = emptyFormatted;
                    return true;
                }
            }
            catch (Exception exception)
            {
                LogUnreadableParameter(parameter.GetType(), exception.GetType().Name);
            }

            return false;
        }

        private static void LogUnreadableParameter(Type parameterType, string reason = null)
        {
            if (UnreadableParameterTypes.TryAdd(parameterType, 0))
            {
                string detail = string.IsNullOrEmpty(reason) ? string.Empty : $" ({reason})";
                Log.Warn($"VVUP Custom Item RueI Bridge: Could not format native hint parameter '{parameterType.FullName}'{detail}; keeping the original message. Further messages of this type will be suppressed.");
            }
        }
    }

    internal static class HintMessageBridge
    {
        private const int MessageZIndex = 10;
        private const int CustomItemMessageZIndex = 12;
        private const float DefaultVerticalPosition = 700f;
        private const float CustomItemVerticalPosition = 230f;
        private const float CustomItemFeedbackVerticalPosition = 880f;
        private const float EffectDisplayVerticalPosition = 700f;
        private const float SubclassVerticalPosition = 960f;
        private const float SubclassMessageDuration = 6f;
        private const string SubclassMessageChannel = "subclass-framework";
        private const float ProximityChatVerticalPosition = 120f;
        private const float ProximityChatDuration = 2.5f;
        private const float RagdollCleanupVerticalPosition = 150f;
        private static readonly Regex HudPositioningTagRegex = new Regex(
            "</?align(?:\\s*=[^>]*)?>|<space\\s*=[^>]*>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Tag DefaultMessageTag = new Tag("plugin_hint_overlay");
        private static readonly Tag CustomItemMessageTag = new Tag("plugin_custom_item_overlay");
        private static readonly Tag CustomItemFeedbackMessageTag = new Tag("plugin_custom_item_feedback_overlay");
        private static readonly Tag EffectDisplayMessageTag = new Tag("plugin_effect_display_overlay");
        private static readonly Tag SubclassMessageTag = new Tag("plugin_subclass_overlay");
        private static readonly Tag ProximityChatMessageTag = new Tag("plugin_proximity_chat_overlay");
        private static readonly Tag RagdollCleanupMessageTag = new Tag("plugin_ragdoll_cleanup_overlay");
        private static readonly HintPlacement[] Placements =
        {
            HintPlacement.Default,
            HintPlacement.CustomItem,
            HintPlacement.CustomItemFeedback,
            HintPlacement.EffectDisplay,
            HintPlacement.Subclass,
            HintPlacement.ProximityChat,
            HintPlacement.RagdollCleanup
        };
        private static readonly object SyncRoot = new object();
        private static readonly Dictionary<Player, List<ActiveMessage>> ActiveMessages =
            new Dictionary<Player, List<ActiveMessage>>();

        internal static void ShowPluginHint(
            Player player,
            string channel,
            HintPlacement placement,
            string content,
            float duration)
        {
            Show(player, channel, placement, content, duration);
        }

        internal static void ClearPluginHint(Player player, string channel, HintPlacement placement)
        {
            Show(player, channel, placement, string.Empty, 0f);
        }

        internal static void ShowCustomItemMessage(Player player, string content, float duration)
        {
            Show(player, "vvup-custom-item", HintPlacement.CustomItem, content, duration);
        }

        internal static void ClearCustomItemMessage(Player player)
        {
            Show(player, "vvup-custom-item", HintPlacement.CustomItem, string.Empty, 0f);
        }

        internal static void Clear(Player player)
        {
            if (player == null)
                return;

            lock (SyncRoot)
                ActiveMessages.Remove(player);

            if (player.IsConnected)
                RemoveOverlays(player);
        }

        internal static void ClearAll()
        {
            Player[] players;

            lock (SyncRoot)
            {
                players = ActiveMessages.Keys.ToArray();
                ActiveMessages.Clear();
            }

            foreach (Player player in players)
            {
                if (player.IsConnected)
                    RemoveOverlays(player);
            }
        }

        private static void Show(
            Player player,
            string channel,
            HintPlacement placement,
            string content,
            float duration)
        {
            if (player == null || !player.IsConnected)
                return;

            if (float.IsNaN(duration) || duration < 0f)
            {
                Log.Error($"VVUP Custom Item RueI Bridge: Invalid hint duration '{duration}'.");
                return;
            }

            content = content?.Trim('\r', '\n');
            if (placement == HintPlacement.Subclass)
            {
                channel = SubclassMessageChannel;
                if (!string.IsNullOrWhiteSpace(content) && duration > 0f)
                    duration = SubclassMessageDuration;
            }

            if (placement == HintPlacement.ProximityChat)
                duration = ProximityChatDuration;

            lock (SyncRoot)
            {
                if (!ActiveMessages.TryGetValue(player, out List<ActiveMessage> messages))
                {
                    messages = new List<ActiveMessage>();
                    ActiveMessages.Add(player, messages);
                }

                double now = GetTimeSeconds();
                messages.RemoveAll(message => message.ExpiresAt <= now);

                int messageIndex = messages.FindIndex(message => message.Channel == channel);
                if (string.IsNullOrEmpty(content) || duration == 0f)
                {
                    if (messageIndex >= 0)
                        messages.RemoveAt(messageIndex);
                }
                else
                {
                    double messageExpiresAt = float.IsPositiveInfinity(duration)
                        ? double.PositiveInfinity
                        : now + duration;
                    var message = new ActiveMessage(channel, placement, content, messageExpiresAt);

                    if (messageIndex >= 0)
                        messages[messageIndex] = message;
                    else
                        messages.Add(message);
                }

                if (messages.Count == 0)
                    ActiveMessages.Remove(player);
            }

            foreach (HintPlacement activePlacement in Placements)
                RefreshOverlay(player, activePlacement);
        }

        private static void RefreshOverlay(Player player, HintPlacement placement)
        {
            List<ActiveMessage> visibleMessages;
            double nextExpiration;
            double expiresAt;

            lock (SyncRoot)
            {
                if (!ActiveMessages.TryGetValue(player, out List<ActiveMessage> messages))
                {
                    RueDisplay.Get(player).Remove(GetTag(placement));
                    return;
                }

                double now = GetTimeSeconds();
                messages.RemoveAll(message => message.ExpiresAt <= now);
                visibleMessages = messages
                    .Where(message => message.Placement == placement)
                    .ToList();
                nextExpiration = visibleMessages.Count == 0
                    ? double.PositiveInfinity
                    : visibleMessages.Min(message => message.ExpiresAt);
                expiresAt = visibleMessages.Count == 0
                    ? double.NegativeInfinity
                    : visibleMessages.Max(message => message.ExpiresAt);

                if (messages.Count == 0)
                    ActiveMessages.Remove(player);
            }

            RueDisplay display = RueDisplay.Get(player);
            if (visibleMessages.Count == 0)
            {
                display.Remove(GetTag(placement));
                return;
            }

            string content = FormatContent(placement, visibleMessages.Select(message => message.Content));
            var element = new BasicElement(GetPosition(placement), content)
            {
                ZIndex = placement switch
                {
                    HintPlacement.CustomItem or HintPlacement.CustomItemFeedback => CustomItemMessageZIndex,
                    HintPlacement.RagdollCleanup => 3,
                    _ => MessageZIndex
                },
                ResolutionBasedAlign = true,
                VerticalAlign = VerticalAlign.Down,
                ShowToSpectators = placement == HintPlacement.RagdollCleanup
            };

            if (double.IsPositiveInfinity(expiresAt))
                display.Show(GetTag(placement), element);
            else
                display.Show(GetTag(placement), element, (float)Math.Max(0.01d, expiresAt - GetTimeSeconds()));

            if (!double.IsPositiveInfinity(nextExpiration) && nextExpiration < expiresAt)
            {
                float delay = (float)Math.Max(0.01d, nextExpiration - GetTimeSeconds());
                Timing.CallDelayed(delay, () =>
                {
                    if (player.IsConnected)
                        RefreshOverlay(player, placement);
                });
            }
        }

        private static string FormatContent(HintPlacement placement, IEnumerable<string> messages)
        {
            string renderedContent = string.Join("\n",
                messages.Select(StripHudPositioningTags)
                    .Where(message => !string.IsNullOrWhiteSpace(message))
                    .Distinct(StringComparer.Ordinal));
            return placement switch
            {
                HintPlacement.EffectDisplay => $"<align=left>{renderedContent}</align>",
                HintPlacement.CustomItem or HintPlacement.CustomItemFeedback => $"<align=center>{renderedContent}</align>",
                HintPlacement.Subclass => $"<align=center><size=20>{renderedContent}</size></align>",
                HintPlacement.ProximityChat => $"<align=right><size=25><b><color=#FF6666>{renderedContent}</color></b></size></align>",
                HintPlacement.RagdollCleanup => $"<align=left>{renderedContent}</align>",
                _ => $"<align=left>{renderedContent}</align>"
            };
        }

        private static string StripHudPositioningTags(string content) =>
            string.IsNullOrEmpty(content) ? string.Empty : HudPositioningTagRegex.Replace(content, string.Empty);

        private static Tag GetTag(HintPlacement placement) => placement switch
        {
            HintPlacement.CustomItem => CustomItemMessageTag,
            HintPlacement.CustomItemFeedback => CustomItemFeedbackMessageTag,
            HintPlacement.EffectDisplay => EffectDisplayMessageTag,
            HintPlacement.Subclass => SubclassMessageTag,
            HintPlacement.ProximityChat => ProximityChatMessageTag,
            HintPlacement.RagdollCleanup => RagdollCleanupMessageTag,
            _ => DefaultMessageTag
        };

        private static float GetPosition(HintPlacement placement) => placement switch
        {
            HintPlacement.CustomItem => CustomItemVerticalPosition,
            HintPlacement.CustomItemFeedback => CustomItemFeedbackVerticalPosition,
            HintPlacement.EffectDisplay => EffectDisplayVerticalPosition,
            HintPlacement.Subclass => SubclassVerticalPosition,
            HintPlacement.ProximityChat => ProximityChatVerticalPosition,
            HintPlacement.RagdollCleanup => RagdollCleanupVerticalPosition,
            _ => DefaultVerticalPosition
        };

        private static void RemoveOverlays(Player player)
        {
            RueDisplay display = RueDisplay.Get(player);
            foreach (HintPlacement placement in Placements)
                display.Remove(GetTag(placement));
        }

        private static double GetTimeSeconds() =>
            (double)Stopwatch.GetTimestamp() / Stopwatch.Frequency;

        internal static bool TryGetCallingChannel(out string channel, out HintPlacement placement)
        {
            channel = "unattributed-plugin-hint";
            placement = HintPlacement.Default;

            foreach (StackFrame frame in new StackTrace().GetFrames() ?? Array.Empty<StackFrame>())
            {
                MethodBase method = frame.GetMethod();
                Type declaringType = method?.DeclaringType;

                if (declaringType == null || declaringType.Assembly == typeof(HintMessageBridge).Assembly)
                    continue;

                string assemblyName = declaringType.Assembly.GetName().Name;
                if (assemblyName.StartsWith("Exiled", StringComparison.OrdinalIgnoreCase) ||
                    assemblyName.StartsWith("LabApi", StringComparison.OrdinalIgnoreCase) ||
                    assemblyName.StartsWith("Harmony", StringComparison.OrdinalIgnoreCase) ||
                    assemblyName.StartsWith("MonoMod", StringComparison.OrdinalIgnoreCase) ||
                    assemblyName.StartsWith("Assembly-CSharp", StringComparison.OrdinalIgnoreCase) ||
                    assemblyName.StartsWith("Mirror", StringComparison.OrdinalIgnoreCase) ||
                    assemblyName.StartsWith("NorthwoodLib", StringComparison.OrdinalIgnoreCase) ||
                    assemblyName.StartsWith("CommandSystem", StringComparison.OrdinalIgnoreCase) ||
                    assemblyName.StartsWith("MEC", StringComparison.OrdinalIgnoreCase) ||
                    assemblyName.StartsWith("System", StringComparison.OrdinalIgnoreCase) ||
                    assemblyName.StartsWith("Unity", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (string.Equals(assemblyName, "RueI", StringComparison.OrdinalIgnoreCase))
                    return false;

                if (string.Equals(assemblyName, "VVUP.CustomItems", StringComparison.OrdinalIgnoreCase))
                    placement = HintPlacement.CustomItemFeedback;
                else if (string.Equals(assemblyName, "EffectDisplay", StringComparison.OrdinalIgnoreCase))
                    placement = HintPlacement.EffectDisplay;
                else if (string.Equals(assemblyName, "SubclassFramework", StringComparison.OrdinalIgnoreCase))
                    placement = HintPlacement.Subclass;
                else if (assemblyName.StartsWith("ScpProximityChat", StringComparison.OrdinalIgnoreCase))
                    placement = HintPlacement.ProximityChat;
                else if (string.Equals(assemblyName, "SER", StringComparison.OrdinalIgnoreCase) ||
                         string.Equals(assemblyName, "SER-Exiled", StringComparison.OrdinalIgnoreCase))
                    placement = HintPlacement.RagdollCleanup;

                try
                {
                    channel = $"{declaringType.Assembly.ManifestModule.ModuleVersionId:N}:{method.MetadataToken}";
                }
                catch (InvalidOperationException)
                {
                    channel = declaringType.FullName;
                }

                return true;
            }

            return false;
        }

        private sealed class ActiveMessage
        {
            internal ActiveMessage(string channel, HintPlacement placement, string content, double expiresAt)
            {
                Channel = channel;
                Placement = placement;
                Content = content;
                ExpiresAt = expiresAt;
            }

            internal string Channel { get; }
            internal HintPlacement Placement { get; }
            internal string Content { get; }
            internal double ExpiresAt { get; }
        }
    }
}
