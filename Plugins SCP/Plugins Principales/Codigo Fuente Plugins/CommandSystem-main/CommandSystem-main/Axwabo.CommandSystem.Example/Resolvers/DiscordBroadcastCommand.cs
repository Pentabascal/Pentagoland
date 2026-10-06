using System;
using Axwabo.CommandSystem.Commands;
using Axwabo.Helpers;
using LabApi.Features.Wrappers;

namespace Axwabo.CommandSystem.Example.Resolvers;

[EnumCommand(CustomCommandType.DiscordBroadcast)]
public sealed class DiscordBroadcastCommand : CommandBase
{

    protected override CommandResult Execute(ArraySegment<string> arguments, CommandSender sender)
    {
        Server.SendBroadcast("¡Únete a nuestro servidor de Discord!".Size(70) + "\nEnlace en la información del servidor.", 15);
        return "Aviso enviado.";
    }

}
