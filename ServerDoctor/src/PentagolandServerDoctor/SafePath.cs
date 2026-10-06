using System;
using System.IO;
using System.Linq;

namespace Pentagoland.ServerDoctor;

public static class SafePath
{
    public static string ResolveFile(AccessRoot root, string relativePath, bool mustExist)
    {
        if (string.IsNullOrWhiteSpace(relativePath) || Path.IsPathRooted(relativePath))
            throw new ArgumentException("La ruta debe ser relativa a una raíz autorizada.");
        if (!Directory.Exists(root.Path))
            throw new DirectoryNotFoundException($"La raíz '{root.Name}' no existe.");

        string normalized = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        if (normalized.Split(Path.DirectorySeparatorChar).Any(part => part == ".." || part.Length == 0))
            throw new ArgumentException("La ruta contiene segmentos no permitidos.");

        string fullPath = Path.GetFullPath(Path.Combine(root.Path, normalized));
        EnsureContained(root.Path, fullPath);
        EnsureNoReparsePoints(root.Path, fullPath);

        if (mustExist && !File.Exists(fullPath))
            throw new FileNotFoundException("No se encontró el archivo indicado.", relativePath);

        return fullPath;
    }

    public static string ResolveDirectory(AccessRoot root, string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath) || relativePath == ".")
        {
            if (!Directory.Exists(root.Path))
                throw new DirectoryNotFoundException($"La raíz '{root.Name}' no existe.");
            EnsureNoReparsePoints(root.Path, root.Path);
            return root.Path;
        }

        if (Path.IsPathRooted(relativePath))
            throw new ArgumentException("La ruta debe ser relativa a una raíz autorizada.");
        if (!Directory.Exists(root.Path))
            throw new DirectoryNotFoundException($"La raíz '{root.Name}' no existe.");

        string normalized = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        if (normalized.Split(Path.DirectorySeparatorChar).Any(part => part == ".." || part.Length == 0))
            throw new ArgumentException("La ruta contiene segmentos no permitidos.");

        string fullPath = Path.GetFullPath(Path.Combine(root.Path, normalized));
        EnsureContained(root.Path, fullPath);
        EnsureNoReparsePoints(root.Path, fullPath);
        return fullPath;
    }

    public static void EnsureNoReparsePoints(string target)
    {
        string fullTarget = Path.GetFullPath(target);
        string volumeRoot = Path.GetPathRoot(fullTarget)
            ?? throw new ArgumentException("No se pudo determinar la raíz de la ruta.", nameof(target));
        string current = volumeRoot;
        if (File.Exists(current) || Directory.Exists(current))
            CheckReparsePoint(current);

        string relative = fullTarget.Substring(volumeRoot.Length);
        foreach (string part in relative.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries))
        {
            current = Path.Combine(current, part);
            if (File.Exists(current) || Directory.Exists(current))
                CheckReparsePoint(current);
        }
    }

    public static void EnsureNoReparsePoints(string root, string target)
    {
        EnsureNoReparsePoints(root);
        EnsureNoReparsePoints(root, target, true);
    }

    private static void EnsureContained(string root, string target)
    {
        string fullRoot = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        string prefix = fullRoot + Path.DirectorySeparatorChar;
        if (!target.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("La ruta queda fuera de la raíz autorizada.");
    }

    private static void EnsureNoReparsePoints(string root, string target, bool checkTarget)
    {
        string fullRoot = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        string current = fullRoot;
        CheckReparsePoint(current);

        string relative = target.Substring(fullRoot.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        string[] parts = relative.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < parts.Length; i++)
        {
            current = Path.Combine(current, parts[i]);
            if ((checkTarget || i < parts.Length - 1) && (File.Exists(current) || Directory.Exists(current)))
                CheckReparsePoint(current);
        }
    }

    private static void CheckReparsePoint(string path)
    {
        FileAttributes attributes = File.GetAttributes(path);
        if ((attributes & FileAttributes.ReparsePoint) != 0)
            throw new UnauthorizedAccessException("No se permiten enlaces simbólicos ni puntos de reanálisis.");
    }
}
