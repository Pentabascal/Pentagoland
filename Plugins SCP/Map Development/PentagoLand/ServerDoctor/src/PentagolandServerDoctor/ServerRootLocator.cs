using System;
using System.IO;

namespace Pentagoland.ServerDoctor;

public static class ServerRootLocator
{
    public static string Locate(string currentDirectory, string applicationBase, string configuredRoot)
    {
        if (!string.IsNullOrWhiteSpace(configuredRoot))
        {
            string configuredPath = Path.IsPathRooted(configuredRoot)
                ? configuredRoot
                : Path.Combine(currentDirectory, configuredRoot);
            string fullConfiguredPath = Path.GetFullPath(configuredPath);
            if (!Directory.Exists(fullConfiguredPath))
                throw new DirectoryNotFoundException($"ServerRootPath no existe: {fullConfiguredPath}");
            return fullConfiguredPath;
        }

        string? installationRoot = FindInstallationRoot(currentDirectory) ??
                                  FindInstallationRoot(applicationBase);
        string fallback = Path.GetFullPath(currentDirectory);
        return installationRoot ?? fallback;
    }

    private static string? FindInstallationRoot(string candidate)
    {
        if (string.IsNullOrWhiteSpace(candidate))
            return null;

        DirectoryInfo? current = new(Path.GetFullPath(candidate));
        for (int depth = 0; current is not null && depth < 8; depth++, current = current.Parent)
        {
            string managedDirectory = Path.Combine(current.FullName, "SCPSL_Data", "Managed");
            if (Directory.Exists(managedDirectory))
                return current.FullName;
        }

        return null;
    }
}
