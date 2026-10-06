namespace XPSystem.Commands.Client.Subcommands
{
    using System;
    using CommandSystem;
    using XPSystem.API;

    public class LeaderboardCommandClient : SanitizedInputCommand, IAliasableCommand
    {
        public override bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
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

                if (amount > XPAPI.Config.LeaderboardMaxLength)
                {
                    response = $"La cantidad no puede superar {XPAPI.Config.LeaderboardMaxLength}.";
                    return false;
                }
            }

            response = XPAPI.StorageProvider!.GetTopPlayers(amount)
                .FormatLeaderboard();
            return true;
        }

        public string CommandOverride { get; set; } = "leaderboard";
        public override string Command => CommandOverride;
        public override string[] Aliases { get; } = Array.Empty<string>();
        public override string Description { get; } = "Muestra la clasificación de jugadores.";
    }
}