using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
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
            process.Refresh();
            TimeSpan uptime = DateTime.UtcNow - process.StartTime.ToUniversalTime();
            long residentBytes;
            long privateBytes;
            bool hasNativeMetrics = TryGetWindowsMemory(out residentBytes, out privateBytes);
            if (!hasNativeMetrics)
            {
                residentBytes = process.WorkingSet64;
                privateBytes = process.PrivateMemorySize64;
            }

            if (residentBytes <= 0)
            {
                residentBytes = Environment.WorkingSet;
            }

            lines.Add($"Proceso: {process.ProcessName} | activo {uptime.Days}d {uptime.Hours}h {uptime.Minutes}m");
            lines.Add($"Memoria administrada: {FormatBytes(GC.GetTotalMemory(forceFullCollection: false))}");
            lines.Add($"Memoria residente: {FormatBytesIfAvailable(residentBytes)}");
            lines.Add($"Memoria privada: {FormatBytesIfAvailable(privateBytes)}");
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

    private static bool TryGetWindowsMemory(out long residentBytes, out long privateBytes)
    {
        residentBytes = 0;
        privateBytes = 0;

        if (Environment.OSVersion.Platform != PlatformID.Win32NT)
        {
            return false;
        }

        try
        {
            var counters = new ProcessMemoryCountersEx
            {
                Size = (uint)Marshal.SizeOf(typeof(ProcessMemoryCountersEx))
            };

            if (!GetProcessMemoryInfo(GetCurrentProcess(), out counters, counters.Size))
            {
                return false;
            }

            residentBytes = unchecked((long)counters.WorkingSetSize.ToUInt64());
            privateBytes = unchecked((long)counters.PrivateUsage.ToUInt64());
            return residentBytes > 0 && privateBytes > 0;
        }
        catch (DllNotFoundException)
        {
            return false;
        }
        catch (EntryPointNotFoundException)
        {
            return false;
        }
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

    private static string FormatBytesIfAvailable(long bytes) =>
        bytes > 0 ? FormatBytes(bytes) : "no disponible";

    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern IntPtr GetCurrentProcess();

    [DllImport("psapi.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetProcessMemoryInfo(
        IntPtr process,
        out ProcessMemoryCountersEx counters,
        uint size);

    [StructLayout(LayoutKind.Sequential)]
    private struct ProcessMemoryCountersEx
    {
        public uint Size;
        public uint PageFaultCount;
        public UIntPtr PeakWorkingSetSize;
        public UIntPtr WorkingSetSize;
        public UIntPtr QuotaPeakPagedPoolUsage;
        public UIntPtr QuotaPagedPoolUsage;
        public UIntPtr QuotaPeakNonPagedPoolUsage;
        public UIntPtr QuotaNonPagedPoolUsage;
        public UIntPtr PagefileUsage;
        public UIntPtr PeakPagefileUsage;
        public UIntPtr PrivateUsage;
    }
}
