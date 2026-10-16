using System;
using System.Globalization;
using System.Linq;
using System.Text;
using CommandSystem;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;

namespace Pentagoland.ServerDoctor;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]
public sealed class ServerDoctorCommand : ICommand
{
    private const int MaximumResponseCharacters = 14000;

    public string Command => "pdoctor";
    public string[] Aliases => Array.Empty<string>();
    public string Description => "Admin-only server diagnostics and guarded configuration tools.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        ServerDoctorPlugin? plugin = ServerDoctorPlugin.Instance;
        if (plugin?.FileAccess is null || plugin.Roots is null)
        {
            response = "Pentagoland Server Doctor no está inicializado.";
            return false;
        }

        string permission = plugin.ActiveConfig.AdminPermission;
        if (string.IsNullOrWhiteSpace(permission) || !sender.HasAnyPermission(permission))
        {
            response = "No tienes permiso para usar Pentagoland Server Doctor.";
            return false;
        }

        string actor = Player.Get(sender)?.UserId ?? "remote-admin-console";
        try
        {
            response = ExecuteCommand(plugin, arguments, actor);
            return true;
        }
        catch (Exception exception) when (
            exception is ArgumentException ||
            exception is UnauthorizedAccessException ||
            exception is IOException ||
            exception is InvalidOperationException ||
            exception is FormatException ||
            exception is System.Security.Cryptography.CryptographicException)
        {
            response = exception.Message;
            return false;
        }
        catch (Exception exception)
        {
            LabApi.Features.Console.Logger.Error($"[Pentagoland Server Doctor] Falló un comando administrativo: {exception}");
            response = "La operación falló. Revisa la consola del servidor para ver el error registrado.";
            return false;
        }
    }

    private static string ExecuteCommand(ServerDoctorPlugin plugin, ArraySegment<string> args, string actor)
    {
        if (args.Count == 0)
            return Help();

        string operation = GetArgument(args, 0).ToLowerInvariant();
        FileAccessService files = plugin.FileAccess!;

        switch (operation)
        {
            case "help":
                return Help();

            case "status":
            case "scan":
                return JoinLines(files.Scan());

            case "health":
                return JoinLines(RuntimeHealth.Snapshot());

            case "loaded-plugins":
                return JoinLines(RuntimePluginInventory.Snapshot());

            case "report":
                if (args.Count != 1)
                    throw new ArgumentException("Uso: pdoctor report");
                return files.CreateSupportReport(
                    actor,
                    RuntimeHealth.Snapshot().Concat(RuntimePluginInventory.Snapshot()));

            case "check-gameplay":
            {
                string[] file = RequireRootedPath(args, "Uso: pdoctor check-gameplay scpsl-config:<puerto>/config_gameplay.txt");
                return JoinLines(files.CheckScpslGameplayConfig(file[0], file[1]));
            }

            case "plugins":
                return JoinLines(files.ScanPluginFiles());

            case "inspect-plugin":
            {
                string[] file = RequireRootedPath(args, "Uso: pdoctor inspect-plugin <raíz>:<plugin.dll>");
                return JoinLines(files.InspectPluginFile(file[0], file[1]));
            }

            case "compat-plugin":
            {
                string[] file = RequireRootedPath(args, "Uso: pdoctor compat-plugin <raíz>:<plugin.dll>");
                return JoinLines(files.AnalyzePluginCompatibility(file[0], file[1]));
            }

            case "dependencies":
                return JoinLines(files.ScanDependencyFiles());

            case "compatibility":
                return JoinLines(files.DescribeCompatibility());

            case "roots":
                return JoinLines(files.DescribeRoots());

            case "files":
            {
                if (args.Count < 2)
                    throw new ArgumentException("Uso: pdoctor files <raíz> [directorio-relativo]");
                string relativeDirectory = args.Count > 2 ? string.Join(" ", args.Skip(2)) : ".";
                var entries = files.ListFiles(GetArgument(args, 1), relativeDirectory);
                return entries.Count == 0 ? "No hay archivos de texto en ese directorio." : JoinLines(entries);
            }

            case "read":
            {
                string[] file = RequireRootedPath(args, "Uso: pdoctor read <raíz>:<archivo>");
                return Limit(files.Read(file[0], file[1]));
            }

            case "tail":
            {
                if (args.Count < 2)
                    throw new ArgumentException("Uso: pdoctor tail logs:<archivo.log> [líneas]");
                string[] file = SplitRootedPath(GetArgument(args, 1));
                int lineCount = 40;
                if (args.Count > 2 && (!int.TryParse(GetArgument(args, 2), NumberStyles.None, CultureInfo.InvariantCulture, out lineCount) || lineCount < 1))
                    throw new ArgumentException("El número de líneas debe ser un entero positivo.");
                return JoinLines(files.Tail(file[0], file[1], lineCount));
            }

            case "backup":
            {
                string[] file = RequireRootedPath(args, "Uso: pdoctor backup <raíz>:<archivo>");
                string id = files.CreateBackup(file[0], file[1], actor);
                return $"Copia creada. ID: {id}";
            }

            case "backups":
            {
                var backups = files.ListBackups();
                return backups.Count == 0 ? "Todavía no hay copias de seguridad." : JoinLines(backups);
            }

            case "restore":
                if (args.Count != 3 || !string.Equals(GetArgument(args, 2), "confirm", StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException("Uso: pdoctor restore <id-de-copia> confirm");
                return files.RestoreBackup(GetArgument(args, 1), actor, confirmed: true);

            case "set":
            {
                if (args.Count < 5 || !string.Equals(GetArgument(args, args.Count - 1), "--confirm", StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException("Uso: pdoctor set <raíz>:<archivo.yml> <clave.ruta> <valor> --confirm");
                string[] file = SplitRootedPath(GetArgument(args, 1));
                string value = string.Join(" ", args.Skip(3).Take(args.Count - 4));
                return files.SetConfigValue(file[0], file[1], GetArgument(args, 2), value, actor, confirmed: true);
            }

            default:
                throw new ArgumentException($"Subcomando desconocido: {operation}. Ejecuta pdoctor help.");
        }
    }

    private static string[] RequireRootedPath(ArraySegment<string> args, string usage)
    {
        if (args.Count != 2)
            throw new ArgumentException(usage);
        return SplitRootedPath(GetArgument(args, 1));
    }

    private static string GetArgument(ArraySegment<string> arguments, int index)
    {
        if (index < 0 || index >= arguments.Count || arguments.Array is null)
            throw new ArgumentOutOfRangeException(nameof(index));

        return arguments.Array[arguments.Offset + index];
    }

    private static string[] SplitRootedPath(string value)
    {
        int separator = value.IndexOf(':');
        if (separator <= 0 || separator == value.Length - 1)
            throw new ArgumentException("Indica la raíz y el archivo, por ejemplo: labapi-configs:ProjectMER/global.yml.");

        return [value.Substring(0, separator), value.Substring(separator + 1)];
    }

    private static string Help() => string.Join(Environment.NewLine,
    new[]
    {
        "Pentagoland Server Doctor — consola del servidor y Remote Admin",
        "pdoctor status | scan       Diagnóstico del proceso, espacio y archivos de plugins.",
        "pdoctor health              Estado en vivo: jugadores, TPS, memoria y proceso.",
        "pdoctor loaded-plugins      Plugins habilitados y dependencias cargadas según LabAPI.",
        "pdoctor report              Genera un informe local con inventario y últimas líneas de logs.",
        "pdoctor check-gameplay <raíz>:<puerto>/config_gameplay.txt",
        "pdoctor plugins             Inventario de DLL instaladas (no prueba que estén cargadas).",
        "pdoctor inspect-plugin <raíz>:<plugin.dll>  Huella y metadatos estáticos de una DLL.",
        "pdoctor compat-plugin <raíz>:<plugin.dll>   Revisa referencias estáticas y assemblies hallados.",
        "pdoctor dependencies        Inventario de dependencias de LabAPI.",
        "pdoctor compatibility      Alcance de compatibilidad y límites de detección.",
        "pdoctor roots               Muestra raíces disponibles y política de acceso.",
        "pdoctor files <raíz> [dir]  Lista archivos de texto de configuración.",
        "pdoctor read <raíz>:<ruta>  Vista limitada y con redacción de secretos/IP.",
        "pdoctor tail logs:<archivo> [n]",
        "pdoctor backup <raíz>:<ruta>",
        "pdoctor backups",
        "pdoctor restore <id> confirm",
        "pdoctor set <raíz>:<config> <clave.ruta> <valor> --confirm",
        "Edición y restauración vienen desactivadas. No se recargan plugins automáticamente."
    });

    private static string JoinLines(System.Collections.Generic.IEnumerable<string> lines) =>
        Limit(string.Join(Environment.NewLine, lines));

    private static string Limit(string value) =>
        value.Length <= MaximumResponseCharacters
            ? value
            : value.Substring(0, MaximumResponseCharacters) + Environment.NewLine + "[Respuesta truncada]";
}
