#if !EXILED
using Callvote.Commands.ParentCommands;
#endif
using System;
using CommandSystem;

namespace Callvote.Commands
{
#if !EXILED
    [CommandHandler(typeof(CallVoteParentCommand))]
#endif
    public class HelpCommand : ICommand
    {
        public string Command => "help";

        public string[] Aliases => [];

        public string Description => "Muestra cómo usar Callvote.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response =
                "\ncallvote custom [\"pregunta\"] [comando1(detalle1)] [comando2(detalle2)]...." +
                "\ncallvote binary [pregunta]" +
                "\ncallvote kick [jugador] [motivo]" +
                "\ncallvote kill [jugador] [motivo]" +
                "\ncallvote nuke" +
                "\ncallvote respawnwave" +
                "\ncallvote restartround" +
                "\ncallvote stopvote" +
                "\ncallvote ff" +
                "\ncallvote queue [check/clear/pause/removeplayer/removetype/removeindex] [jugador/tipo/índice]" +
                "\ncallvote rig [opción]" +
                "\ncallvote translation [none/idioma/código de país]" +
                "\n<color=red>QUITA LOS CORCHETES (-> []) AL USAR EL COMANDO</color>";
            return true;
        }
    }
}