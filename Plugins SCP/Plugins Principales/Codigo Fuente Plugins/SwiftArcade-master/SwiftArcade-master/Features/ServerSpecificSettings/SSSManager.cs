using Hints;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using SwiftArcadeMode.Commands.Client;
using System;
using UnityEngine;
using UserSettings.ServerSpecific;
using Logger = LabApi.Features.Console.Logger;

namespace SwiftArcadeMode.Features.ServerSpecificSettings
{
    public static class SSSManager
    {
        public static SSKeybindSetting Keybind_SeePerks;
        public static SSKeybindSetting Keybind_SeeUpgrades;
        public static SSKeybindSetting Keybind_ChooseUpgrade1;
        public static SSKeybindSetting Keybind_ChooseUpgrade2;
        public static SSKeybindSetting Keybind_ChooseUpgrade3;

        public static void Enable()
        {
            Keybind_SeePerks = new(20070923, "Ver ventajas", KeyCode.LeftBracket, true, true, "Muestra tus ventajas.");
            Keybind_SeeUpgrades = new(20070924, "Ver mejoras", KeyCode.RightBracket, true, false, "Muestra las mejoras disponibles.");
            Keybind_ChooseUpgrade1 = new(20070925, "Elegir mejora 1", KeyCode.Comma, true, false, "Elige la mejora 1.");
            Keybind_ChooseUpgrade2 = new(20070926, "Elegir mejora 2", KeyCode.Period, true, false, "Elige la mejora 2.");
            Keybind_ChooseUpgrade3 = new(20070927, "Elegir mejora 3", KeyCode.Slash, true, false, "Elige la mejora 3.");

            AppendSettings();
            ServerSpecificSettingsSync.ServerOnSettingValueReceived += OnPressKeybind;
            PlayerEvents.Joined += OnJoined;
        }

        private static void AppendSettings()
        {
            ServerSpecificSettingsSync.DefinedSettings = [.. ServerSpecificSettingsSync.DefinedSettings ?? [], new SSGroupHeader("Modo arcade de SwiftKraft"), Keybind_SeePerks, Keybind_SeeUpgrades, Keybind_ChooseUpgrade1, Keybind_ChooseUpgrade2, Keybind_ChooseUpgrade3];
            ServerSpecificSettingsSync.SendToAll();
        }

        private static void OnJoined(PlayerJoinedEventArgs ev)
        {
            ServerSpecificSettingsSync.SendToPlayer(ev.Player.ReferenceHub);
        }

        private static void OnPressKeybind(ReferenceHub pl, ServerSpecificSettingBase setting)
        {
            try
            {
                Player p = Player.Get(pl);

                if (p == null)
                    return;

                PerkInventory inv = p.GetPerkInventory();

                void select(int ind)
                {
                    if (inv == null)
                    {
                        p.SendHint("<align=\"left\">No hay mejoras disponibles.</align>", [HintEffectPresets.FadeOut()], 2f);
                        return;
                    }

                    if (inv.UpgradeQueue.Choose(ind, out string name))
                        p.SendHint("<align=\"left\">Elegida: " + name + "</align>", [HintEffectPresets.FadeOut()], 10f);
                    else
                        p.SendHint("<align=\"left\">No se pudo elegir la mejora.</align>", [HintEffectPresets.FadeOut()], 2f);
                }

                switch (setting)
                {
                    case SSKeybindSetting { SettingId: 20070923, SyncIsPressed: true } keyb: // see perks
                        ClientSeePerksCommand.GetPerks(p, out string brief1);
                        p.SendHint(brief1, [HintEffectPresets.FadeOut()], 5f);
                        break;
                    case SSKeybindSetting { SettingId: 20070924, SyncIsPressed: true } keyb: // see upgrades
                        if (inv == null)
                        {
                            p.SendHint("<align=\"left\">No hay mejoras disponibles.</align>", [HintEffectPresets.FadeOut()], 2f);
                            break;
                        }
                        string text = inv.UpgradeQueue.Peek(out string n);
                        p.SendHint("<align=\"left\">" + n + "</align>", [HintEffectPresets.FadeOut()], 10f);
                        break;
                    case SSKeybindSetting { SettingId: 20070925, SyncIsPressed: true } keyb: // choose 1
                        select(0);
                        break;
                    case SSKeybindSetting { SettingId: 20070926, SyncIsPressed: true } keyb: // choose 2
                        select(1);
                        break;
                    case SSKeybindSetting { SettingId: 20070927, SyncIsPressed: true } keyb: // choose 3
                        select(2);
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public static void Disable() => PlayerEvents.Joined -= OnJoined;
    }
}
