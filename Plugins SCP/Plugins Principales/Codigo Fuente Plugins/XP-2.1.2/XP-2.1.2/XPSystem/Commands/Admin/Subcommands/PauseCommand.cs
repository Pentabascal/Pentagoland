namespace XPSystem.Commands.Admin.Subcommands
{
    using System;
    using CommandSystem;
    using XPSystem.API;

    public class PauseCommand : SanitizedInputCommand
    {
        public override bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermissionLS("xps.pause"))
            {
                response = "No tienes permiso (xps.pause) para usar este comando.";
                return false;
            }

            if (arguments.Count > 0)
            {
                switch (arguments.At(0).ToLower())
                {
                    case "pause":
                    case "p":
                    case "true":
                    case "1":
                        XPAPI.XPGainPaused = true;
                        response = "Se ha pausado la obtención de XP.";
                        return true;
                    case "unpause":
                    case "up":
                    case "false":
                    case "0":
                        XPAPI.XPGainPaused = false;
                        response = "Se ha reanudado la obtención de XP.";
                        return true;
                }
            }

            response = $"XP gain is currently {(XPAPI.XPGainPaused ? "pausada" : "reanudada")}.";
            return true;
        }

        public override string Command { get; } = "pause";
        public override string[] Aliases { get; } = new[] { "p" };
        public override string Description { get; } = "Pausa la obtención de XP.";
    }
}