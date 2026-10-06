namespace SCPReplacer
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Exiled.API.Features;
    using Exiled.API.Extensions;
    using PlayerRoles;

    /// <summary>
    /// Shared helper methods used across the plugin.
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Strips everything except digits from user-typed input (e.g. "SCP-079" becomes "079").
        /// </summary>
        public static string ScpNumber(this string input)
        {
            return Regex.Replace(input, "[^0-9]", string.Empty);
        }

        /// <summary>
        /// Gets the SCP number for a given role type (e.g. RoleTypeId.Scp079 becomes "079").
        /// </summary>
        public static string ScpNumber(this RoleTypeId role)
        {
            return Regex.Replace(role.ToString(), "[^0-9]", string.Empty);
        }

        /// <summary>
        /// Builds a "SCP-XXX" label colored with that SCP's own role color.
        /// </summary>
        public static string ColoredScpLabel(this RoleTypeId role)
        {
            string colorHex = role.GetColor().ToHex();
            return $"<color={colorHex}>SCP-{role.ScpNumber()}</color>";
        }

        /// <summary>
        /// Whether a player may enter, and be picked in, the replacement lottery. Spectators can, SCPs (other than SCP-049-2) and Overwatch can't.
        /// </summary>
        public static bool CanVolunteer(this Player player)
        {
            if (!player.IsConnected || player.Role.Type == RoleTypeId.Overwatch)
                return false;

            return !player.IsScp || player.Role.Type == RoleTypeId.Scp0492;
        }

        /// <summary>
        /// Whether two SCP number strings refer to the same SCP, ignoring leading zeros (e.g. "49" and "049").
        /// </summary>
        public static bool SameScp(string first, string second)
        {
            return int.TryParse(first, out int firstNumber) && int.TryParse(second, out int secondNumber) && firstNumber == secondNumber;
        }
    }
}