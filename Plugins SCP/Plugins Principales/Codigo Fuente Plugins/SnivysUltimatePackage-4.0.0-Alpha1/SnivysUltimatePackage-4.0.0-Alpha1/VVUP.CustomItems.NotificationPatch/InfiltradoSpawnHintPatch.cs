using System;
using HarmonyLib;
using Exiled.API.Features;

namespace VVUP.CustomItems.NotificationPatch
{
    internal static class InfiltradoSpawnHintPatch
    {
        private const string IntroMarker = "<color=#5df><b>INFILTRADO</b></color>";
        private const string IntroDescription = "Tu objetivo es pasar desapercibido entre el bando contrario.";

        public static void Apply(Harmony harmony)
        {
            Type type = AccessTools.TypeByName("SubclassFramework.Core.SfUtils");
            if (type == null)
            {
                Log.Debug("SubclassFramework was not loaded; the optional Infiltrado duplicate-hint filter was skipped.");
                return;
            }

            var original = AccessTools.Method(type, "Hint", new[] { typeof(Player), typeof(string), typeof(float) });
            if (original is null)
            {
                Log.Error("SubclassFramework.Core.SfUtils.Hint(Player, string, float) was not found; the Infiltrado duplicate-hint filter was not applied.");
                return;
            }

            var prefix = AccessTools.Method(typeof(InfiltradoSpawnHintPatch), nameof(Prefix));
            if (prefix is null)
                throw new MissingMethodException(typeof(InfiltradoSpawnHintPatch).FullName, nameof(Prefix));

            harmony.Patch(original, prefix: new HarmonyMethod(prefix));
            Log.Info("The duplicate Infiltrado spawn hint is suppressed; the SubclassFramework banner remains enabled.");
        }

        private static bool Prefix(string __1)
        {
            return __1 is null ||
                   __1.IndexOf(IntroMarker, StringComparison.Ordinal) < 0 ||
                   __1.IndexOf(IntroDescription, StringComparison.Ordinal) < 0;
        }
    }
}
