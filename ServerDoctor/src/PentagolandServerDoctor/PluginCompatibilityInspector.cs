using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace Pentagoland.ServerDoctor;

public static class PluginCompatibilityInspector
{
    private const int MaximumReferencesToReport = 100;
    private const int MaximumAssemblyPathsToInspect = 5000;
    private static readonly HashSet<string> FrameworkAssemblies = new(StringComparer.OrdinalIgnoreCase)
    {
        "mscorlib", "netstandard", "System", "System.Core", "System.Data", "System.Drawing",
        "System.Numerics", "System.Runtime", "System.Runtime.Serialization", "System.Security",
        "System.ServiceModel", "System.Transactions", "System.Web", "System.Xml", "System.Xml.Linq",
        "System.Configuration", "Microsoft.CSharp", "Microsoft.VisualBasic"
    };

    public static IReadOnlyList<string> Analyze(string pluginPath, IEnumerable<string> availableAssemblyPaths)
    {
        using var stream = new FileStream(pluginPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var peReader = new PEReader(stream);
        if (!peReader.HasMetadata)
            throw new BadImageFormatException("El archivo no contiene metadatos de ensamblado .NET.", pluginPath);

        MetadataReader metadata = peReader.GetMetadataReader();
        if (!metadata.IsAssembly)
            throw new BadImageFormatException("El módulo no declara una identidad de ensamblado .NET.", pluginPath);

        AssemblyDefinition definition = metadata.GetAssemblyDefinition();
        string assemblyName = metadata.GetString(definition.Name);
        Version assemblyVersion = definition.Version;
        Dictionary<string, List<AssemblyName>> available = BuildAvailableAssemblyMap(
            availableAssemblyPaths,
            out int unreadableAssemblyCount,
            out bool assemblyPathLimitReached);
        var lines = new List<string>
        {
            $"Ensamblado .NET: {assemblyName}, versión {assemblyVersion}",
            "Revisión estática de referencias; no carga, inicializa ni ejecuta el plugin."
        };

        var references = metadata.AssemblyReferences
            .Select(handle => metadata.GetAssemblyReference(handle))
            .Select(reference => new
            {
                Name = metadata.GetString(reference.Name),
                reference.Version
            })
            .OrderBy(reference => reference.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        int missingCount = 0;
        int platformUnverifiedCount = 0;
        int versionDifferenceCount = 0;
        int omittedCount = Math.Max(0, references.Length - MaximumReferencesToReport);
        foreach (var reference in references.Take(MaximumReferencesToReport))
        {
            if (!available.TryGetValue(reference.Name, out List<AssemblyName>? candidates) || candidates.Count == 0)
            {
                if (FrameworkAssemblies.Contains(reference.Name))
                {
                    lines.Add($"PLATAFORMA PROBABLE/NO VERIFICADA: {reference.Name}, referencia {reference.Version}.");
                    platformUnverifiedCount++;
                }
                else
                {
                    lines.Add($"NO ENCONTRADA EN RUTAS REVISADAS: {reference.Name}, referencia {reference.Version}.");
                    missingCount++;
                }

                continue;
            }

            AssemblyName? matching = candidates.FirstOrDefault(candidate => candidate.Version == reference.Version);
            if (matching is not null)
            {
                lines.Add($"COINCIDE POR NOMBRE/VERSIÓN: {reference.Name}, versión {matching.Version}; firma e identidad completa no verificadas.");
                continue;
            }

            AssemblyName closest = candidates
                .OrderBy(candidate => VersionDistance(candidate.Version, reference.Version))
                .First();
            lines.Add($"VERSIÓN DISTINTA: {reference.Name}, plugin {reference.Version}, encontrada {closest.Version}; valida la compatibilidad.");
            versionDifferenceCount++;
        }

        if (omittedCount > 0)
            lines.Add($"Se omitieron {omittedCount} referencias adicionales para limitar la respuesta.");
        if (assemblyPathLimitReached)
            lines.Add($"El inventario de DLL se limitó a {MaximumAssemblyPathsToInspect} rutas.");
        if (unreadableAssemblyCount > 0)
            lines.Add($"{unreadableAssemblyCount} DLL candidatas no se pudieron leer como ensamblados .NET.");
        lines.Add($"Resumen: {missingCount} referencias no encontradas en las rutas examinadas; {platformUnverifiedCount} probables referencias de plataforma no verificadas; {versionDifferenceCount} con versión distinta.");
        lines.Add("El resultado no certifica compatibilidad ABI/API, clave pública, arquitectura, dependencias dinámicas, configuración, orden de carga ni interacción entre plugins. Confirma en logs y en un servidor de pruebas.");
        return lines;
    }

    private static Dictionary<string, List<AssemblyName>> BuildAvailableAssemblyMap(
        IEnumerable<string> paths,
        out int unreadableAssemblyCount,
        out bool pathLimitReached)
    {
        var available = new Dictionary<string, List<AssemblyName>>(StringComparer.OrdinalIgnoreCase);
        unreadableAssemblyCount = 0;
        pathLimitReached = false;
        int inspectedPathCount = 0;
        foreach (string path in paths.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (inspectedPathCount >= MaximumAssemblyPathsToInspect)
            {
                pathLimitReached = true;
                break;
            }
            inspectedPathCount++;

            try
            {
                AssemblyName assembly = AssemblyName.GetAssemblyName(path);
                string name = assembly.Name ?? Path.GetFileNameWithoutExtension(path);
                if (!available.TryGetValue(name, out List<AssemblyName>? entries))
                {
                    entries = new List<AssemblyName>();
                    available.Add(name, entries);
                }
                entries.Add(assembly);
            }
            catch (BadImageFormatException)
            {
                unreadableAssemblyCount++;
            }
            catch (FileLoadException)
            {
                unreadableAssemblyCount++;
            }
            catch (IOException)
            {
                unreadableAssemblyCount++;
            }
            catch (UnauthorizedAccessException)
            {
                unreadableAssemblyCount++;
            }
        }

        return available;
    }

    private static long VersionDistance(Version? left, Version right)
    {
        if (left is null)
            return long.MaxValue;
        return Math.Abs((long)left.Major - right.Major) * 1_000_000L +
               Math.Abs((long)left.Minor - right.Minor) * 10_000L +
               Math.Abs((long)Math.Max(0, left.Build) - Math.Max(0, right.Build)) * 100L +
               Math.Abs((long)Math.Max(0, left.Revision) - Math.Max(0, right.Revision));
    }
}
