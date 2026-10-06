using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;

namespace Pentagoland.Utilities
{
    public static class PentagolandCommandGuard
    {
        public static bool IsAdministrator(Player player, PentagolandConfig config = null)
        {
            if (player == null)
                return false;

            if (player.IsStaff)
                return true;

            var effectiveConfig = config ?? new PentagolandConfig();
            var userId = GetUserId(player);
            if (!string.IsNullOrWhiteSpace(userId) && effectiveConfig.AdminUserIds != null)
            {
                if (effectiveConfig.AdminUserIds.Any(adminId => string.Equals(adminId, userId, StringComparison.OrdinalIgnoreCase)))
                    return true;
            }

            var nick = player.Nickname ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(nick) && effectiveConfig.AdminUserIds != null)
            {
                if (effectiveConfig.AdminUserIds.Any(adminId => string.Equals(adminId, nick, StringComparison.OrdinalIgnoreCase)))
                    return true;
            }

            return false;
        }

        public static bool CanUseCommand(string command, Player player, PentagolandConfig config = null, bool adminOnly = false)
        {
            if (string.IsNullOrWhiteSpace(command) || player == null)
                return false;

            if (!adminOnly)
                return true;

            return IsAdministrator(player, config);
        }

        public static bool HasPermission(Player player, PentagolandConfig config = null, params string[] commandNames)
        {
            if (player == null)
                return false;

            if (commandNames == null || commandNames.Length == 0)
                return IsAdministrator(player, config);

            return IsAdministrator(player, config);
        }

        public static string NormalizeCommand(string value, string prefix = "!")
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var trimmed = value.Trim();
            if (string.IsNullOrWhiteSpace(prefix))
                prefix = "!";

            if (trimmed.StartsWith(prefix, StringComparison.Ordinal))
                return trimmed;

            return string.Concat(prefix, trimmed.TrimStart('!'));
        }

        public static string ExtractCommandName(string value, string prefix = "!")
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var normalized = NormalizeCommand(value, prefix);
            var segments = normalized.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            return segments.Length == 0 ? normalized : segments[0];
        }

        private static string GetUserId(Player player)
        {
            return !string.IsNullOrWhiteSpace(player.UserId) ? player.UserId : (player.Nickname ?? string.Empty);
        }
    }

    public static class PentagolandAdminAccess
    {
        public static bool IsAdmin(Player player, PentagolandConfig config = null)
        {
            return PentagolandCommandGuard.IsAdministrator(player, config);
        }
    }
}
