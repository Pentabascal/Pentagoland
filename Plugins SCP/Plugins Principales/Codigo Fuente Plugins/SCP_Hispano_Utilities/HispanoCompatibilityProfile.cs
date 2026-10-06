using System;
using System.Collections.Generic;
using System.Linq;

namespace Pentagoland.Utilities
{
    /// <summary>
    /// Perfil de compatibilidad para evitar conflictos con otros plugins EXILED.
    /// Mantiene comandos, prefijos y alias bajo control para un servidor estable.
    /// </summary>
    public static class PentagolandCompatibilityProfile
    {
        public const string CommandPrefix = "!";

        public static readonly HashSet<string> ReservedCommands = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "!help",
            "!ayuda",
            "!info",
            "!report",
            "!reporte",
            "!denuncia",
            "!puntos",
            "!points",
            "!score",
            "!rules",
            "!reglas",
            "!staff",
            "!moderacion",
            "!stats",
            "!estadisticas",
            "!warns",
            "!advertencias",
            "!server",
            "!status",
            "!admin",
            "!admins",
            "!snapshot",
            "!estado",
            "!kick",
            "!ban",
            "!mute",
            "!warn",
            "!clearreportes",
            "!resetstats"
        };

        public static readonly Dictionary<string, string[]> Aliases = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["!help"] = new[] { "!ayuda", "!h", "!helpme" },
            ["!info"] = new[] { "!i", "!status", "!server" },
            ["!report"] = new[] { "!reporte", "!denuncia" },
            ["!puntos"] = new[] { "!points", "!score" },
            ["!rules"] = new[] { "!reglas", "!normas" },
            ["!staff"] = new[] { "!moderacion", "!admins" },
            ["!server"] = new[] { "!estado", "!serverstatus" },
            ["!stats"] = new[] { "!estadisticas", "!leaderboard" },
            ["!admin"] = new[] { "!admins", "!staffpanel" }
        };

        public static bool IsReservedCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                return false;

            var normalized = command.Trim();
            var firstArgument = normalized.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            return ReservedCommands.Contains(normalized)
                || (normalized.StartsWith(CommandPrefix, StringComparison.Ordinal) && ReservedCommands.Contains(firstArgument));
        }

        public static bool IsCommandAlias(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                return false;

            var normalized = command.Trim();
            foreach (var aliasGroup in Aliases)
            {
                if (aliasGroup.Value.Any(alias => string.Equals(alias, normalized, StringComparison.OrdinalIgnoreCase)))
                    return true;
            }

            return false;
        }

        public static string Normalize(string command, string prefix = CommandPrefix)
        {
            if (string.IsNullOrWhiteSpace(command))
                return string.Empty;

            var trimmed = command.Trim();
            if (string.IsNullOrWhiteSpace(prefix))
                prefix = CommandPrefix;

            if (trimmed.StartsWith(prefix, StringComparison.Ordinal))
                return trimmed;

            var safe = trimmed.TrimStart('!');
            return string.Concat(prefix, safe);
        }

        public static bool IsKnownCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                return false;

            var normalized = Normalize(command, CommandPrefix);
            return ReservedCommands.Contains(normalized)
                || Aliases.Values.Any(aliasGroup => aliasGroup.Any(alias => string.Equals(alias, normalized, StringComparison.OrdinalIgnoreCase)));
        }
    }

    public static class HispanoCompatibilityProfile
    {
        public static bool IsReservedCommand(string command) => PentagolandCompatibilityProfile.IsReservedCommand(command);

        public static bool IsCommandAlias(string command) => PentagolandCompatibilityProfile.IsCommandAlias(command);
    }
}
