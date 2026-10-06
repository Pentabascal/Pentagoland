namespace XPSystem.Commands.Admin.Subcommands
{
    using System;
    using CommandSystem;
    using XPSystem.API;
    using XPSystem.API.Player;
    using XPSystem.API.StorageProviders;
    using XPSystem.API.StorageProviders.Models;

    public class GiveCommand : DatabasePlayerCommand
    {
        public override bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!XPPlayer.TryGetAndCheckPermission(sender, "xps.give", out BaseXPPlayer? player))
            {
                response = "No tienes permiso (xps.give) para usar este comando.";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Uso: xp give (amount) o xp give (amount) (player)";
                return false;
            }
            
            string amountString = arguments.At(0);
            if (!int.TryParse(amountString, out int amount))
            {
                response = $"Cantidad no válida: {amountString}.";
                return false;
            }

            response = "temp";
            XPAPI.EnsureStorageProviderValid();

            if (!DoThingWithArgs(ref arguments, 1, player, ref response, out PlayerInfoWrapper playerInfo, out IPlayerId playerId))
                return false;

            playerInfo.XP += amount;

            response = $"Se han concedido {amount} XP a {playerId.ToString()} ({playerInfo.Nickname}).";
            return true;
        }

        public override string Command { get; } = "give";
        public override string Description { get; } = "Concede XP a un jugador.";
    }
}