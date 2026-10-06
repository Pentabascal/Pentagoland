namespace PlaceholderAPI.Commands.Sub
{
    using System;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using PlaceholderAPI.Cloud;
    using PlaceholderAPI.Cloud.Beans;
    using PlaceholderAPI.Cloud.Helper;

    /// <summary>
    /// ECloud Support command.
    /// </summary>
    public class ECloudSubCommand : ICommand
    {
        /// <inheritdoc />
        public string Command { get; } = "ecloud";

        /// <inheritdoc />
        public string[] Aliases { get; } = ["cloud"];

        /// <inheritdoc />
        public string Description { get; } = "Base de datos en línea de PlaceholderAPI";

        /// <inheritdoc />
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count < 1)
            {
                response = "[PAPI] Debes indicar un subcomando: forceupdate, download.";
                return false;
            }

            switch (arguments.At(0))
            {
                case "download":
                    if (!sender.CheckPermission("papi.ecloud.download"))
                    {
                        response = "[PAPI] No tienes permisos suficientes.";
                        return false;
                    }

                    if (!PlaceholderAPIPlugin.Instance.Config.ConnectToEcloud)
                    {
                        response = "[PAPI] El servicio ECloud está desactivado en la configuración.";
                        return false;
                    }

                    ECloudDatabase.UpdateData();

                    if (arguments.Count < 2 || !ECloudDatabase.TryGetExpansion(arguments.At(1), out ECloudExpansion expansion))
                    {
                        response = "[PAPI] Falta el nombre del repositorio.";
                        return false;
                    }

                    if (expansion.Hidden)
                    {
                        response = "[PAPI] Falta el nombre del repositorio.";
                        return false;
                    }

                    if (expansion.Internal)
                    {
                        response = "[PAPI] El repositorio es interno, así que no hace falta descargarlo.";
                        return false;
                    }

                    if (!expansion.Verified && !PlaceholderAPIPlugin.Instance.Config.DownloadUnsafeFromEcloud)
                    {
                        response = "[PAPI] El repositorio no es seguro.";
                        return false;
                    }

                    ECloudDownloader.Download(expansion.RepoId);

                    response = "[PAPI] Se está descargando tu expansión.";
                    return true;

                case "forceupdate":
                    if (!sender.CheckPermission("papi.ecloud.download"))
                    {
                        response = "[PAPI] No tienes permisos suficientes.";
                        return false;
                    }

                    if (!PlaceholderAPIPlugin.Instance.Config.ConnectToEcloud)
                    {
                        response = "[PAPI] El servicio ECloud está desactivado en la configuración.";
                        return false;
                    }


                    ECloudDatabase.ClearCache();

                    ECloudDatabase.UpdateData();
                    response = "[PAPI] Se ha vaciado la caché.";
                    return true;

                default:
                    response = "[PAPI] Este subcomando no existe.";
                    return false;
            }
        }
    }
}
