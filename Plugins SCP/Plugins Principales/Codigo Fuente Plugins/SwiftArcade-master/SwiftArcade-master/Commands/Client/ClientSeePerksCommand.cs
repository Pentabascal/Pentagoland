using CommandSystem;
using Hints;
using LabApi.Features.Wrappers;
using SwiftArcadeMode.Features;
using System;
using System.Text;

namespace SwiftArcadeMode.Commands.Client
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ClientSeePerksCommand : ICommand
    {
        public string Command => "seeperks";

        public string[] Aliases => ["sperk", "sp"];

        public string Description => "Muestra todas las ventajas que tienes.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player p = Player.Get(sender);

            if (!PerkManager.Inventories.ContainsKey(p) || PerkManager.Inventories[p].Perks.Count <= 0)
            {
                response = "\n\nNo tienes ninguna ventaja.";
                return true;
            }

            response = GetPerks(p, out string b);
            p.SendHint(b, [HintEffectPresets.FadeOut()], 10f);
            return true;
        }

        public static string GetPerks(Player p, out string brief)
        {
            if (!PerkManager.Inventories.ContainsKey(p))
            {
                brief = "No tienes ninguna ventaja.";
                return brief;
            }

            StringBuilder stringBuilder = new("\n\n");
            StringBuilder hint = new("<align=\"left\">Ventajas actuales: \n");

            foreach (PerkBase perk in PerkManager.Inventories[p].Perks)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append(perk.FancyName);
                hint.Append(perk.FancyName);
                hint.AppendLine();
                stringBuilder.Append(" - ");
                stringBuilder.AppendLine(perk.Description);
            }

            hint.Append("</align>");

            brief = hint.ToString();
            return stringBuilder.ToString();
        }
    }
}
