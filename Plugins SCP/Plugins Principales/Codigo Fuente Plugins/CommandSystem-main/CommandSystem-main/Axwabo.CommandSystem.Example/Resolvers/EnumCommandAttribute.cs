using System;
using Axwabo.CommandSystem.Attributes;

namespace Axwabo.CommandSystem.Example.Resolvers;

[AttributeUsage(AttributeTargets.Class)]
internal sealed class EnumCommandAttribute : CommandTargetAttribute
{

    public CustomCommandType Command { get; }

    public EnumCommandAttribute(CustomCommandType command) : base(CommandHandlerType.RemoteAdmin) => Command = command;

    public static string GetDescription(CustomCommandType command) => command switch
    {
        CustomCommandType.Flash => "Aplica un efecto cegador a los jugadores indicados",
        CustomCommandType.DiscordBroadcast => "Envía un aviso para invitar a unirse al servidor de Discord",
        CustomCommandType.SendHint => "Muestra un aviso a los jugadores indicados",
        _ => null
    };

}
