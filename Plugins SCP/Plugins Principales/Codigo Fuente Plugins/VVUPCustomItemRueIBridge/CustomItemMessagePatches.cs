using System;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using HarmonyLib;

namespace VVUPCustomItemRueIBridge
{
    [HarmonyPatch(typeof(CustomItem), "ShowPickedUpMessage")]
    internal static class PickedUpMessagePatch
    {
        private static bool Prefix(CustomItem __instance, Player player) =>
            CustomItemMessageBridge.Show(__instance, player, pickedUp: true);
    }

    [HarmonyPatch(typeof(CustomItem), "ShowSelectedMessage")]
    internal static class SelectedMessagePatch
    {
        private static bool Prefix(CustomItem __instance, Player player) =>
            CustomItemMessageBridge.Show(__instance, player, pickedUp: false);
    }

    internal static class CustomItemMessageBridge
    {
        private const string VvupCustomItemsAssemblyName = "VVUP.CustomItems";
        internal static bool Show(CustomItem item, Player player, bool pickedUp)
        {
            if (item == null ||
                player == null ||
                !string.Equals(item.GetType().Assembly.GetName().Name, VvupCustomItemsAssemblyName, StringComparison.Ordinal))
                return true;

            Exiled.CustomItems.CustomItems customItemsPlugin = Exiled.CustomItems.CustomItems.Instance;
            if (customItemsPlugin == null)
            {
                Log.Error("VVUP Custom Item RueI Bridge: Exiled.CustomItems is unavailable; keeping the original message.");
                return true;
            }

            var message = pickedUp
                ? customItemsPlugin.Config.PickedUpHint
                : customItemsPlugin.Config.SelectedHint;

            if (!message.Show)
            {
                HintMessageBridge.ClearCustomItemMessage(player);
                return false;
            }

            string content;
            try
            {
                content = string.Format(message.Content, item.Name, item.Description);
            }
            catch (FormatException exception)
            {
                Log.Error($"VVUP Custom Item RueI Bridge: Invalid CustomItems message template: {exception}");
                HintMessageBridge.ClearCustomItemMessage(player);
                return true;
            }
            catch (ArgumentNullException exception)
            {
                Log.Error($"VVUP Custom Item RueI Bridge: CustomItems message template is null: {exception}");
                HintMessageBridge.ClearCustomItemMessage(player);
                return true;
            }

            HintMessageBridge.ShowCustomItemMessage(player, content, message.Duration);

            return false;
        }
    }
}
