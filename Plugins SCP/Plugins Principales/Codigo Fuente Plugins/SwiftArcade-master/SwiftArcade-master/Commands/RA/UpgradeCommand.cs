using CommandSystem;
using LabApi.Features.Wrappers;
using SwiftArcadeMode.Features;
using SwiftArcadeMode.Features.SCPs.Upgrades;
using System;

namespace SwiftArcadeMode.Commands.RA
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class UpgradeCommand : ICommand
    {
        public string Command => "upgradeperk";

        public string[] Aliases => ["uperk", "upgradep", "up"];

        public string Description => "Mejora una rama de mejoras para ti o para otro jugador.";

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

            if (!p.TryGetPerkInventory(out PerkInventory inv) || !inv.TryGetPerk(t.Perk, out PerkBase perk) || perk is not UpgradePathPerkBase upg)
            {
                response = "¡No se encontró la rama de mejoras!";
                return false;
            }

            upg.Progress++;

            response = "Ventaja mejorada: " + t.ID + " para " + p.Nickname;
            return true;
        }
    }
}
