namespace XPSystem.Commands.Console.Subcommands
{
    using System;
    using System.IO;
    using CommandSystem;
    using LiteDB;
    using XPSystem.API.Legacy;
    using static API.XPAPI;

    public class MigrateCommand : SanitizedInputCommand
    {
        public override bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string path;
            if (arguments.Count < 1)
            {
                path = Config.LegacyDefaultDatabasePath;
                LogInfo("No file provided, using default.");
            }
            else
            {
                path = string.Join(" ", arguments);
            }

            if (!File.Exists(path))
            {
                response = $"No se encontró el archivo: {path}.";
                return false;
            }

            int affected = -1;
            try
            {
                using (var db = new LiteDatabase(path))
                {
                    affected = LiteDBMigrator.ImportLegacyDB(db);
                }
            }
            catch (Exception e)
            {
                response = $"No se pudo migrar la base de datos: {e}";
                return false;
            }

            response = $"Se han migrado {affected} registros.";
            return true;
        }

        public override string Command { get; } = "migrate";
        public override string[] Aliases { get; } = Array.Empty<string>();
        public override string Description { get; } = "Importa datos de la base de datos antigua de XPSystem.";
    }
}