namespace XPSystem.Commands.Admin.Subcommands
{
    using System;
    using CommandSystem;
    using XPSystem.API;

    public class RefreshCommand : SanitizedInputCommand
    {
        public override bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermissionLS("xps.refresh"))
            {
                response = "No tienes permiso (xps.refresh) para usar este comando.";
                return false;
            }

            XPAPI.DisplayProviders.Refresh();

            response = "Se han actualizado todas las visualizaciones de XP.";
            return true;
        }

        public override string Command { get; } = "refresh";
        public override string[] Aliases { get; } = new[] { "r" };
        public override string Description { get; } = "Actualiza todas las visualizaciones de XP.";
    }
}