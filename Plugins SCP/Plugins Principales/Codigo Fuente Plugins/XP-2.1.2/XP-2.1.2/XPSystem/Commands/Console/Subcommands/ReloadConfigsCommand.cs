namespace XPSystem.Commands.Console.Subcommands
{
    using System;
    using CommandSystem;

    public class ReloadConfigsCommand : SanitizedInputCommand
    {
        public override bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Main.Instance!.LoadExtraConfigs();
            response = "Se han vuelto a cargar los archivos de configuración adicionales (para recargar la configuración principal, utiliza el cargador de complementos).";
            return true;
        }

        public override string Command { get; } = "reloadconfigs";
        public override string[] Aliases { get; } = new[] { "reload" };
        public override string Description { get; } = "Vuelve a cargar los archivos de configuración adicionales.";
    }
}