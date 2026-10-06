using CommandSystem;
using LabApi.Features.Wrappers;
using SwiftArcadeMode.Features;
using System;

namespace SwiftArcadeMode.Commands.Client
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ClientChooseUpgradeCommand : ICommand
    {
        public string Command => "chooseupgrade";

        public string[] Aliases => ["choose", "c"];

        public string Description => "Elige una mejora.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player p = Player.Get(sender);

            if (arguments.Array.Length < 2 || !int.TryParse(arguments.Array[1], out int num))
            {
                response = "¡Introduce un número!";
                return false;
            }

            if (p.TryGetPerkInventory(out PerkInventory inv) && inv.UpgradeQueue.Upgrades.Count > 0)
            {
                bool success = inv.UpgradeQueue.Choose(num - 1, out string name);

                response = success ? ("Mejora elegida: " + name + (inv.UpgradeQueue.Upgrades.Count > 0 ? ". Quedan " + inv.UpgradeQueue.Upgrades.Count + " opciones de mejora." : "")) : "Índice no válido.";
                return success;
            }

            response = "No hay mejoras disponibles.";
            return false;
        }
    }
}
