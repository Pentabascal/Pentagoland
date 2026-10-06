namespace PlaceholderAPI.Commands
{
    using System;
    using CommandSystem;
    using PlaceholderAPI.Commands.Sub;

    /// <summary>
    /// Main Command for PAPI.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class PAPICommand : ParentCommand, IUsageProvider
    {
        /// <inheritdoc/>
        public override string Command => "papi";

        /// <inheritdoc/>
        public override string[] Aliases => new string[] { };

        /// <inheritdoc/>
        public override string Description => "Comando principal de PlaceholderAPI";

        /// <inheritdoc/>
        public string[] Usage => ["ecloud/credits/parse", "name/string"];

        /// <summary>
        /// Initializes a new instance of the <see cref="PAPICommand"/> class.
        /// Main Command.
        /// </summary>
        public PAPICommand() => this.LoadGeneratedCommands();

        /// <inheritdoc/>
        public override void LoadGeneratedCommands()
        {
            this.RegisterCommand(new CreditsSubCommand());
            this.RegisterCommand(new ECloudSubCommand());
            this.RegisterCommand(new TestTagsSubCommand());
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = $"¡Bienvenido!\nPlaceholderAPI ({PlaceholderAPIPlugin.Instance.Version}v), creado por {PlaceholderAPIPlugin.Instance.Author}\nUso:\bpapi ecloud Name\npapi credits\npapi parse Tags";
            return true;
        }
    }
}
