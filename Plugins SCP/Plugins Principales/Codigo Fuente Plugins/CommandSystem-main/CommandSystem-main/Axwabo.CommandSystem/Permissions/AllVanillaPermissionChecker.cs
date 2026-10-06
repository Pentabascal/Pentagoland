using Axwabo.CommandSystem.Commands;

namespace Axwabo.CommandSystem.Permissions;

/// <summary>
/// A permission checker that ensures that all of the given permissions are sufficient.
/// </summary>
public sealed class AllVanillaPermissionChecker : IPermissionChecker
{

    /// <summary>The list of permissions.</summary>
    public readonly PlayerPermissions[] Permissions;

    /// <summary>
    /// Initializes a new instance of the <see cref="AllVanillaPermissionChecker"/> class.
    /// </summary>
    /// <param name="permissions">A list of permissions.</param>
    public AllVanillaPermissionChecker(PlayerPermissions[] permissions) => Permissions = permissions;

    /// <inheritdoc />
    public CommandResult CheckPermission(CommandSender sender)
    {
        if (sender.FullPermissions)
            return true;
        var reduced = Permissions.Aggregate((PlayerPermissions) 0, (a, b) => a | b);
        return sender.CheckPermission(reduced)
            ? true
            : "!No tienes permiso para usar esta función.\nSe requieren todos estos permisos: " + string.Join(", ", Permissions);
    }

}
