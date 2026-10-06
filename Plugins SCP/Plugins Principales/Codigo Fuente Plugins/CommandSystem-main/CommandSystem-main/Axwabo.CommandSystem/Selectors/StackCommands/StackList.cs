using Axwabo.CommandSystem.Attributes;
using Axwabo.CommandSystem.Commands;

namespace Axwabo.CommandSystem.Selectors.StackCommands;

[CommandProperties(CommandHandlerType.RaAndServer, "stackList", "Muestra los jugadores de la pila de selección.", "slist")]
internal sealed class StackList : CommandBase
{

    protected override CommandResult Execute(ArraySegment<string> arguments, CommandSender sender)
    {
        var selection = PlayerSelectionStack.Get(sender);
        return selection == null
            ? $"!No se puede obtener una pila de selección para {sender.GetType().FullName}."
            : selection.IsEmpty
                ? "La pila de selección está vacía."
                : $"Tu pila de selección:\n{selection}";
    }

}
