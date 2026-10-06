using System;

namespace Exiled.API.Features
{
    public static class Log
    {
        public static void Info(string message) => Console.WriteLine($"[INFO] {message}");
        public static void Warn(string message) => Console.WriteLine($"[WARN] {message}");
        public static void Error(string message) => Console.WriteLine($"[ERROR] {message}");
        public static void Debug(string message) => Console.WriteLine($"[DEBUG] {message}");
    }
}
