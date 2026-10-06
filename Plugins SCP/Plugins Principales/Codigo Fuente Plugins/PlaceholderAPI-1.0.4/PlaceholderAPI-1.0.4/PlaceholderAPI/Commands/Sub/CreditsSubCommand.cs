namespace PlaceholderAPI.Commands.Sub
{
    using System;
    using CommandSystem;

    /// <summary>
    /// Credits Command.
    /// </summary>
    public class CreditsSubCommand : ICommand
    {
        /// <inheritdoc />
        public string Command { get; } = "credits";

        /// <inheritdoc />
        public string[] Aliases { get; } = [];

        /// <inheritdoc />
        public string Description { get; } = "Créditos de PlaceholderAPI";

        /// <inheritdoc />
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = $"¡Bienvenido!\nPlaceholderAPI ({PlaceholderAPIPlugin.Instance.Version}v), creado por {PlaceholderAPIPlugin.Instance.Author}\nUso:\npapi ecloud Name\npapi credits";
            return true;
        }
    }
}
