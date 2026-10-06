using CommandSystem;
using Exiled.API.Features.Pools;
using Exiled.Permissions.Extensions;
using System;
using System.Linq;
using System.Text;

namespace AdminTools.Commands.InstantKill
{
    public class List : ICommand
    {
        public string Command { get; } = "list";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Muestra la lista de jugadores con InstantKill activado.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.instakill"))
            {
                response = "No tienes permiso para usar este comando";
                return false;
            }

            StringBuilder playerLister = StringBuilderPool.Pool.Get();

            playerLister.Append(Main.InstantKill.Any() ? "Jugadores con InstantKill activado:\n" : "Ningún jugador conectado tiene InstantKill activado");
            playerLister.Append(Extensions.LogPlayers(Main.InstantKill));

            response = StringBuilderPool.Pool.ToStringReturn(playerLister);
            return true;
        }
    }
}
