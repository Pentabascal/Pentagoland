using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;
using Exiled.API.Enums;
using System.Collections.Generic;
using Exiled.API.Features.Pickups.Projectiles;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Grenade : ICommand, IUsageProvider
    {
        public string Command { get; } = "grenade";

        public string[] Aliases { get; } = new string[] { "gn" };

        public string Description { get; } = $"Genera una granada {string.Join("/", Enum.GetNames(typeof(ProjectileType)))} para uno o varios jugadores";

        public string[] Usage { get; } = new string[] { "%player%", string.Join(", ", Enum.GetNames(typeof(ProjectileType))), "FuseTime" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.grenade"))
            {
                response = "No tienes permiso para usar este comando";
                return false;
            }

            if (arguments.Count < 2 || arguments.Count > 3)
            {
                response = "Uso: grenade ((ID / nombre del jugador) o (all / *)) (ProjectileType) [tiempo de la granada = predeterminado]";
                return false;
            }

            if (!Enum.TryParse(arguments.At(1), true, out ProjectileType type))
            {
                response = $"Tipo de proyectil no válido: {arguments.At(1)}\n{string.Join(", ",Enum.GetNames(typeof(ProjectileType)))}.";
                return false;
            }

            float? fusetime = null;
            if (float.TryParse(arguments.At(2), out float value))
                fusetime = value;

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"No se ha encontrado al jugador: {arguments.At(0)}";
                return false;
            }

            foreach (Player player in players)
            {
                if (player.IsDead)
                    continue;
                if (Projectile.CreateAndSpawn(type, player.Position, player.Rotation).Is(out TimeGrenadeProjectile timeGrenadeProjectile) && fusetime.HasValue)
                    fusetime = timeGrenadeProjectile.FuseTime = fusetime.Value;
            }

            response = $"Se ha enviado una granada ({type}) a los siguientes jugadores:{(fusetime.HasValue ? $" La granada explotará en {fusetime} segundos." : string.Empty)}\n{Extensions.LogPlayers(players)}";
            return true;
        }
    }
}
