using PlayerRoles;
using SwiftArcadeMode.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SwiftArcadeMode.Features.SCPs.Upgrades
{
    public static class UpgradePathManager
    {
        public static readonly HashSet<UpgradePathAttribute> RegisteredUpgrades = [];

        public static void Enable()
        {
            if (Core.Instance.Config.AllowBaseContent)
                RegisterUpgrades();
        }

        public static void Disable() { }

        public static void RegisterUpgrades()
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();

            Dictionary<Type, UpgradePathAttribute> atts = callingAssembly
                .GetTypes()
                .Select(t => (type: t, attr: t.GetCustomAttribute<UpgradePathAttribute>()))
                .Where(pair => pair.attr != null)
                .ToDictionary(pair => pair.type, pair => pair.attr);

            foreach (KeyValuePair<Type, UpgradePathAttribute> attr in atts)
            {
                attr.Value.Perk = PerkManager.GetPerk(attr.Key);
                if (!RegisteredUpgrades.Contains(attr.Value))
                    RegisteredUpgrades.Add(attr.Value);
            }
        }

        public static UpgradePathAttribute GetRandomUpgradePath(this RoleTypeId role) => RegisteredUpgrades.Where((t) => t.Roles.Contains(role)).ToArray().GetWeightedRandom();
        public static UpgradePathAttribute GetRandomUpgradePath(this RoleTypeId role, ICollection<UpgradePathAttribute> noRep) => RegisteredUpgrades.Where((t) => t.Roles.Contains(role) && !noRep.Contains(t)).ToArray().GetWeightedRandom();
    }
}
