namespace XPSystem.Commands.Client.Subcommands
{
    using System;
    using CommandSystem;
    using XPSystem.API;
    using XPSystem.API.Player;
    using XPSystem.API.StorageProviders;

    public class GetCommandClient : SanitizedInputCommand, IAliasableCommand
    {
        public override bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!XPPlayer.TryGet(sender, out BaseXPPlayer? basePlayer) || basePlayer is not XPPlayer player)
            {
                response = "Este comando solo está disponible para jugadores.";
                return false;
            }

            PlayerInfoWrapper log = player.GetPlayerInfo();
            response = $"Nivel: {log.Level} | XP: {log.XP} | XP necesario: {log.NeededXPNext}";
            return true;
        }

        public string CommandOverride { get; set; } = "get";
        public override string Command => CommandOverride;
        public override string[] Aliases { get; } = Array.Empty<string>();
        public override string Description { get; } = "Muestra tu nivel y tus XP.";
    }
}