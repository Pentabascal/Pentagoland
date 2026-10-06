using System;
using System.Collections.Generic;
using System.Linq;
using Axwabo.CommandSystem.Attributes;
using Axwabo.CommandSystem.Attributes.Targeting;
using Axwabo.CommandSystem.Commands;
using Axwabo.CommandSystem.Commands.Interfaces;
using Axwabo.CommandSystem.Commands.MessageOverrides;
using Axwabo.CommandSystem.Extensions;
using Axwabo.CommandSystem.Permissions;
using InventorySystem;
using InventorySystem.Items;

namespace Axwabo.CommandSystem.Example;

[RemoteAdminCommand]
[AffectedOnePlayerMessage("{0} ha recibido una tarjeta de acceso aleatoria")] // {0} will be replaced with the player's name using string.Format
public sealed class GiveRandomKeycard : SeparatedTargetingCommand, IAffectedMultiplePlayersMessageGenerator, ICustomResultCompiler
{

    private static readonly ItemType[] KeycardItems =
    [
        ItemType.KeycardJanitor,
        ItemType.KeycardScientist,
        ItemType.KeycardResearchCoordinator,
        ItemType.KeycardZoneManager,
        ItemType.KeycardGuard,
        ItemType.KeycardMTFPrivate,
        ItemType.KeycardContainmentEngineer,
        ItemType.KeycardMTFOperative,
        ItemType.KeycardMTFCaptain,
        ItemType.KeycardFacilityManager,
        ItemType.KeycardChaosInsurgency,
        ItemType.KeycardO5
    ];

    // you can also override basic command properties instead of using attributes
    // targets are still required to be specified with attributes
    public override string Name => "giveKeycard";

    public override string Description => "Entrega una tarjeta de acceso aleatoria a los jugadores indicados";

    // just use attributes for this if can use a built-in permission checker
    // or implement IPermissionChecker in the command/custom attribute
    // also check out permission creators (resolvers)
    protected override IPermissionChecker Permissions { get; } = new SimpleVanillaPlayerPermissionChecker(PlayerPermissions.GivingItems);

    protected override CommandResult ExecuteOn(ReferenceHub target, ArraySegment<string> arguments, CommandSender sender)
    {
        if (target.inventory.UserInventory.Items.Values.Any(i => i.Category == ItemCategory.Keycard))
            return "!El jugador ya tiene una tarjeta de acceso";
        if (!target.inventory.ServerAddItem(KeycardItems.RandomItem(), ItemAddReason.AdminCommand)) // implicit null-check
            return "!No se pudo añadir el objeto al inventario del jugador";
        return "Tarjeta de acceso añadida";
    }

    // all affected overrides this
    public string OnAffected(int players) => $"Se ha entregado una tarjeta de acceso aleatoria a {players} jugadores";

    protected override string NoPlayersAffected => "Ningún jugador ha recibido una tarjeta de acceso";

    public CommandResult? CompileResultCustom(List<CommandResultOnTarget> success, List<CommandResultOnTarget> failures)
    {
        if (failures.Count == 0)
            return CommandResult.Null;
        if (success.Count == 0)
            return CommandResult.Failed(failures.JoinResults());
        return CommandResult.Succeeded(
            success.JoinResults()
            + "\n"
            + failures.JoinResults()
        );
    }

}
