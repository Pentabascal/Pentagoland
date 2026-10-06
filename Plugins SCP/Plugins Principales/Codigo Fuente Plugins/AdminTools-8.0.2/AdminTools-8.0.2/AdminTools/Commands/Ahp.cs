using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Ahp : ICommand, IUsageProvider
    {
        public string Command { get; } = "ahp";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Establece la vida artificial de uno o varios jugadores en el valor indicado";

        public string[] Usage { get; } = new string[] { "%player%", "Valor", "[decay = 1.2]", "[efficacy = 0.7]", "[sustain = 0]", "[IsPersistant = false]" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
                return false;

            if (arguments.Count < 2)
            {
                response = "Uso: ahp ((ID / nombre del jugador) o (all / *)) (valor) [decay = 1.2] [efficacy = 0.7] [sustain = 0] [persistant = false]";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"No se ha encontrado al jugador: {arguments.At(0)}";
                return false;
            }

            if (!float.TryParse(arguments.At(1), out float value))
            {
                response = $"Valor no válido para AHP: {value}";
                return false;
            }

            if (!float.TryParse(arguments.ElementAtOrDefault(3), out float decay))
                decay = 1.2f;

            if (!float.TryParse(arguments.ElementAtOrDefault(4), out float efficacy))
                efficacy = 0.7f;

            float.TryParse(arguments.ElementAtOrDefault(5), out float sustain);

            bool.TryParse(arguments.ElementAtOrDefault(6), out bool persistant);

            foreach (Player p in players)
            {
                p.AddAhp(value, value, decay, efficacy, sustain, persistant);
            }
            response = $"Se ha establecido AHP en {value} para los siguientes jugadores:\n{Extensions.LogPlayers(players)}";
            return true;
        }
    }
}
