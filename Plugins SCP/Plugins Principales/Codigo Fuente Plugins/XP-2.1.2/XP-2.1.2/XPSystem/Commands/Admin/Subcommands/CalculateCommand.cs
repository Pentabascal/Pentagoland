namespace XPSystem.Commands.Admin.Subcommands
{
    using System;
    using System.Diagnostics;
    using CommandSystem;
    using XPSystem.API;

    public class CalculateCommand : SanitizedInputCommand
    {
        public override bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermissionLS("xps.calculate"))
            {
                response = "No tienes permiso (xps.calculate) para ejecutar este comando.";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = "Uso: xp calculate <level|xp> <value>";
                return false;
            }

            if (!int.TryParse(arguments.At(1), out int value))
            {
                response = "Valor no válido.";
                return false;
            }

            Stopwatch stopwatch = new();
            switch (arguments.At(0).ToLower())
            {
                case "l":
                case "lvl":
                case "level":
                    stopwatch.Start();
                    int xp;
                    try
                    {
                        xp = LevelCalculator.GetLevel(value, true);
                    }
                    catch (Exception e)
                    {
                        response = "No se pudo calcular el nivel (LevelFunction): " + e;
                        return false;
                    }

                    stopwatch.Stop();
                    response = $"{value} XP corresponde al nivel {xp} (calculado en {stopwatch.ElapsedMilliseconds} ms).";
                    return true;
                case "xp":
                    stopwatch.Start();
                    int level;
                    try
                    {
                        level = LevelCalculator.GetXP(value, true);
                    }
                    catch (Exception e)
                    {
                        response = "No se pudieron calcular los XP (XPFunction): " + e;
                        return false;
                    }

                    stopwatch.Stop();
                    response = $"El nivel {value} requiere {level} XP (calculado en {stopwatch.ElapsedMilliseconds} ms).";
                    return true;
                default:
                    response = "Uso: xp calculate <level|xp> <value>";
                    return false;
            }
        }

        public override string Command { get; } = "calculate";
        public override string[] Aliases { get; } = { "calc", "c" };
        public override string Description { get; } = "Calcula niveles o cantidades de XP.";
    }
}