using CommandSystem;
using System;

namespace AdminTools.Commands.HintBroadcast
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class HintBroadcast : ParentCommand
    {
        public HintBroadcast() => LoadGeneratedCommands();

        public override string Command { get; } = "hintbroadcast";
        public override string[] Aliases { get; } = new string[] { "hint" , "hbc" };
        public override string Description { get; } = "Envía un mensaje a un jugador, un grupo, un rol, todo el personal o a todos";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new Clear());
            RegisterCommand(new Group());
            RegisterCommand(new User());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response) // TODO: Make it ParentCommand
        {
            response = "Subcomando no válido. Subcomandos disponibles: user, group, clear";
            return false;
        }
    }
}
