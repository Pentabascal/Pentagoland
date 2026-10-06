namespace XPSystem.Config
{
    using System.ComponentModel;
    using System.IO;
    using static API.LoaderSpecific;

    /// <summary>
    /// Config class shared by configs made for in-game usage.
    /// </summary>
    public abstract class ConfigShared : Config
    {
        [Description("Ruta de la carpeta de configuración adicional.")]
        public override string ExtendedConfigPath { get; set; } =
            Path.Combine(ConfigPath, "XPSystem");

        [Description("Ruta al archivo de la base de datos heredada.")]
        public override string LegacyDefaultDatabasePath { get; set; } =
            Path.Combine(
#if EXILED
                Exiled.API.Features.Paths.Configs, 
#else
                Path.Combine(LabApi.Loader.Features.Paths.PathManager.Plugins.FullName, "XPSystem"),
#endif
                "Players.db");
    }
}