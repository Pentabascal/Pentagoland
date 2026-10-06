using CommandSystem;
using Hints;
using LabApi.Features.Wrappers;
using SwiftArcadeMode.Features;
using System;

namespace SwiftArcadeMode.Commands.Client
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ClientSeeUpgradesCommand : ICommand
    {
        public string Command => "seeupgrades";

        public string[] Aliases => ["sup", "su"];

        public string Description => "Muestra todas las opciones de mejora que tienes.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player p = Player.Get(sender);

            if (p.TryGetPerkInventory(out PerkInventory inv))
            {
                response = inv.UpgradeQueue.Peek(out string brief);
                p.SendHint("<align=\"left\">" + brief + "</align>", [HintEffectPresets.FadeOut()], 10f);
                return true;
            }

            response = "No hay mejoras disponibles.";
            p.SendHint($"<align=\"left\">{response}</align>", [HintEffectPresets.FadeOut()], 2f);
            return false;
        }
    }
}
