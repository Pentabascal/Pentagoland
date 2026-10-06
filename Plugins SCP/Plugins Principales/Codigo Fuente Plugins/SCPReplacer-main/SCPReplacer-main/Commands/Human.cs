namespace SCPReplacer.Commands
{
    using System;
    using System.Text.RegularExpressions;
    using CommandSystem;
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.CustomRoles.API;
    using Exiled.CustomRoles.API.Features;
    using PlayerRoles;
    using SCPReplacer.Models;

    /// <summary>
    /// The .human command, letting an SCP voluntarily give up their role early for a random human class.
    /// </summary>
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Human : ICommand
    {
        /// <inheritdoc/>
        public string Command => "human";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "no" };

        /// <inheritdoc/>
        public string Description => "Renuncia a tu rol de SCP y conviértete en una clase humana aleatoria.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Config config = Plugin.Instance!.Config;
            Translation translation = Plugin.Instance!.Translation;

            if (!config.HumanForfeitEnabled)
            {
                response = translation.HumanDisabled;
                return false;
            }

            if (Player.Get(sender) is not Player player)
            {
                response = translation.NotAPlayer;
                return false;
            }

            if (!player.IsScp || player.Role == RoleTypeId.Scp0492)
            {
                response = translation.HumanNotScp;
                return false;
            }

            if (Round.ElapsedTime.TotalSeconds > config.QuitCutoffSeconds)
            {
                response = translation.HumanTooLate;
                return false;
            }

            double requiredHealth = config.RequiredHealthPercent / 100.0 * player.MaxHealth;
            if (player.Health < requiredHealth)
            {
                response = translation.HumanLowHealth;
                return false;
            }

            RoleTypeId newRole = UnityEngine.Random.value switch
            {
                < 0.45f => RoleTypeId.ClassD,
                < 0.9f => RoleTypeId.Scientist,
                _ => RoleTypeId.FacilityGuard,
            };

            RoleTypeId formerRole = player.Role.Type;

            foreach (CustomRole customRole in player.GetCustomRoles())
                customRole.RemoveRole(player);

            player.DisableAllEffects();
            player.Role.Set(newRole, SpawnReason.LateJoin, RoleSpawnFlags.All);

            ScpToReplace.Open(formerRole, player.UserId);

            if (config.Debug)
                Log.Debug($"{player.Nickname} forfeited {formerRole} and became {newRole}.");

            string roleColorHex = newRole.GetColor().ToHex();
            string roleName = Regex.Replace(newRole.ToString(), "(?<=[a-z])(?=[A-Z])", " ");
            string coloredRoleName = $"<color={roleColorHex}>{roleName}</color>";
            response = string.Format(translation.HumanForfeitConfirmed, coloredRoleName);

            player.Broadcast(new Broadcast(translation.BroadcastHeader + response, 5));

            return true;
        }
    }
}