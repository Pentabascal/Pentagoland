using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features.Pickups.Projectiles;
using PlayerRoles.FirstPersonControl;
using Random = UnityEngine.Random;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Ball : ICommand, IUsageProvider
    {
        public string Command { get; } = "ball";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Genera una pelota saltarina (SCP-018) para uno o todos los jugadores";

        public string[] Usage { get; } = new string[] { "%player%", "[Speed = 5]","[IsMute = false]"};

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.GivingItems, out response))
                return false;

            if (arguments.Count < 1)
            {
                response = "Uso: ball ((ID / nombre del jugador) o (all / *)) [Speed] [IsMute]";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"No se ha encontrado al jugador: {arguments.At(0)}";
                return false;
            }

            if (!float.TryParse(arguments.ElementAtOrDefault(1), out float speed) || speed >= 200)
                speed = 5;

            if (!bool.TryParse(arguments.ElementAtOrDefault(2), out bool isMute) || !isMute)
                LabApi.Features.Wrappers.Cassie.Message("pitch_1.5 xmas_bouncyballs", "");

            foreach (Player p in players)
            {
                Scp018Projectile scp018 = Projectile.CreateAndSpawn(ProjectileType.Scp018, p.Position, p.Transform.rotation).As<Scp018Projectile>();
                scp018.Rigidbody.velocity = p.ReferenceHub.GetVelocity() + Random.onUnitSphere * speed;
            }

            response = $"Se ha generado una pelota para los siguientes jugadores:\n{Extensions.LogPlayers(players)}";
            return true;
        }
    }
}
