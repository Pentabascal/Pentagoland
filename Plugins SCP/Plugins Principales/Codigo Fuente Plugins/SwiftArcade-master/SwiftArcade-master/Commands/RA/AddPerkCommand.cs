using CommandSystem;
using LabApi.Features.Wrappers;
using SwiftArcadeMode.Features;
using System;

namespace SwiftArcadeMode.Commands.RA
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class AddPerkCommand : ICommand
    {
        public string Command => "addperk";

        public string[] Aliases => ["aperk", "addp", "ap"];

        public string Description => "Añade una ventaja a ti o a otro jugador.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.Effects))
            {
                response = "¡No tienes permiso!";
                return false;
            }

            Player p = Player.Get(sender);

            if (arguments.Array.Length < 2 || !PerkManager.TryGetPerk(arguments.Array[1].ToLower(), out PerkAttribute t))
            {
                response = "¡No se encontró esa ventaja!";
                return false;
            }

            if (arguments.Array.Length > 2)
                p = int.TryParse(arguments.Array[2], out int id) ? Player.Get(id) : Player.Get(arguments.Array[2]);

            p.GivePerk(t);

            response = "Ventaja añadida: " + t.ID + " a " + p.Nickname;
            return true;
        }
    }
}
