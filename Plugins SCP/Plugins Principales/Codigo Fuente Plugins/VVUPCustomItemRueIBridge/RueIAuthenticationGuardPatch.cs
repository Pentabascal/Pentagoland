using System;
using System.Collections.Generic;
using System.Reflection;
using Exiled.API.Features;
using HarmonyLib;

namespace VVUPCustomItemRueIBridge
{
    [HarmonyPatch]
    internal static class RueIAuthenticationGuardPatch
    {
        private static readonly HashSet<ReferenceHub> VerifiedHubs = new HashSet<ReferenceHub>();
        private static readonly HashSet<ReferenceHub> SuppressedHubs = new HashSet<ReferenceHub>();

        private static MethodBase TargetMethod()
        {
            Type elementCombiner = AccessTools.TypeByName("RueI.API.Parsing.ElementCombiner");
            if (elementCombiner == null)
                throw new TypeLoadException("RueI's ElementCombiner was not found.");

            return AccessTools.Method(elementCombiner, "Combine")
                   ?? throw new MissingMethodException(elementCombiner.FullName, "Combine");
        }

        private static bool Prefix(ReferenceHub hub)
        {
            if (hub == null)
                return false;

            if (VerifiedHubs.Contains(hub))
                return true;

            if (SuppressedHubs.Add(hub))
                Log.Warn("RueI HUD output was held until the player finishes verification.");

            return false;
        }

        internal static void MarkVerified(ReferenceHub hub)
        {
            if (hub == null)
                return;

            VerifiedHubs.Add(hub);
            SuppressedHubs.Remove(hub);
        }

        internal static void Remove(ReferenceHub hub)
        {
            if (hub == null)
                return;

            VerifiedHubs.Remove(hub);
            SuppressedHubs.Remove(hub);
        }
    }
}
