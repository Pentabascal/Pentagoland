namespace XPSystem.Config
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using XPSystem.API.Enums;

    public abstract class Config
    {
        [Description("¿Mostrar mensajes de depuración?")]
        public bool Debug { get; set; } = false;

        [Description("Función que calcula los XP necesarios para alcanzar un nivel; es la inversa de LevelFunction. Parámetro: level. Funciones disponibles: https://learn.microsoft.com/en-us/dotnet/api/system.math?view=net-8.0#methods.")]
        public string XPFunction { get; set; } = "Floor((level^a) * b)";

        [Description("Función que calcula el nivel a partir de los XP indicados. Parámetro: xp. Funciones disponibles: https://learn.microsoft.com/en-us/dotnet/api/system.math?view=net-8.0#methods.")]
        public string LevelFunction { get; set; } = "Floor((xp/b)^(1/a))";

        [Description("Parámetros adicionales para las funciones de nivel y XP.")]
        public Dictionary<string, double> AdditionalFunctionParameters { get; set; } = new()
        {
            { "a", 2 },
            { "b", 100 }
        };

        [Description("Sustituye los XP necesarios para alcanzar niveles concretos. Clave: level; valor: XP.")]
        public Dictionary<int, int> LevelXPOverrides { get; set; } = new()
        {
            [0] = 0
        };

        [Description("Indica si se detendrá la obtención de XP al terminar la ronda.")]
        public bool XPAfterRoundEnd { get; set; } = true;

        [Description("Multiplicador global de XP.")]
        public float GlobalXPMultiplier { get; set; } = 1f;

        [Description("Indica si el multiplicador global de XP se aplica a jugadores desconectados (por ejemplo, al usar comandos).")]
        public bool GlobalXPMultiplierForNonOnline { get; set; } = true;

        [Description("Indica si el multiplicador global de XP se aplica a los XP retirados.")]
        public bool XPMultiplierForXPLoss { get; set; } = false;

        [Description("Ruta de la carpeta de configuración de eventos, relativa a ExtendedConfigPath.")]
        public string EventConfigsFolder { get; set; } = "XPEventConfigs";

        [Description("Indica si se activa el comando .xp get para jugadores.")]
        public bool EnableGetXPCommand { get; set; } = true;

        [Description("Indica si se activa el comando .xp leaderboard para jugadores.")]
        public bool EnableLeaderboardCommand { get; set; } = true;

        [Description("Número máximo de entradas que puede mostrar el comando de clasificación para jugadores.")]
        public byte LeaderboardMaxLength { get; set; } = 10;

        [Description("Retraso adicional para actualizar los proveedores de visualización de XP. Aumentarlo puede ayudar si solo funciona para algunos jugadores.")]
        public float ExtraDelay { get; set; } = 0f;

        [Description("Mensaje que se mostrará a los jugadores que tengan activado DNT.")]
        public string DNTMessage { get; set; } =
            "No podemos registrar tus estadísticas mientras tengas DNT activado en las opciones del juego.";

        [Description("Indica si los mensajes que aparecen al conceder XP se formatean mediante una plantilla.")]
        public bool UseAddedXPTemplate { get; set; } = true;

        [Description("Si se activa, se utilizará una plantilla para los mensajes que modifican los XP. Parámetros: %message%, %currentxp%, %currentlevel%, %neededxp%, %nextlevel%."
            + "También: %progressbarfilled%, %progressbarremaining%. Su uso requiere los mismos cálculos adicionales que cuando UseTotalXP está desactivado.")]
        public string AddedXPTemplate { get; set; } = "%message%, (%currentxp% / %neededxp%)";

        [Description("Caracteres que se usarán en la barra de progreso, si se incluye en AddedXPTemplate.")]
        public string AddedXPProgressBarChars { get; set; } = "██";

        [Description("Longitud total (en caracteres) de la barra de progreso que se generará.")]
        public int AddedXPProgressBarLength { get; set; } = 20;

        [Description("Indica si se utilizarán los XP totales en lugar de solo los necesarios para el siguiente nivel. Si se desactiva y no se usa una barra de progreso en AddedXPTemplate, se realizarán cálculos adicionales.")]
        public bool UseTotalXP { get; set; } = true;

        [Description("Indica si se mostrará un mensaje al jugador cuando suba de nivel.")]
        public bool ShowAddedLVL { get; set; } = true;

        [Description("Indica si se mostrará el mensaje de XP al subir de nivel. Si se desactiva, solo se mostrará el nivel.")]
        public bool ShowXPOnLevelUp { get; set; } = true;

        [Description("Mensaje que se mostrará cuando un jugador suba de nivel.")]
        public string AddedLVLMessage { get; set; } = "NUEVO NIVEL: <color=red>%level%</color>";

        [Description("Forma en que se mostrarán los mensajes (por ejemplo, los de XP obtenida o subida de nivel).")]
        public DisplayMode DisplayMode { get; set; } = DisplayMode.Hint;

        [Description("Duración del mensaje, si corresponde.")]
        public float DisplayDuration { get; set; } = 5f;

        [Description("Texto que se antepondrá a todos los mensajes.")]
        public string TextPrefix { get; set; } = "";

        [Description("Texto que se añadirá al final de todos los mensajes.")]
        public string TextSuffix { get; set; } = "";

        [Description("Alias para que los jugadores consulten sus propios XP. Déjalo vacío para desactivarlo.")]
        public string ClientGetCommandAlias { get; set; } = "";

        [Description("Alias para que los jugadores consulten la clasificación. Déjalo vacío para desactivarlo.")]
        public string ClientLeaderboardCommandAlias { get; set; } = "";

        [Description("Nombre de tipo completo del proveedor de almacenamiento que se utilizará (predeterminado: XPSystem.BuiltInProviders.LiteDB.LiteDBProvider) Al ser un proveedor integrado, no necesita el nombre de ensamblado completo." +
                     "Disponible, aunque no se ofrece asistencia para: XPSystem.BuiltInProviders.MySql.MySqlProvider")]
        public string StorageProvider { get; set; } = "XPSystem.BuiltInProviders.LiteDB.LiteDBProvider";

        [Description("Nombres de tipo completos de los proveedores de visualización de XP que se cargarán, además de los proveedores integrados para el rango y el apodo.")]
        public List<string> AdditionalDisplayProviders { get; set; } = new()
        {
        };

        [Description("Imprime en la consola las claves utilizadas al final de la ronda. No se recomienda mantenerlo activado por motivos de rendimiento.")]
        public bool LogXPGainedMethods { get; set; } = false;
        
        public abstract string ExtendedConfigPath { get; set; }
        public abstract string LegacyDefaultDatabasePath { get; set; }
    }
}