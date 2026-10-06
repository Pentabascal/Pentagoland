using System;
using CommandSystem;
using Exiled.Permissions.Extensions;
using UsefulHintsAddons.Extensions;

namespace UsefulHintsAddons.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ChangeLanguage : ICommand
    {
        public string Command => "usefulhints_language";
        public string[] Aliases => new[] { "uhl" };
        public string Description => "Cambia o vuelve a cargar el idioma de UsefulHints (Addons). Uso: uhl <lang|reload|list>";

        private static readonly string[] Supported =
        {
            "pl","en","de","fr","cs","sk","es","it","pt","ru","tr","zh","ko"
        };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("uh.changelanguage"))
            {
                response = "No tienes permiso: uh.changelanguage";
                return false;
            }

            if (arguments.Count == 0)
            {
                response = GetHelp();
                return false;
            }

            string arg = arguments.At(0).ToLowerInvariant();

            if (arg is "list" or "langs" or "languages")
            {
                response = "Idiomas disponibles: " + string.Join(", ", Supported);
                return true;
            }

            if (arg is "reload" or "refresh")
            {
                var current = UsefulHintsAddons.Instance.Config.Language;
                _ = TranslationManager.ApplyAsync(current);
                response = $"Volviendo a cargar el idioma: {current}";
                return true;
            }

            if (!Supported.Contains(arg))
            {
                response = $"Idioma no compatible: {arg}. Usa: uhl list";
                return false;
            }

            UsefulHintsAddons.Instance.Config.Language = arg;
            _ = TranslationManager.ApplyAsync(arg);
            response = $"Idioma cambiado a: {arg}";
            return true;
        }

        private string GetHelp() =>
            "Uso:\n" +
            "  uhl <lang>    - Cambiar el idioma (p. ej., uhl pl)\n" +
            "  uhl reload    - Forzar la descarga del idioma actual\n" +
            "  uhl list      - Mostrar los idiomas disponibles\n" +
            $"Idiomas disponibles: {string.Join(", ", Supported)}";
    }
}