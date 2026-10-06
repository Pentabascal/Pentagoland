namespace XPSystem.Commands.Admin.Subcommands
{
    using System;
    using CommandSystem;
    using XPSystem.API;
    using XPSystem.API.Player;

    public class MultiplierCommand : SanitizedInputCommand
    {
        public override bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermissionLS("xp.setmultiplier"))
            {
                response = "No tienes permiso para ejecutar este comando.";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = "Uso: xp setmultiplier global (multiplier) o xp setmultiplier player (player) (multiplier)";
                return false;
            }

            float multiplier;
            switch (arguments.At(0).ToLower())
            {
                case "g":
                case "global":
                    if (!float.TryParse(arguments.At(1), out multiplier))
                    {
                        response = $"Multiplicador global: {XPAPI.Config.GlobalXPMultiplier}.";
                        return true;
                    }

                    XPAPI.Config.GlobalXPMultiplier = multiplier;
                    response = $"Multiplicador global de XP establecido temporalmente en {multiplier}.";
                    return true;
                case "p":
                case "player":
                    BaseXPPlayer? basePlayer;
                    int intArgLocation;

                    if (arguments.Count > 2)
                    {
                        intArgLocation = 2;
                        if (!XPPlayer.TryGet(arguments.At(1), out basePlayer))
                        {
                            response = "Jugador no válido.";
                            return false;
                        }
                    }
                    else if (XPPlayer.TryGet(sender, out basePlayer))
                    {
                        intArgLocation = 1;
                    }
                    else
                    {
                        response = "Debes indicar un jugador.";
                        return false;
                    }

                    if (basePlayer is not XPPlayer player)
                    {
                        response = "El jugador no pertenece a XPPlayer.";
                        return false;
                    }

                    if (!float.TryParse(arguments.At(intArgLocation), out multiplier))
                    {
                        response = $"Multiplicador de {player.DisplayedName}: {player.XPMultiplier}.";
                        return false;
                    }

                    player.XPMultiplier = multiplier;
                    response = $"Multiplicador de {player.DisplayedName} establecido en {multiplier}.";
                    return true;
                default:
                    response = "Objetivo no válido.";
                    return false;
            }
        }

        public override string Command { get; } = "multiplier";
        public override string[] Aliases { get; } = new[] { "mult" };
        public override string Description { get; } = "Consulta o establece los multiplicadores de XP.";
    }
}