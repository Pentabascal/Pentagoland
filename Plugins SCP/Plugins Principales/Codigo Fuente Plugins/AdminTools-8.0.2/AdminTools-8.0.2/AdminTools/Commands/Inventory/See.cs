using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using NorthwoodLib.Pools;
using System;
using System.Text;

namespace AdminTools.Commands.Inventory
{
    using Exiled.API.Features.Items;

    public class See : ICommand
    {
        public string Command { get; } = "see";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Muestra los objetos del inventario de un jugador";

        public string[] Usage { get; } = new string[] { "%player%", };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.GivingItems, out response))
                return false;

            if (arguments.Count != 1)
            {
                response = "Uso: inventory see (ID / nombre del jugador)";
                return false;
            }

            Player ply = Player.Get(arguments.At(0));
            if (ply == null)
            {
                response = $"No se ha encontrado al jugador: {arguments.At(0)}";
                return false;
            }

            StringBuilder invBuilder = StringBuilderPool.Shared.Rent();
            if (ply.Items.Count != 0)
            {
                invBuilder.Append("El jugador ");
                invBuilder.Append(ply.Nickname);
                invBuilder.AppendLine(" tiene los siguientes objetos en el inventario:");
                foreach (Item item in ply.Items)
                {
                    invBuilder.Append("- ");
                    invBuilder.AppendLine(item.Type.ToString());
                }
            }
            else
            {
                invBuilder.Append("El jugador ");
                invBuilder.Append(ply.Nickname);
                invBuilder.Append(" no tiene objetos en el inventario");
            }
            string msg = invBuilder.ToString();
            StringBuilderPool.Shared.Return(invBuilder);
            response = msg;
            return true;
        }
    }
}
