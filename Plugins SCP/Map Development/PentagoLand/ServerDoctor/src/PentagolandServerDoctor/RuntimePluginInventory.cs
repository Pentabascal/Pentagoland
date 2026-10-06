using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LabApi.Loader;

namespace Pentagoland.ServerDoctor;

public static class RuntimePluginInventory
{
    private const int MaximumEntries = 100;

    public static IReadOnlyList<string> Snapshot()
    {
        if (!PluginLoader.Initialized)
            return ["LabAPI aún no ha inicializado el cargador de plugins."];

        var plugins = PluginLoader.Plugins
            .Select(entry => new
            {
                entry.Key,
                Identity = entry.Value.GetName()
            })
            .OrderBy(entry => entry.Identity.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var dependencies = PluginLoader.Dependencies
            .Select(assembly => new { Assembly = assembly, Identity = assembly.GetName() })
            .OrderBy(entry => entry.Identity.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var lines = new List<string>
        {
            "Estado consultado desde el cargador de LabAPI; no se inspecciona código de terceros.",
            $"Plugins conocidos: {plugins.Length}"
        };

        foreach (var entry in plugins.Take(MaximumEntries))
        {
            string status = PluginLoader.EnabledPlugins.Contains(entry.Key) ? "HABILITADO" : "NO HABILITADO";
            string name = Sanitize(entry.Identity.Name ?? "ensamblado sin nombre");
            string version = Sanitize(entry.Identity.Version?.ToString() ?? "versión no declarada");
            string fileName = Sanitize(Path.GetFileName(entry.Key.FilePath));
            lines.Add($"[{status}] {name} {version}{(fileName.Length == 0 ? string.Empty : $" | {fileName}")}");
        }

        if (plugins.Length > MaximumEntries)
            lines.Add($"Se omitieron {plugins.Length - MaximumEntries} plugins para limitar la respuesta.");
        if (plugins.Length == 0)
            lines.Add("LabAPI no registra plugins.");

        lines.Add($"Dependencias cargadas: {dependencies.Length}");
        foreach (var entry in dependencies.Take(MaximumEntries))
        {
            string name = Sanitize(entry.Identity.Name ?? "ensamblado sin nombre");
            string version = Sanitize(entry.Identity.Version?.ToString() ?? "versión no declarada");
            lines.Add($"[DEPENDENCIA] {name} {version}");
        }

        if (dependencies.Length > MaximumEntries)
            lines.Add($"Se omitieron {dependencies.Length - MaximumEntries} dependencias para limitar la respuesta.");
        if (dependencies.Length == 0)
            lines.Add("LabAPI no registra dependencias cargadas.");

        return lines;
    }

    private static string Sanitize(string value)
    {
        string sanitized = new(value.Where(character => !char.IsControl(character)).ToArray());
        return sanitized.Length <= 160 ? sanitized : sanitized.Substring(0, 160) + "...";
    }
}
