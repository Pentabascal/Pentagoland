namespace XPSystem.Commands.Admin.Subcommands
{
    using System;
    using CommandSystem;
    using XPSystem.API;
    using XPSystem.API.StorageProviders;

    public class ClearCacheCommand : SanitizedInputCommand
    {
        public override bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermissionLS("xps.admin.clearcache"))
            {
                response = "No tienes permiso (xps.clearcache) para usar este comando.";
                return false;
            }

            XPAPI.EnsureStorageProviderValid();

            if (XPAPI.StorageProvider is not StorageProvider storageProvider)
            {
                response = "El proveedor de almacenamiento no es uno de los proveedores con caché predefinidos; no se puede borrar su caché con este comando.";
                return false;
            }

            storageProvider.ClearCache();

            response = "Caché borrada.";
            return true;
        }

        public override string Command { get; } = "clearcache";
        public override string Description { get; } = "Borra la caché del proveedor de almacenamiento.";
    }
}