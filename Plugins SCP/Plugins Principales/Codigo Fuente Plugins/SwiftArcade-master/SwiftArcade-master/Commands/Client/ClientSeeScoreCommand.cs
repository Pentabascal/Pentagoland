using CommandSystem;
using LabApi.Features.Wrappers;
using SwiftArcadeMode.Features.Scoring;
using System;

namespace SwiftArcadeMode.Commands.Client
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ClientSeeScoreCommand : ICommand
    {
        public string Command => "seescore";

        public string[] Aliases => ["ss", "sscore"];

        public string Description => "Muestra tu puntuación actual.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player p = Player.Get(sender);

            response = $"Puntuación actual: {p.GetScore()}";
            return true;
        }
    }
}
