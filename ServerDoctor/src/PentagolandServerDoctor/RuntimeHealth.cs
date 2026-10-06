using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using LabApi.Features;
using LabApi.Features.Wrappers;

namespace Pentagoland.ServerDoctor;

public static class RuntimeHealth
{
    public static IReadOnlyList<string> Snapshot()
    {
        var lines = new List<string>
        {
            $"LabAPI: {LabApiProperties.CompiledVersion}",
            $"Puerto: {Server.Port}",
            $"Jugadores: {Server.PlayerCount}/{Server.MaxPlayers}",
            $"TPS: {FormatTps(Server.Tps)}/{Server.MaxTps}"
        };

        try
        {
            using var process = Process.GetCurrentProcess();
            TimeSpan uptime = DateTime.UtcNow - process.StartTime.ToUniversalTime();
            lines.Add($"Proceso: {process.ProcessName} | activo {uptime.Days}d {uptime.Hours}h {uptime.Minutes}m");
            lines.Add($"Memoria administrada: {FormatBytes(GC.GetTotalMemory(forceFullCollection: false))}");
            lines.Add($"Memoria residente: {FormatBytes(process.WorkingSet64)}");
            lines.Add($"Memoria privada: {FormatBytes(process.PrivateMemorySize64)}");
        }
        catch (Exception exception) when (
            exception is Win32Exception ||
            exception is InvalidOperationException ||
            exception is PlatformNotSupportedException ||
            exception is UnauthorizedAccessException)
        {
            lines.Add($"Métricas del proceso no disponibles: {exception.Message}");
        }

        return lines;
    }

    private static string FormatTps(double tps) =>
        double.IsNaN(tps) || double.IsInfinity(tps)
            ? "no disponible"
            : tps.ToString("0.##", CultureInfo.InvariantCulture);

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
}
