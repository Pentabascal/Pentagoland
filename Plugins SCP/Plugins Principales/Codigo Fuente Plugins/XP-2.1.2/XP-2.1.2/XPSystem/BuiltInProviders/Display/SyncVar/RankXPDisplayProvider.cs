namespace XPSystem.XPDisplayProviders
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using HarmonyLib;
    using MEC;
    using XPSystem.API;
    using XPSystem.API.DisplayProviders;
    using XPSystem.API.Player;
    using XPSystem.API.StorageProviders;
    using YamlDotNet.Serialization;

    public class RankXPDisplayProvider : SyncVarXPDisplayProvider<RankXPDisplayProvider.RankConfig, Badge>
    {
        protected override string VariableKey { get; } = "RankXPDisplayProvider_badge";

        protected override (Type typeName, string methodName, Func<BaseXPPlayer, Badge, object?> getFakeSyncVar, Func<BaseXPPlayer, object?> getResyncVar)[] SyncVars { get; } =
        {
            (typeof(ServerRoles), nameof(ServerRoles.Network_myText), (_, obj) => obj.Text, player => player.BadgeText),
            (typeof(ServerRoles), nameof(ServerRoles.Network_myColor), (_, obj) => obj.Color, player => player.BadgeColor)
        };

        protected override Badge? CreateObject(BaseXPPlayer player, PlayerInfoWrapper? playerInfo)
        {
            if (player.DNT)
            {
                if (player is { HasBadge: true, HasHiddenBadge: false })
                    return null;

                return Config.DNTBadge;
            }

            if (player is not XPPlayer xpPlayer)
                return null;
            playerInfo ??= xpPlayer.GetPlayerInfo();

            Badge? badge = null;
            string format = !player.HasBadge || player.HasHiddenBadge
                ? Config.BadgeStructureNoBadge
                : Config.BadgeStructure;
            string? color = null;

            foreach (var kvp in Config.SortedBadges)
            {
                if (kvp.Key > playerInfo.Level)
                    break;

                badge = kvp.Value;
                color = kvp.Value.Color;
            }

            if (badge == null)
                return null;

            if (player.HasBadge && !Config.OverrideColor)
                color = player.BadgeColor;

            return new Badge()
            {
                Text = format
                    .Replace("%lvl%", playerInfo.Level.ToString())
                    .Replace("%badge%", badge.Text)
                    .Replace("%oldbadge%", player.BadgeText),
                Color = color ?? "default"
            };
        }

        protected override bool ShouldEdit(BaseXPPlayer player)
        {
            if (Config.SkipGlobalBadges && player.HasGlobalBadge)
                return false;

            if (!Config.EditBadgeHiding && player.HasHiddenBadge)
                return false;

            return true;
        }

        protected override bool ShouldShowTo(BaseXPPlayer player, BaseXPPlayer target)
        {
            if (!base.ShouldShowTo(player, target))
                return false;
            if (!Config.EditBadgeHiding)
                return true;

            return !(player.HasHiddenBadge && (player == target || target.CanViewHiddenBadge));
        }

        public override void Enable()
        {
            base.Enable();

            Config.DNTBadge.ValidateColor();
            foreach (var kvp in Config.Badges)
                kvp.Value.ValidateColor();
        }

        internal static class HiddenBadgePatch
        {
            [HarmonyPatch(typeof(ServerRoles), nameof(ServerRoles.RefreshLocalTag))]
            public class HiddenBadgePatchShow
            {
                public static void Postfix(ServerRoles __instance) => Refresh(__instance);
            }

            [HarmonyPatch(typeof(ServerRoles), nameof(ServerRoles.TryHideTag))]
            public class HiddenBadgePatchHide
            {
                public static void Postfix(ServerRoles __instance) => Refresh(__instance);
            }

            private static void Refresh(ServerRoles instance)
            {
                foreach (IXPDisplayProvider provider in XPAPI.DisplayProviders)
                {
                    if (provider is RankXPDisplayProvider rankProvider && rankProvider.Config.Enabled && rankProvider.Config.PatchBadgeCommands)
                    {
                        Timing.CallDelayed(.5f + XPAPI.Config.ExtraDelay, () =>
                        {
                            rankProvider.RefreshOf(instance._hub);
                        });

                        return;
                    }
                }
            }
        }

        public class RankConfig : IXPDisplayProviderConfig
        {
            [Description("¿Activar la modificación de insignias?")]
            public bool Enabled { get; set; } = true;

            [Description("Formato de la insignia que se muestra en el juego. Variables: %lvl%: nivel; %badge%: insignia obtenida, definida en LevelsBadge; %oldbadge%: insignia original del juego (por ejemplo, la definida en config-remoteadmin) o insignia global. Puede ser null.")]
            public string BadgeStructure { get; set; } = "%badge% | %oldbadge%";

            [Description("Consulta la descripción anterior; se aplica a quienes no tienen insignia. Si se deja vacío, se utilizará BadgeStructure.")]
            public string BadgeStructureNoBadge { get; set; } = "%badge%";

            [Description("¿Sustituir el color si el usuario ya tiene una insignia?")]
            public bool OverrideColor { get; set; } = false;

            [Description("Necesario para cumplir con VSR.")]
            public bool SkipGlobalBadges { get; set; } = true;

            [Description("Indica si se cambiará el comportamiento al ocultar insignias.")]
            public bool EditBadgeHiding { get; set; } = true;

            [Description("Indica si se modificarán los comandos para mostrar u ocultar insignias, de modo que la insignia se actualice automáticamente.")]
            public bool PatchBadgeCommands { get; set; } = true;

            [Description("Insignia para los jugadores con DNT.")]
            public Badge DNTBadge { get; set; } = new()
            {
                Text = "(DNT) Anónimo",
                Color = nameof(Misc.PlayerInfoColorTypes.Nickel)
            };

            private Dictionary<int, Badge> _sortedBadges = null!;
            [YamlIgnore]
            public Dictionary<int, Badge> SortedBadges => _sortedBadges ??= Badges
                .OrderBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Value);

            [Description("(Puedes añadir tus propias entradas). Define un nivel mínimo y una insignia. Si aparece TAG FAIL en la consola, cambia el color o elimina caracteres especiales, como los corchetes.")]
            public Dictionary<int, Badge> Badges { get; set; } = new()
            {
                [0] = new Badge
                {
                    Text = "Visitante",
                    Color = nameof(Misc.PlayerInfoColorTypes.Cyan).ToLower()
                },
                [1] = new Badge
                {
                    Text = "Novato",
                    Color = nameof(Misc.PlayerInfoColorTypes.Orange).ToLower()
                },
                [5] = new Badge
                {
                    Text = "Éliteo",
                    Color = nameof(Misc.PlayerInfoColorTypes.Yellow).ToLower()
                },
                [10] = new Badge
                {
                    Text = "Élite",
                    Color = nameof(Misc.PlayerInfoColorTypes.Red).ToLower()
                },
                [50] = new Badge
                {
                    Text = "Friki",
                    Color = nameof(Misc.PlayerInfoColorTypes.Lime).ToLower()
                }
            };
        }
    }
}