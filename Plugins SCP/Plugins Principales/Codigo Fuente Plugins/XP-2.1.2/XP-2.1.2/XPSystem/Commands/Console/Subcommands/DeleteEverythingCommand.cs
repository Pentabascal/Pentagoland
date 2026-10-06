namespace XPSystem.Commands.Console.Subcommands
{
    using System;
    using CommandSystem;
    using XPSystem.API;

    public class DeleteEverythingCommand : SanitizedInputCommand
    {
        private static DateTime _lastUsed = DateTime.MinValue;

        public override bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is not ServerConsoleSender)
            {
                response = "Este comando solo se puede usar desde la consola del servidor.";
                return false;
            }

            XPAPI.EnsureStorageProviderValid();

            if ((DateTime.Now - _lastUsed).TotalSeconds < 10)
            {
                XPAPI.StorageProvider!.DeleteAllPlayerInfo();

                response = "Se han eliminado todos los datos.";
                return true;
            }

            _lastUsed = DateTime.Now;
            response = "¿Seguro que quieres eliminar todos los datos de la base de datos? Vuelve a ejecutar el comando en menos de 10 segundos para confirmar.";
            return true;
        }

        public override string Command { get; } = "deleteeverything";
        public override string[] Aliases { get; } = Array.Empty<string>();
        public override string Description { get; } = "Elimina los datos de todos los jugadores.";
    }
}