using System;
using System.Collections.Generic;
using Exiled.API.Features;

namespace Pentagoland.Utilities
{
    public static class PentagolandCommandRegistry
    {
        private static readonly HashSet<string> Commands = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "!help",
            "!ayuda",
            "!rules",
            "!reglas",
            "!info",
            "!report",
            "!reporte",
            "!denuncia",
            "!puntos",
            "!points",
            "!score",
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

        private static readonly HashSet<string> AdminOnlyCommands = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
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

        public static bool IsCommand(string message, string prefix = "!")
        {
            if (string.IsNullOrWhiteSpace(message))
                return false;

            var trimmed = message.Trim();
            if (string.IsNullOrWhiteSpace(prefix))
                prefix = "!";

            if (!trimmed.StartsWith(prefix, StringComparison.Ordinal))
                return false;

            var command = trimmed.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries)[0];
            var normalized = NormalizeWithPrefix(command, prefix);
            return Commands.Contains(normalized) || Commands.Contains(command);
        }

        public static bool CanUseCommand(Player player, string message, PentagolandConfig config = null, string prefix = "!")
        {
            if (!IsCommand(message, prefix))
                return false;

            var command = GetCommandName(message, prefix);
            if (string.IsNullOrWhiteSpace(command))
                return false;

            if (!AdminOnlyCommands.Contains(command))
                return true;

            return PentagolandCommandGuard.IsAdministrator(player, config);
        }

        public static bool IsAdminOnlyCommand(string message, string prefix = "!")
        {
            var command = GetCommandName(message, prefix);
            return !string.IsNullOrWhiteSpace(command) && AdminOnlyCommands.Contains(command);
        }

        public static string GetCommandName(string message, string prefix = "!")
        {
            if (string.IsNullOrWhiteSpace(message))
                return string.Empty;

            var trimmed = message.Trim();
            if (string.IsNullOrWhiteSpace(prefix))
                prefix = "!";

            var command = trimmed.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries)[0];
            return NormalizeWithPrefix(command, prefix);
        }

        public static string NormalizeWithPrefix(string value, string prefix = "!")
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            if (string.IsNullOrWhiteSpace(prefix))
                prefix = "!";

            var trimmed = value.Trim();
            if (trimmed.StartsWith(prefix, StringComparison.Ordinal))
                return trimmed;

            var remaining = trimmed.TrimStart('!');
            return string.Concat(prefix, remaining);
        }
    }

    public static class HispanoCommandRegistry
    {
        public static bool IsCommand(string message, string prefix = "!")
        {
            return PentagolandCommandRegistry.IsCommand(message, prefix);
        }

        public static bool CanUseCommand(Player player, string message, PentagolandConfig config = null, string prefix = "!")
        {
            return PentagolandCommandRegistry.CanUseCommand(player, message, config, prefix);
        }
    }
}
