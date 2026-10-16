using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Pentagoland.ServerDoctor;

public sealed class FileAccessService
{
    private const int MaximumPreviewCharacters = 18000;
    private const int MaximumDirectoryEntries = 100;
    private const int MaximumScanDirectoriesPerRoot = 256;
    private const int MaximumDllsPerRoot = 500;
    private const int MaximumFilesPerRoot = 10000;
    private const int MaximumCompatibilityAssemblies = 5000;
    private const int MaximumTailLines = 100;
    private const long MaximumBackupStoreBytes = 268435456;
    private const long MaximumAuditBytes = 5242880;
    private const int MaximumSupportReports = 30;
    private const int MaximumSupportReportBytes = 1048576;
    private const long MaximumPluginInspectionBytes = 134217728;
    private static readonly HashSet<string> ReadableExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".cfg", ".conf", ".ini", ".json", ".log", ".properties", ".toml", ".txt", ".xml", ".yaml", ".yml"
    };
    private static readonly Regex SecretLine = new(
        "(?i)(password|passwd|token|secret|api[_-]?(?:key|token)|auth(?:orization)?|rcon|webhook|connection[_-]?string|private[_-]?key|license[_-]?key|steam[_-]?(?:id|web[_-]?api[_-]?key))([\"']?\\s*[:=]\\s*)(.*)$",
        RegexOptions.Compiled | RegexOptions.Multiline);
    private static readonly Regex SensitiveInline = new(
        "(?i)([?&](?:token|key|secret|password)=)[^&\\s]+|\\bBearer\\s+[^\\s,;]+",
        RegexOptions.Compiled);
    private static readonly Regex Ipv4 = new(
        @"\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b",
        RegexOptions.Compiled);
    private static readonly Regex YamlBlockScalarIndicator = new(
        @"^[|>](?:[1-9][+-]?|[+-][1-9]?)?(?:\s+#.*)?$",
        RegexOptions.Compiled);

    private readonly string _serverRoot;
    private readonly RootCatalog _roots;
    private readonly ServerDoctorConfig _config;
    private readonly IConfigEditorAdapter[] _configEditors;
    private readonly string _dataDirectory;
    private readonly string _backupDirectory;
    private readonly string _reportDirectory;
    private readonly string _auditPath;
    private readonly object _backupLock = new();
    private readonly object _reportLock = new();
    private readonly object _auditLock = new();

    public FileAccessService(
        string serverRoot,
        RootCatalog roots,
        ServerDoctorConfig config,
        IEnumerable<IConfigEditorAdapter>? configEditors = null)
    {
        _serverRoot = Path.GetFullPath(serverRoot);
        _roots = roots;
        _config = config;
        _configEditors = (configEditors ?? new[] { new YamlScalarConfigEditor() }).ToArray();
        _dataDirectory = Path.Combine(_serverRoot, "PentagolandServerDoctor");
        _backupDirectory = Path.Combine(_dataDirectory, "backups");
        _reportDirectory = Path.Combine(_dataDirectory, "reports");
        _auditPath = Path.Combine(_dataDirectory, "audit.log");
    }

    public string CreateSupportReport(string actor, IEnumerable<string> healthLines)
    {
        lock (_reportLock)
        {
            EnsureDataDirectory();
            Directory.CreateDirectory(_reportDirectory);
            SafePath.EnsureNoReparsePoints(_serverRoot, _reportDirectory);

            int reportCount = Directory.EnumerateFiles(_reportDirectory, "support-*.txt", SearchOption.TopDirectoryOnly)
                .Count();
            if (reportCount >= MaximumSupportReports)
                throw new IOException($"Se alcanzó el límite de {MaximumSupportReports} informes. Descarga y elimina los antiguos desde el host.");

            var report = new StringBuilder()
                .AppendLine("Informe de soporte - Pentagoland Server Doctor")
                .AppendLine($"Generado UTC: {DateTime.UtcNow:O}")
                .AppendLine()
                .AppendLine("[Estado en vivo]");
            foreach (string line in healthLines)
                report.AppendLine(line);

            report.AppendLine().AppendLine("[Compatibilidad detectada]");
            foreach (string line in DescribeCompatibility())
                report.AppendLine(line);

            report.AppendLine().AppendLine("[Inventario de plugins]");
            foreach (string line in ScanPluginFiles())
                report.AppendLine(line);

            report.AppendLine().AppendLine("[Inventario de dependencias]");
            foreach (string line in ScanDependencyFiles())
                report.AppendLine(line);

            AppendRecentLogs(report);

            byte[] reportBytes = new UTF8Encoding(false).GetBytes(Redact(report.ToString()));
            if (reportBytes.Length > MaximumSupportReportBytes)
                throw new IOException("El informe supera el límite de 1 MiB; reduce el tamaño de los logs e inténtalo de nuevo.");

            string id = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss", System.Globalization.CultureInfo.InvariantCulture) +
                        "-" + Guid.NewGuid().ToString("N").Substring(0, 8);
            string reportPath = Path.Combine(_reportDirectory, $"support-{id}.txt");
            string temporaryPath = reportPath + ".tmp";
            SafePath.EnsureNoReparsePoints(_serverRoot, reportPath);
            try
            {
                using (var stream = new FileStream(temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, FileOptions.WriteThrough))
                {
                    stream.Write(reportBytes, 0, reportBytes.Length);
                    stream.Flush(flushToDisk: true);
                }

                WriteAudit(actor, "support-report", $"id={id}");
                File.Move(temporaryPath, reportPath);
            }
            finally
            {
                if (File.Exists(temporaryPath))
                    File.Delete(temporaryPath);
            }

            return $"Informe local creado: {reportPath}. Revísalo antes de compartirlo.";
        }
    }

    private void AppendRecentLogs(StringBuilder report)
    {
        report.AppendLine().AppendLine("[Logs recientes del servidor]");
        if (!_roots.TryGet("logs", out AccessRoot logsRoot) || !Directory.Exists(logsRoot.Path))
        {
            report.AppendLine("No se encontró la raíz logs.");
            return;
        }

        try
        {
            SafePath.EnsureNoReparsePoints(logsRoot.Path);
            string[] recentLogs = Directory.EnumerateFiles(logsRoot.Path, "*", SearchOption.TopDirectoryOnly)
                .Where(path => (Path.GetExtension(path).Equals(".log", StringComparison.OrdinalIgnoreCase) ||
                                Path.GetExtension(path).Equals(".txt", StringComparison.OrdinalIgnoreCase)) &&
                               (File.GetAttributes(path) & FileAttributes.ReparsePoint) == 0)
                .OrderByDescending(File.GetLastWriteTimeUtc)
                .Take(3)
                .ToArray();

            if (recentLogs.Length == 0)
            {
                report.AppendLine("No se encontraron logs .log o .txt.");
                return;
            }

            foreach (string path in recentLogs)
            {
                string relativePath = path.Substring(logsRoot.Path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Length)
                    .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                report.AppendLine($"--- {Path.GetFileName(path)} ---");
                foreach (string line in Tail("logs", relativePath, 40))
                    report.AppendLine(line);
            }
        }
        catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException || exception is ArgumentException)
        {
            report.AppendLine($"No se pudieron adjuntar logs: {exception.Message}");
        }
    }

    public IReadOnlyList<string> ListFiles(string rootName, string relativeDirectory)
    {
        AccessRoot root = GetRoot(rootName);
        string directory = SafePath.ResolveDirectory(root, relativeDirectory);
        if (!Directory.Exists(directory))
            throw new DirectoryNotFoundException($"No existe el directorio '{rootName}:{relativeDirectory}'.");

        var entries = new List<string>();
        bool omittedEntries = false;
        foreach (string file in Directory.EnumerateFiles(directory)
                     .Where(path => ReadableExtensions.Contains(Path.GetExtension(path)))
                     .Take(MaximumDirectoryEntries + 1)
                     .OrderBy(path => path, StringComparer.OrdinalIgnoreCase))
        {
            if (entries.Count == MaximumDirectoryEntries)
            {
                omittedEntries = true;
                break;
            }

            string relative = file.Substring(root.Path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Length)
                .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            entries.Add($"{root.Name}:{relative}");
        }

        int directoryEntryLimit = MaximumDirectoryEntries - entries.Count;
        if (!omittedEntries)
        {
            int inspectedDirectories = 0;
            foreach (string child in Directory.EnumerateDirectories(directory))
            {
                if ((File.GetAttributes(child) & FileAttributes.ReparsePoint) != 0)
                    continue;

                if (inspectedDirectories++ >= directoryEntryLimit)
                {
                    omittedEntries = true;
                    break;
                }

                entries.Add($"[{Path.GetFileName(child)}]");
            }
        }

        if (omittedEntries)
        {
            if (entries.Count >= MaximumDirectoryEntries)
                entries.RemoveAt(entries.Count - 1);
            entries.Add("[Listado parcial: se omitieron entradas adicionales.]");
        }

        return entries;
    }

    public string Read(string rootName, string relativePath)
    {
        string path = ResolveReadableFile(rootName, relativePath);
        var info = new FileInfo(path);
        if (info.Length > MaximumPreviewCharacters * 4)
            throw new IOException("El archivo supera el límite de lectura. Usa el visor de archivos del host.");

        string content = ReadUtf8(path, out _, MaximumPreviewCharacters * 4L);
        content = Redact(content);
        if (content.Length > MaximumPreviewCharacters)
            content = content.Substring(0, MaximumPreviewCharacters) + Environment.NewLine + "[Vista truncada]";
        return content;
    }

    public IReadOnlyList<string> CheckScpslGameplayConfig(string rootName, string relativePath)
    {
        if (!rootName.Equals("scpsl-config", StringComparison.OrdinalIgnoreCase) &&
            !rootName.Equals("scpsl-user-config", StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("La comprobación solo acepta las raíces scpsl-config o scpsl-user-config.");

        var editor = new ScpslTextConfigEditor();
        if (!editor.Supports(rootName, relativePath, Path.GetExtension(relativePath)))
            throw new UnauthorizedAccessException("La comprobación solo acepta un archivo config_gameplay.txt.");

        string path = ResolveReadableFile(rootName, relativePath);
        string content = ReadUtf8(path, out _, GetMaximumConfigFileBytes());
        return ScpslGameplayConfigValidator.Validate(content);
    }

    public IReadOnlyList<string> InspectPluginFile(string rootName, string relativePath)
    {
        if (!Path.GetExtension(relativePath).Equals(".dll", StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("La inspección de plugin solo acepta archivos DLL.");

        AccessRoot root = GetRoot(rootName);
        string path = SafePath.ResolveFile(root, relativePath, mustExist: true);
        var info = new FileInfo(path);
        if (info.Length > MaximumPluginInspectionBytes)
            throw new IOException("El archivo DLL supera el límite de inspección de 128 MiB.");

        FileVersionInfo version = FileVersionInfo.GetVersionInfo(path);
        var lines = new List<string>
        {
            $"Archivo: {root.Name}:{relativePath}",
            $"Tamaño: {FormatBytes(info.Length)}",
            $"Última modificación UTC: {info.LastWriteTimeUtc:O}",
            $"SHA-256: {ComputeSha256(path)}",
            $"Producto declarado: {SanitizeMetadata(version.ProductName)}",
            $"Versión de producto declarada: {SanitizeMetadata(version.ProductVersion)}",
            $"Versión de archivo declarada: {SanitizeMetadata(version.FileVersion)}",
            "Inspección estática de metadatos de archivo; no carga ni ejecuta la DLL y no verifica compatibilidad binaria."
        };
        return lines;
    }

    public IReadOnlyList<string> AnalyzePluginCompatibility(string rootName, string relativePath)
    {
        if (!Path.GetExtension(relativePath).Equals(".dll", StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("El análisis de compatibilidad solo acepta archivos DLL.");

        AccessRoot root = GetRoot(rootName);
        string pluginPath = SafePath.ResolveFile(root, relativePath, mustExist: true);
        if (new FileInfo(pluginPath).Length > MaximumPluginInspectionBytes)
            throw new IOException("El archivo DLL supera el límite de análisis de 128 MiB.");

        (List<string> paths, List<string> warnings) = CollectCompatibilityAssemblies();
        IReadOnlyList<string> analysis = PluginCompatibilityInspector.Analyze(pluginPath, paths);
        var lines = new List<string>
        {
            $"Plugin analizado: {root.Name}:{relativePath}",
            $"SHA-256: {ComputeSha256(pluginPath)}",
            $"Ensamblados examinados por nombre: {paths.Count}"
        };
        lines.AddRange(warnings);
        lines.AddRange(analysis);
        return lines;
    }

    private (List<string> Paths, List<string> Warnings) CollectCompatibilityAssemblies()
    {
        var paths = new List<string>();
        var warnings = new List<string>();
        var roots = new Dictionary<string, AccessRoot>(StringComparer.OrdinalIgnoreCase);
        foreach (string name in (_config.PluginScanRoots ?? [])
                     .Concat(_config.DependencyScanRoots ?? [])
                     .Concat(_roots.LabApiPluginScanRoots)
                     .Concat(_roots.LabApiDependencyScanRoots))
        {
            if (_roots.TryGet(name, out AccessRoot configuredRoot))
                roots[name] = configuredRoot;
            else
                warnings.Add($"Raíz de compatibilidad desconocida omitida: {name}.");
        }

        string managedRoot = Path.Combine(_serverRoot, "SCPSL_Data", "Managed");
        if (Directory.Exists(managedRoot))
        {
            try
            {
                SafePath.EnsureNoReparsePoints(managedRoot);
                foreach (string file in Directory.EnumerateFiles(managedRoot, "*", SearchOption.TopDirectoryOnly))
                {
                    if (!string.Equals(Path.GetExtension(file), ".dll", StringComparison.OrdinalIgnoreCase))
                        continue;
                    if ((File.GetAttributes(file) & FileAttributes.ReparsePoint) == 0)
                        paths.Add(file);
                    if (paths.Count >= MaximumCompatibilityAssemblies)
                        break;
                }
            }
            catch (Exception exception) when (
                exception is IOException ||
                exception is UnauthorizedAccessException ||
                exception is ArgumentException)
            {
                warnings.Add($"No se pudo inspeccionar SCPSL_Data\\Managed: {exception.Message}");
            }
        }
        else
        {
            warnings.Add("No se encontró SCPSL_Data\\Managed; no se pudieron contrastar las referencias del juego.");
        }

        foreach (var entry in roots)
        {
            if (paths.Count >= MaximumCompatibilityAssemblies)
            {
                warnings.Add("El inventario de compatibilidad alcanzó el límite global de ensamblados.");
                break;
            }
            if (!Directory.Exists(entry.Value.Path))
                continue;

            try
            {
                SafePath.EnsureNoReparsePoints(entry.Value.Path);
                var pending = new Queue<(string Path, int Depth)>();
                pending.Enqueue((entry.Value.Path, 0));
                int visited = 0;
                int filesSeen = 0;
                while (pending.Count > 0 && visited < MaximumScanDirectoriesPerRoot &&
                       filesSeen < MaximumFilesPerRoot && paths.Count < MaximumCompatibilityAssemblies)
                {
                    (string directory, int depth) = pending.Dequeue();
                    visited++;
                    foreach (string file in Directory.EnumerateFiles(directory, "*", SearchOption.TopDirectoryOnly))
                    {
                        filesSeen++;
                        if (string.Equals(Path.GetExtension(file), ".dll", StringComparison.OrdinalIgnoreCase) &&
                            (File.GetAttributes(file) & FileAttributes.ReparsePoint) == 0)
                            paths.Add(file);

                        if (filesSeen >= MaximumFilesPerRoot || paths.Count >= MaximumCompatibilityAssemblies)
                            break;
                    }

                    if (depth >= 8 || filesSeen >= MaximumFilesPerRoot || paths.Count >= MaximumCompatibilityAssemblies)
                        continue;

                    foreach (string child in Directory.EnumerateDirectories(directory, "*", SearchOption.TopDirectoryOnly))
                    {
                        if ((File.GetAttributes(child) & FileAttributes.ReparsePoint) == 0)
                            pending.Enqueue((child, depth + 1));
                    }
                }

                if (pending.Count > 0 || visited >= MaximumScanDirectoriesPerRoot || filesSeen >= MaximumFilesPerRoot)
                    warnings.Add($"Inventario parcial para {entry.Key}: se alcanzó un límite de recorrido.");
            }
            catch (Exception exception) when (
                exception is IOException ||
                exception is UnauthorizedAccessException ||
                exception is ArgumentException)
            {
                warnings.Add($"No se pudo inspeccionar la raíz {entry.Key}: {exception.Message}");
            }
        }

        return (paths.Distinct(StringComparer.OrdinalIgnoreCase).ToList(), warnings);
    }

    public IReadOnlyList<string> Tail(string rootName, string relativePath, int lineCount)
    {
        if (!string.Equals(rootName, "logs", StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("El comando tail solo acepta archivos dentro de la raíz logs.");

        string path = ResolveReadableFile(rootName, relativePath);
        if (!string.Equals(Path.GetExtension(path), ".log", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(Path.GetExtension(path), ".txt", StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("tail solo acepta archivos .log o .txt.");

        int count = Clamp(lineCount, 1, MaximumTailLines);
        const int windowBytes = 262144;
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        if (stream.Length > windowBytes)
            stream.Seek(-windowBytes, SeekOrigin.End);

        using var reader = new StreamReader(stream, new UTF8Encoding(false, false), true);
        if (stream.Position > 0)
            _ = reader.ReadLine();

        var lines = new Queue<string>(count);
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            if (lines.Count == count)
                lines.Dequeue();
            lines.Enqueue(line);
        }

        if (lines.Count == 0)
            return Array.Empty<string>();

        return Regex.Split(Redact(string.Join(Environment.NewLine, lines)), "\\r\\n|\\n|\\r");
    }

    public string SetConfigValue(string rootName, string relativePath, string dottedKey, string value, string actor, bool confirmed)
    {
        if (!confirmed)
            throw new InvalidOperationException("Falta la confirmación explícita '--confirm'; no se modificó el archivo.");
        EnsureChangesEnabled(rootName);
        string path = ResolveReadableFile(rootName, relativePath);
        string extension = Path.GetExtension(path);
        IConfigEditorAdapter? editor = _configEditors.FirstOrDefault(candidate =>
            candidate.Supports(rootName, relativePath, extension));
        if (editor is null)
            throw new UnauthorizedAccessException($"No hay un editor seguro instalado para '{extension}'.");

        var info = new FileInfo(path);
        if (info.Length > GetMaximumConfigFileBytes())
            throw new IOException("El archivo supera el tamaño máximo de edición configurado.");

        string originalHash = ComputeSha256(path);
        string original = ReadUtf8(path, out bool hasBom, GetMaximumConfigFileBytes());
        if (!editor.TrySetValue(original, dottedKey, value, out string updated, out string error))
            throw new InvalidOperationException(error);
        if (string.Equals(original, updated, StringComparison.Ordinal))
            return "El valor ya coincide; no se modificó el archivo.";

        string backupId = CreateBackup(rootName, relativePath, actor);
        if (!string.Equals(ComputeSha256(path), originalHash, StringComparison.OrdinalIgnoreCase))
            throw new IOException("El archivo cambió durante la edición. Se conservó la copia y no se sobrescribió el cambio externo.");
        WriteAudit(actor, "config-set", $"{rootName}:{relativePath} key={dottedKey} backup={backupId}");
        AtomicWrite(path, EncodeUtf8(updated, hasBom));
        return $"{editor.Name} guardado. Copia previa: {backupId}. No se recargó ningún plugin; aplica el cambio con el método recomendado por su autor.";
    }

    public string CreateBackup(string rootName, string relativePath, string actor)
    {
        lock (_backupLock)
            return CreateBackupCore(rootName, relativePath, actor);
    }

    private string CreateBackupCore(string rootName, string relativePath, string actor)
    {
        AccessRoot root = GetRoot(rootName);
        string source = SafePath.ResolveFile(root, relativePath, mustExist: true);
        EnsureReadableExtension(source);

        var info = new FileInfo(source);
        if (info.Length > GetMaximumConfigFileBytes())
            throw new IOException("El archivo supera el tamaño máximo de copia configurado.");

        EnsureDataDirectory();
        int maximumBackups = Clamp(_config.MaximumBackupCount, 1, 500);
        int existingCount = Directory.EnumerateFiles(_backupDirectory, "*.meta", SearchOption.TopDirectoryOnly).Count();
        if (existingCount >= maximumBackups)
            throw new IOException($"Se alcanzó el límite de {maximumBackups} copias. Descarga o limpia copias antiguas desde el host.");

        long currentStoreBytes = Directory.EnumerateFiles(_backupDirectory, "*.bin", SearchOption.TopDirectoryOnly)
            .Sum(path => new FileInfo(path).Length);
        if (info.Length > MaximumBackupStoreBytes - currentStoreBytes)
            throw new IOException("Se alcanzó el límite de 256 MiB de copias. Descarga o limpia copias antiguas desde el host.");

        string id = Guid.NewGuid().ToString("N");
        string backupPath = Path.Combine(_backupDirectory, id + ".bin");
        string metadataPath = Path.Combine(_backupDirectory, id + ".meta");
        string tempBackupPath = backupPath + ".tmp";
        string tempMetadataPath = metadataPath + ".tmp";
        string relativeIdentity = Convert.ToBase64String(Encoding.UTF8.GetBytes(root.Name + "\n" + relativePath));
        WriteAudit(actor, "backup-create", $"{root.Name}:{relativePath} id={id}");
        try
        {
            CopyFileBounded(source, tempBackupPath, GetMaximumConfigFileBytes());
            if (new FileInfo(tempBackupPath).Length > MaximumBackupStoreBytes - currentStoreBytes)
                throw new IOException("El límite de 256 MiB de copias se alcanzó durante la lectura; no se guardó la copia.");
            string hash = ComputeSha256(tempBackupPath);
            File.WriteAllText(tempMetadataPath, relativeIdentity + Environment.NewLine + hash, new UTF8Encoding(false));
            File.Move(tempBackupPath, backupPath);
            File.Move(tempMetadataPath, metadataPath);
            return id;
        }
        catch
        {
            if (!File.Exists(metadataPath) && File.Exists(backupPath))
                File.Delete(backupPath);
            throw;
        }
        finally
        {
            if (File.Exists(tempBackupPath))
                File.Delete(tempBackupPath);
            if (File.Exists(tempMetadataPath))
                File.Delete(tempMetadataPath);
        }
    }

    public string RestoreBackup(string id, string actor, bool confirmed)
    {
        if (!confirmed)
            throw new InvalidOperationException("Para restaurar, añade 'confirm' después del ID. No se modificó el archivo.");
        if (!Regex.IsMatch(id ?? string.Empty, "^[a-f0-9]{32}$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
            throw new ArgumentException("El identificador de copia no es válido.");

        string metadataPath = Path.Combine(_backupDirectory, id + ".meta");
        string backupPath = Path.Combine(_backupDirectory, id + ".bin");
        EnsureDataDirectory();
        SafePath.EnsureNoReparsePoints(_serverRoot, metadataPath);
        SafePath.EnsureNoReparsePoints(_serverRoot, backupPath);
        if (!File.Exists(metadataPath) || !File.Exists(backupPath))
            throw new FileNotFoundException("No se encontró esa copia de seguridad.");

        if (new FileInfo(metadataPath).Length > 4096)
            throw new InvalidDataException("El manifiesto de la copia supera el tamaño permitido.");

        string[] metadata = File.ReadAllLines(metadataPath);
        if (metadata.Length != 2)
            throw new InvalidDataException("El manifiesto de la copia está dañado.");

        string identity = Encoding.UTF8.GetString(Convert.FromBase64String(metadata[0]));
        string[] identityParts = identity.Split(new[] { '\n' }, 2);
        if (identityParts.Length != 2)
            throw new InvalidDataException("El manifiesto de la copia no contiene una ruta válida.");

        AccessRoot root = GetWritableRoot(identityParts[0]);
        string target = SafePath.ResolveFile(root, identityParts[1], mustExist: true);
        EnsureReadableExtension(target);
        if (new FileInfo(backupPath).Length > GetMaximumConfigFileBytes())
            throw new IOException("La copia supera el tamaño máximo configurado.");
        if (!Regex.IsMatch(metadata[1], "^[a-f0-9]{64}$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
            throw new InvalidDataException("El hash del manifiesto de la copia no es válido.");
        if (!string.Equals(ComputeSha256(backupPath), metadata[1], StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("La copia no coincide con su hash SHA-256; no se restauró.");

        string originalTargetHash = ComputeSha256(target);
        string rollbackId = CreateBackup(root.Name, identityParts[1], actor);
        if (!string.Equals(ComputeSha256(target), originalTargetHash, StringComparison.OrdinalIgnoreCase))
            throw new IOException("El archivo cambió durante la restauración. Se conservó la copia y no se sobrescribió el cambio externo.");
        WriteAudit(actor, "backup-restore", $"{root.Name}:{identityParts[1]} source={id} rollback={rollbackId}");
        string tempPath = Path.Combine(Path.GetDirectoryName(target)!, $".{Path.GetFileName(target)}.{Guid.NewGuid():N}.tmp");
        try
        {
            CopyFileBounded(backupPath, tempPath, GetMaximumConfigFileBytes());
            ReplaceExistingFile(tempPath, target);
        }
        finally
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
        return $"Copia restaurada. Estado anterior guardado como: {rollbackId}. No se recargó ningún plugin.";
    }

    public IReadOnlyList<string> ListBackups()
    {
        if (!Directory.Exists(_backupDirectory))
            return Array.Empty<string>();

        SafePath.EnsureNoReparsePoints(_serverRoot, _backupDirectory);
        var results = new List<string>();
        foreach (string metadataPath in Directory.EnumerateFiles(_backupDirectory, "*.meta", SearchOption.TopDirectoryOnly)
                     .OrderByDescending(File.GetLastWriteTimeUtc)
                     .Take(MaximumDirectoryEntries))
        {
            string id = Path.GetFileNameWithoutExtension(metadataPath);
            string description = id;
            if ((File.GetAttributes(metadataPath) & FileAttributes.ReparsePoint) != 0)
            {
                results.Add(description + " [enlace ignorado]");
                continue;
            }

            if (new FileInfo(metadataPath).Length > 4096)
            {
                description += " [manifiesto inválido]";
            }
            else
            {
                try
                {
                    string[] metadata = File.ReadAllLines(metadataPath);
                    if (metadata.Length == 2)
                    {
                        string identity = Encoding.UTF8.GetString(Convert.FromBase64String(metadata[0]));
                        string[] parts = identity.Split(new[] { '\n' }, 2);
                        if (parts.Length == 2)
                            description += $" {parts[0]}:{parts[1]}";
                        else
                            description += " [manifiesto inválido]";
                    }
                    else
                    {
                        description += " [manifiesto inválido]";
                    }
                }
                catch (FormatException)
                {
                    description += " [manifiesto inválido]";
                }
            }

            results.Add(description);
        }

        return results;
    }

    public IReadOnlyList<string> ScanPluginFiles()
        => ScanDllRoots(
            (_config.PluginScanRoots ?? []).Concat(_roots.LabApiPluginScanRoots),
            "Inventario de DLL de plugins instaladas; no confirma que se hayan cargado correctamente.");

    public IReadOnlyList<string> ScanDependencyFiles()
        => ScanDllRoots(
            (_config.DependencyScanRoots ?? []).Concat(_roots.LabApiDependencyScanRoots),
            "Inventario de DLL de dependencias de LabAPI; no confirma que se hayan cargado correctamente.");

    private IReadOnlyList<string> ScanDllRoots(IEnumerable<string>? scanRoots, string header)
    {
        var found = new List<(string Name, string Root, string RelativePath, long Bytes)>();
        var seenFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var errors = new List<string>();
        bool scanLimitReached = false;
        int configuredRootCount = 0;

        foreach (string alias in (scanRoots ?? []).Where(alias => !string.IsNullOrWhiteSpace(alias))
                     .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!_roots.TryGet(alias, out AccessRoot root))
            {
                errors.Add($"Se omitió la raíz de inventario desconocida '{alias}'.");
                continue;
            }
            configuredRootCount++;
            if (!Directory.Exists(root.Path))
                continue;

            try
            {
                SafePath.EnsureNoReparsePoints(root.Path);
                var directories = new Queue<(string Path, int Depth)>();
                directories.Enqueue((root.Path, 0));
                int visitedDirectories = 0;
                int rootDllCount = 0;
                int rootFilesInspected = 0;
                while (directories.Count > 0 && rootDllCount < MaximumDllsPerRoot)
                {
                    if (visitedDirectories >= MaximumScanDirectoriesPerRoot)
                    {
                        scanLimitReached = true;
                        break;
                    }

                    (string directory, int depth) = directories.Dequeue();
                    visitedDirectories++;
                    foreach (string file in Directory.EnumerateFiles(directory, "*", SearchOption.TopDirectoryOnly))
                    {
                        if (rootFilesInspected >= MaximumFilesPerRoot)
                        {
                            scanLimitReached = true;
                            break;
                        }
                        rootFilesInspected++;
                        if (!string.Equals(Path.GetExtension(file), ".dll", StringComparison.OrdinalIgnoreCase))
                            continue;

                        if (rootDllCount >= MaximumDllsPerRoot)
                        {
                            scanLimitReached = true;
                            break;
                        }

                        if ((File.GetAttributes(file) & FileAttributes.ReparsePoint) == 0)
                        {
                            if (!seenFiles.Add(Path.GetFullPath(file)))
                                continue;

                            string relativePath = file.Substring(root.Path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Length)
                                .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                            found.Add((Path.GetFileName(file), alias, relativePath, new FileInfo(file).Length));
                            rootDllCount++;
                        }
                    }

                    if (depth >= 8)
                    {
                        if (Directory.EnumerateDirectories(directory, "*", SearchOption.TopDirectoryOnly).Any())
                            scanLimitReached = true;
                        continue;
                    }

                    foreach (string child in Directory.EnumerateDirectories(directory, "*", SearchOption.TopDirectoryOnly))
                    {
                        if ((File.GetAttributes(child) & FileAttributes.ReparsePoint) != 0)
                            continue;

                        if (visitedDirectories + directories.Count >= MaximumScanDirectoriesPerRoot)
                        {
                            scanLimitReached = true;
                            break;
                        }

                        directories.Enqueue((child, depth + 1));
                    }
                }

                if (rootDllCount >= MaximumDllsPerRoot && directories.Count > 0)
                    scanLimitReached = true;
            }
            catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException || exception is ArgumentException)
            {
                errors.Add($"No se pudo inspeccionar {alias}: {exception.Message}");
            }
        }

        var output = new List<string>
        {
            header
        };
        output.AddRange(errors);
        if (scanLimitReached)
            output.Add("Inventario parcial: se alcanzó el límite seguro de recorrido.");

        foreach (var group in found.GroupBy(entry => entry.Name, StringComparer.OrdinalIgnoreCase)
                     .OrderBy(group => group.Key, StringComparer.OrdinalIgnoreCase))
        {
            string duplicates = group.Count() > 1 ? " [nombre repetido]" : string.Empty;
            foreach (var entry in group)
                output.Add($"{entry.Name} | {entry.Root}:{entry.RelativePath} | {FormatBytes(entry.Bytes)}{duplicates}");
        }

        if (found.Count == 0)
        {
            output.Add(configuredRootCount == 0
                ? "Inventario desactivado: no hay raíces válidas configuradas."
                : "No se encontraron DLL en las carpetas de inventario accesibles.");
        }

        return output;
    }

    public IReadOnlyList<string> DescribeRoots()
    {
        var lines = new List<string>(_roots.Warnings);
        foreach (AccessRoot root in _roots.All)
        {
            bool exists = Directory.Exists(root.Path);
            bool writable = IsWritable(root.Name);
            lines.Add($"{root.Name} | {(exists ? "disponible" : "no encontrada")} | {(writable ? "restauración/edición controlada" : "solo lectura")} | {root.Path}");
        }

        return lines;
    }

    public IReadOnlyList<string> DescribeCompatibility()
    {
        bool labApiAssembly = File.Exists(Path.Combine(_serverRoot, "SCPSL_Data", "Managed", "LabApi.dll"));
        bool exiledData = _roots.TryGet("exiled-data", out AccessRoot exiledRoot) && Directory.Exists(exiledRoot.Path);
        bool exiledServerPlugins = _roots.TryGet("exiled-server-plugins", out AccessRoot serverExiledRoot) && Directory.Exists(serverExiledRoot.Path);
        return
        [
            "Compatibilidad comprobable desde este proceso:",
            $"LabAPI: {(labApiAssembly ? "ensamblado del servidor localizado" : "no detectado en la ubicación estándar")}; el plugin se compila para LabAPI.",
            $"EXILED: {(exiledData || exiledServerPlugins ? "carpetas detectadas; compatibilidad a nivel de archivos" : "no detectado en las rutas conocidas")}.",
            "No se consulta el estado interno, no se carga código de terceros y no se valida si cada plugin está activo.",
            "Sin sus DLL, versiones y logs del servidor real no se puede certificar compatibilidad individual.",
            "El plugin no registra eventos de juego ni aplica Harmony patches; las escrituras exigen permiso, raíz autorizada, opción activada y confirmación."
        ];
    }

    public IReadOnlyList<string> Scan()
    {
        List<string> writableRoots = _config.WritableRoots ?? [];
        var lines = new List<string>
        {
            $"Directorio del servidor: {_serverRoot}",
            $"LabAPI instalado: {File.Exists(Path.Combine(_serverRoot, "SCPSL_Data", "Managed", "LabApi.dll")) || Directory.Exists(Path.Combine(_serverRoot, "LabAPI"))}",
            $"Directorio de datos del plugin: {(Directory.Exists(_dataDirectory) ? "ya existe" : "se creará al hacer una copia o editar")}",
            $"Edición YAML habilitada: {_config.AllowConfigChanges}",
            $"Raíces de escritura: {(writableRoots.Count == 0 ? "ninguna" : string.Join(", ", writableRoots))}"
        };

        try
        {
            var drive = new DriveInfo(Path.GetPathRoot(_serverRoot)!);
            lines.Add($"Espacio libre: {FormatBytes(drive.AvailableFreeSpace)}");
        }
        catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException || exception is ArgumentException)
        {
            lines.Add($"Espacio libre: no disponible ({exception.Message})");
        }

        try
        {
            using var process = System.Diagnostics.Process.GetCurrentProcess();
            TimeSpan uptime = DateTime.UtcNow - process.StartTime.ToUniversalTime();
            lines.Add($"Proceso: {process.ProcessName} | activo {uptime.Days}d {uptime.Hours}h {uptime.Minutes}m");
            lines.Add($"Memoria administrada actual: {FormatBytes(GC.GetTotalMemory(forceFullCollection: false))}");
        }
        catch (Exception exception) when (
            exception is System.ComponentModel.Win32Exception ||
            exception is InvalidOperationException ||
            exception is PlatformNotSupportedException ||
            exception is UnauthorizedAccessException)
        {
            lines.Add($"Datos del proceso: no disponibles ({exception.Message})");
        }

        lines.AddRange(ScanPluginFiles());
        lines.AddRange(ScanDependencyFiles());
        return lines;
    }

    private string ResolveReadableFile(string rootName, string relativePath)
    {
        AccessRoot root = GetRoot(rootName);
        string path = SafePath.ResolveFile(root, relativePath, mustExist: true);
        EnsureReadableExtension(path);
        return path;
    }

    private AccessRoot GetRoot(string name)
    {
        if (!_roots.TryGet(name, out AccessRoot root))
            throw new ArgumentException($"Raíz desconocida '{name}'. Ejecuta pdoctor roots.");
        return root;
    }

    private AccessRoot GetWritableRoot(string name)
    {
        AccessRoot root = GetRoot(name);
        EnsureChangesEnabled(root.Name);
        return root;
    }

    private void EnsureChangesEnabled(string rootName)
    {
        if (!_config.AllowConfigChanges)
            throw new UnauthorizedAccessException("La edición está deshabilitada. Activa AllowConfigChanges y reinicia el servidor.");
        if (!IsWritable(rootName))
            throw new UnauthorizedAccessException($"La raíz '{rootName}' no está en WritableRoots.");
    }

    private bool IsWritable(string rootName) =>
        _config.WritableRoots?.Any(name => string.Equals(name, rootName, StringComparison.OrdinalIgnoreCase)) == true;

    private int GetMaximumConfigFileBytes() => Clamp(_config.MaximumConfigFileBytes, 1024, 16777216);

    private static void EnsureReadableExtension(string path)
    {
        if (!ReadableExtensions.Contains(Path.GetExtension(path)))
            throw new UnauthorizedAccessException("Solo se permiten archivos de texto de configuración o registros.");
    }

    private void EnsureDataDirectory()
    {
        string parent = Path.GetDirectoryName(_dataDirectory)!;
        SafePath.EnsureNoReparsePoints(_serverRoot, _dataDirectory);
        Directory.CreateDirectory(parent);
        Directory.CreateDirectory(_backupDirectory);
        SafePath.EnsureNoReparsePoints(_serverRoot, _backupDirectory);
    }

    private void WriteAudit(string actor, string action, string detail)
    {
        EnsureDataDirectory();
        string safeActor = (actor ?? "unknown").Replace('\r', ' ').Replace('\n', ' ');
        string safeDetail = (detail ?? string.Empty).Replace('\r', ' ').Replace('\n', ' ');
        SafePath.EnsureNoReparsePoints(_serverRoot, _auditPath);
        string entry = $"{DateTime.UtcNow:O}\t{safeActor}\t{action}\t{safeDetail}{Environment.NewLine}";
        lock (_auditLock)
        {
            long currentBytes = File.Exists(_auditPath) ? new FileInfo(_auditPath).Length : 0;
            if (Encoding.UTF8.GetByteCount(entry) > MaximumAuditBytes - currentBytes)
                throw new IOException("El registro de auditoría alcanzó 5 MiB; archívalo desde el host antes de continuar.");
            File.AppendAllText(_auditPath, entry, new UTF8Encoding(false));
        }
    }

    private static string ReadUtf8(string path, out bool hasBom, long maximumBytes)
    {
        byte[] bytes;
        using (var input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (var output = new MemoryStream())
        {
            if (input.Length > maximumBytes)
                throw new IOException("El archivo superó el tamaño máximo permitido.");

            byte[] buffer = new byte[8192];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                if (output.Length + read > maximumBytes)
                    throw new IOException("El archivo superó el tamaño máximo permitido durante la lectura.");
                output.Write(buffer, 0, read);
            }

            bytes = output.ToArray();
        }

        hasBom = bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF;
        int offset = hasBom ? 3 : 0;
        return new UTF8Encoding(false, true).GetString(bytes, offset, bytes.Length - offset);
    }

    private static void CopyFileBounded(string source, string destination, long maximumBytes)
    {
        using var input = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var output = new FileStream(destination, FileMode.CreateNew, FileAccess.Write, FileShare.None, 8192, FileOptions.WriteThrough);
        if (input.Length > maximumBytes)
            throw new IOException("El archivo superó el tamaño máximo permitido.");

        byte[] buffer = new byte[8192];
        int read;
        long copied = 0;
        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
        {
            copied += read;
            if (copied > maximumBytes)
                throw new IOException("El archivo superó el tamaño máximo permitido durante la copia.");
            output.Write(buffer, 0, read);
        }

        output.Flush(flushToDisk: true);
    }

    private static byte[] EncodeUtf8(string content, bool withBom)
    {
        byte[] text = new UTF8Encoding(false, true).GetBytes(content);
        if (!withBom)
            return text;

        byte[] bytes = new byte[text.Length + 3];
        bytes[0] = 0xEF;
        bytes[1] = 0xBB;
        bytes[2] = 0xBF;
        Buffer.BlockCopy(text, 0, bytes, 3, text.Length);
        return bytes;
    }

    private static void AtomicWrite(string path, byte[] bytes)
    {
        string tempPath = Path.Combine(Path.GetDirectoryName(path)!, $".{Path.GetFileName(path)}.{Guid.NewGuid():N}.tmp");
        try
        {
            using (var stream = new FileStream(tempPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, FileOptions.WriteThrough))
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush(flushToDisk: true);
            }

            ReplaceExistingFile(tempPath, path);
        }
        finally
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }

    private static void ReplaceExistingFile(string tempPath, string targetPath)
    {
        if (File.Exists(targetPath))
        {
            File.Replace(tempPath, targetPath, destinationBackupFileName: null);
            return;
        }

        File.Move(tempPath, targetPath);
    }

    private static string ComputeSha256(string path)
    {
        using var stream = File.OpenRead(path);
        using var sha = SHA256.Create();
        return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", string.Empty).ToLowerInvariant();
    }

    private static string Redact(string content)
    {
        string newline = content.IndexOf("\r\n", StringComparison.Ordinal) >= 0 ? "\r\n" : "\n";
        string[] lines = Regex.Split(content, "\\r\\n|\\n|\\r");
        int? blockIndent = null;
        for (int i = 0; i < lines.Length; i++)
        {
            string current = lines[i];
            int indent = 0;
            while (indent < current.Length && current[indent] == ' ')
                indent++;

            if (blockIndent.HasValue)
            {
                if (string.IsNullOrWhiteSpace(current) || indent > blockIndent.Value)
                {
                    lines[i] = "[REDACTED]";
                    continue;
                }

                blockIndent = null;
            }

            Match secret = SecretLine.Match(current);
            if (secret.Success)
            {
                string scalar = secret.Groups[3].Value.Trim();
                if (YamlBlockScalarIndicator.IsMatch(scalar))
                    blockIndent = indent;
                current = current.Substring(0, secret.Groups[3].Index) + "[REDACTED]";
            }

            current = SensitiveInline.Replace(current, match =>
            {
                if (match.Value.StartsWith("Bearer", StringComparison.OrdinalIgnoreCase))
                    return "Bearer [REDACTED]";
                return match.Groups[1].Value + "[REDACTED]";
            });
            lines[i] = Ipv4.Replace(current, "[IP]");
        }

        return string.Join(newline, lines);
    }

    private static string FormatBytes(long bytes)
    {
        string[] units = ["B", "KB", "MB", "GB", "TB"];
        double value = bytes;
        int unit = 0;
        while (value >= 1024 && unit < units.Length - 1)
        {
            value /= 1024;
            unit++;
        }

        return $"{value:0.##} {units[unit]}";
    }

    private static string SanitizeMetadata(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "no declarada";

        string sanitized = new(value.Where(character => !char.IsControl(character)).ToArray());
        return sanitized.Length <= 160 ? sanitized : sanitized.Substring(0, 160) + "...";
    }

    private static int Clamp(int value, int minimum, int maximum) =>
        value < minimum ? minimum : value > maximum ? maximum : value;
}
