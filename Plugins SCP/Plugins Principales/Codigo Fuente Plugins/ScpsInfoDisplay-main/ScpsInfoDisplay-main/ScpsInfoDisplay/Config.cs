using Exiled.API.Interfaces;
using PlayerRoles;
using ScpsInfoDisplay.Enums;
using System.Collections.Generic;
using System.ComponentModel;

namespace ScpsInfoDisplay
{
    public class Config : IConfig
    {
        [Description("¿Está activado el complemento?")]
        public bool IsEnabled { get; set; } = true;
        
        [Description("Textos que se muestran. Formato: rol, texto.")]
        public Dictionary<RoleTypeId, string> DisplayStrings { get; set; } = new Dictionary<RoleTypeId, string>()
        {
            { RoleTypeId.Scp049, "[<color=#FF0000>SCP</color><color=#FFFFFF>049</color> <color=#FFFFFF>%health%</color><color=#B6FFB0>HP</color> <color=#FFFFFF>%humeshield%</color><color=#5B4B9A>HS</color>]" },
            { RoleTypeId.Scp079, "[<color=#FF0000>SCP</color><color=#FFFFFF>079</color> <color=#FFFFFF>TIER %079level%</color> <color=#FFFFFF>%079energypercent%%</color><color=#B6FFB0> ENERGÍA</color>]" },
            { RoleTypeId.Scp096, "[<color=#FF0000>SCP</color><color=#FFFFFF>096</color> <color=#FFFFFF>%health%</color><color=#B6FFB0>HP</color> <color=#FFFFFF>%humeshield%</color><color=#5B4B9A>HS</color>]" },
            { RoleTypeId.Scp106, "[<color=#FF0000>SCP</color><color=#FFFFFF>106</color> <color=#FFFFFF>%health%</color><color=#B6FFB0>HP</color> <color=#FFFFFF>%humeshield%</color><color=#5B4B9A>HS</color>]" },
            { RoleTypeId.Scp173, "[<color=#FF0000>SCP</color><color=#FFFFFF>173</color> <color=#FFFFFF>%health%</color><color=#B6FFB0>HP</color> <color=#FFFFFF>%humeshield%</color><color=#5B4B9A>HS</color>]" },
            { RoleTypeId.Scp939, "[<color=#FF0000>SCP</color><color=#FFFFFF>939</color> <color=#FFFFFF>%health%</color><color=#B6FFB0>HP</color> <color=#FFFFFF>%humeshield%</color><color=#5B4B9A>HS</color>]" },
            { RoleTypeId.Scp3114, "[<color=#FF0000>SCP</color><color=#FFFFFF>3114</color> <color=#FFFFFF>%health%</color><color=#B6FFB0>HP</color> <color=#FFFFFF>%humeshield%</color><color=#5B4B9A>HS</color>]" }
        };
        
        [Description("Integraciones con roles personalizados. Formato: variable de sesión que indica que el jugador pertenece a ese rol, texto que se muestra.")]
        public Dictionary<string, string> CustomRolesIntegrations { get; set; } = new Dictionary<string, string>();

        [Description("Textos mostrados para el estado de SCP-096.")]
        public Dictionary<string, string> Scp096StateIndicator { get; set; } = new Dictionary<string, string>
        {
            ["Docile"] = "En calma",
            ["Distressed"] = "Enfureciéndose",
            ["Enraged"] = "Furioso",
            ["Calming"] = "Calmándose"
        };

        [Description("Indicadores de observación de SCP-173.")]
        public Dictionary<string, string> Scp173ObservationIndicators { get; set; } = new Dictionary<string, string>
        {
            ["Observed"] = "ⓞ",
            ["Unobserved"] = "-"
        };

        [Description("Nombres mostrados para los disfraces de SCP-3114.")]
        public Dictionary<string, string> Scp3114DisguiseDisplay { get; set; } = new Dictionary<string, string>
        {
            ["ClassD"] = "Clase-D",
            ["Scientist"] = "Científico",
            ["FacilityGuard"] = "Guardia",
            ["NtfCaptain"] = "Capitán MTF",
            ["NtfSergeant"] = "Sargento MTF",
            ["NtfSpecialist"] = "Especialista MTF",
            ["NtfPrivate"] = "Soldado MTF",
            ["ChaosConscript"] = "Recluta CI",
            ["ChaosMarauder"] = "Asaltante CI",
            ["ChaosRepressor"] = "Represor CI",
            ["ChaosRifleman"] = "Fusilero CI",
            ["Tutorial"] = "Tutorial"
        };
        
        [Description("Alineación del texto mostrado. Valores posibles: Left, Center, Right.")]
        public TextAlignment TextAlignment { get; set; } = TextAlignment.Right;
        
        [Description("Tamaño del texto mostrado.")]
        public byte TextSize { get; set; } = 17;

        [Description("Desplazamiento horizontal opcional en unidades de espacio de TMP (0 para mantener la alineación adaptable).")]
        public int TextHorizontalOffset { get; set; } = 0;

        [Description("Posición vertical RueI del texto mostrado, entre 0 (abajo) y 1000 (arriba).")]
        public int TextVerticalPosition { get; set; } = 850;
        
        [Description("El jugador que ve la lista se resaltará con el marcador especial situado a la izquierda. Déjalo vacío para desactivarlo.")]
        public string PlayersMarker { get; set; } = "";
        
        [Description("¿Mostrar mensajes de depuración en la consola del servidor?")]
        public bool Debug { get; set; } = false;
    }
}