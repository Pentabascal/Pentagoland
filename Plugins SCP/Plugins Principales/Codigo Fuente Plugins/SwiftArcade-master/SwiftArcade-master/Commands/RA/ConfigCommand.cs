using CommandSystem;
using System;

namespace SwiftArcadeMode.Commands.RA
{
    public abstract class ConfigCommand : ICommand
    {
        public abstract string Command { get; }

        public abstract string[] Aliases { get; }

        public abstract string Description { get; }

        public abstract string Name { get; }

        public virtual bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.RoundEvents))
            {
                response = "¡No tienes permiso!";
                return false;
            }

            if (arguments.Array.Length < 2 || !bool.TryParse(arguments.Array[1], out bool option))
            {
                response = "¡Introduce un valor válido! (true/false)";
                return false;
            }

            ChangeOption(option);

            response = "Opción de configuración \"" + Name + "\" cambiada a " + option;
            return true;
        }

        public abstract void ChangeOption(bool option);
    }
}
