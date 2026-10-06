namespace XPSystem.Commands.Admin.Subcommands
{
    using System;
    using CommandSystem;
    using XPSystem.API;

    public class LeaderboardCommandAdmin : SanitizedInputCommand
    {
        public override bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermissionLS("xps.leaderboard"))
            {
                response = "No tienes permiso (xps.leaderboard) para usar este comando.";
                return false;
            }

            XPAPI.EnsureStorageProviderValid();

            int amount = 5;
            if (arguments.Count > 0)
            {
                if (!int.TryParse(arguments.At(0), out amount))
                {
                    response = "Cantidad no válida.";
                    return false;
                }

                if (amount < 1)
                {
                    response = "La cantidad debe ser como mínimo 1.";
                    return false;
                }
            }
            
            response = XPAPI.StorageProvider!.GetTopPlayers(amount)
                .FormatLeaderboard();
            return true;
        }

        public override string Command { get; } = "leaderboard";
        public override string Description { get; } = "Muestra la clasificación de jugadores.";
    }
}