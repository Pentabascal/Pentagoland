namespace XPSystem.BuiltInProviders.MySql
{
    using System;
    using CommandSystem;
    using MySqlConnector;
    using XPSystem.API.Enums;
    using XPSystem.Commands;
    using static API.XPAPI;

    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class MySqlTestCommand : SanitizedInputCommand
    {
        public override bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            EnsureStorageProviderValid();

            if (StorageProvider is not MySqlProvider mySqlProvider)
            {
                response = "El proveedor de almacenamiento no es una instancia de MySqlProvider.";
                return false;
            }

            try
            {
                using MySqlConnection connection = mySqlProvider.GetConnection();
                using MySqlCommand command = connection.CreateCommand();
                command.CommandText = $"SELECT COUNT(*) FROM {mySqlProvider.GetTableName(AuthType.Steam)}";

                long count = (long)(command.ExecuteScalar()
                                    ?? throw new NullReferenceException("Query returned null!"));
                response = $"Consulta completada: {count} registros de jugadores en la tabla de Steam.";
                return true;
            }
            catch (Exception e)
            {
                response = $"Error en la consulta: {e.Message}";
                return false;
            }
        }

        public override string Command { get; } = "xp_mysqltest";
        public override string Description { get; } = "Intenta consultar la base de datos MySQL.";
    }
}