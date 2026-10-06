namespace XPSystem.Commands.Admin
{
    using System;
    using System.Linq;
    using CommandSystem;
    using XPSystem.Commands.Console.Subcommands;

    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class ConsoleParentCommand : ParentCommand
    {
        public ConsoleParentCommand() => LoadGeneratedCommands();

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new DeleteEverythingCommand());
            RegisterCommand(new MigrateCommand());
            RegisterCommand(new ReloadConfigsCommand());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = $"Uso: xp ({string.Join(" | ", Commands.Select(x => x.Key))})";
            return false;
        }

        public override string Command { get; } = "xp";
        public override string[] Aliases { get; } = new[] { "xps" };
        public override string Description { get; } = "Comando principal de XPSystem para la consola.";
    }
}