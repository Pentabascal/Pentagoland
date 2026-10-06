using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using MEC;
using System;
using System.Collections.Generic;
using PlayerRoles;
using System.Text.RegularExpressions;
using System.Linq;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class SpawnRagdoll : ICommand, IUsageProvider
    {
        public string Command { get; } = "spawnragdoll";

        public string[] Aliases { get; } = new string[] { "ragdoll", "rd", "rag", "doll" };

        public string Description { get; } = "Genera un número determinado de cadáveres junto a un jugador";

        public string[] Usage { get; } = new string[] { "%player%", "%role%", "cantidad", "\"nombre\"", "\"causa de la muerte\"" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string[] quotedArgs = Regex.Matches(string.Join(" ", arguments), "[^\\s\"\']+|\"([^\"]*)\"|\'([^\']*)\'")
                .Cast<Match>()
                .Select(m => m.Value)
                .ToArray()
                .ToArray();

            if (!sender.CheckPermission("at.dolls"))
            {
                response = "No tienes permiso para usar este comando";
                return false;
            }

            if (quotedArgs.Length != 5 && quotedArgs.Length != 3)
            {
                response = "Uso: spawnragdoll ((ID / nombre del jugador) o (all / *)) (RoleTypeId) (cantidad) (nombre del cadáver) (causa de la muerte)";
                return false;
            }

            if (!Enum.TryParse(arguments.At(1), true, out RoleTypeId type))
            {
                response = $"RoleTypeId no válido para el cadáver: {arguments.At(1)}";
                return false;
            }

            if (!int.TryParse(arguments.At(2), out int amount))
            {
                response = $"Número de cadáveres no válido: {arguments.At(2)}";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"No se ha encontrado al jugador: {arguments.At(0)}";
                return false;
            }

            string ragdollName = "SCP-343";
            string deathReason = "End of the Universe";

            if (quotedArgs.Count() == 5)
            {
                ragdollName = quotedArgs.ElementAt(3).Trim('\"');
                deathReason = quotedArgs.ElementAt(4).Trim('\"');
            }    

            if(string.IsNullOrEmpty(ragdollName) || string.IsNullOrEmpty(deathReason))
            {
                ragdollName = "SCP-343";
                deathReason = "End of the universe";
            }

            foreach (Player player in players)
            {
                Timing.RunCoroutine(SpawnDolls(player, type, amount, ragdollName, deathReason));
            }

            response = $"Se han generado {amount} cadáver(es) del rol {type} junto a {arguments.At(0)}.";
            return true;
        }

        private IEnumerator<float> SpawnDolls(Player player, RoleTypeId type, int amount, string ragdollName, string deathReason)
        {
            for (int i = 0; i < amount; i++)
            {
                if (player.IsAlive)
                    Ragdoll.CreateAndSpawn(type, ragdollName, deathReason, player.Position, default);
                yield return Timing.WaitForSeconds(0.5f);
            }
        }
    }
}
