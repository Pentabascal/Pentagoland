using System.Collections.Generic;
using System.ComponentModel;
using PlayerRoles;
using ScpsInfoDisplay.LabApi.Enums;

namespace ScpsInfoDisplay.LabApi
{
    public class Config
    {
        [Description("¿Está activado el complemento?")]
        public bool IsEnabled { get; set; } = true;
        
        [Description("Textos que se muestran. Formato: rol, texto.")]
        public Dictionary<RoleTypeId, string> DisplayStrings { get; set; } = new Dictionary<RoleTypeId, string>()
        {
            { RoleTypeId.Scp049, "<color=%roleColor%>SCP-049 [%healthpercent%% Zombis: %zombies%] %distance%</color>" },
            { RoleTypeId.Scp079, "<color=%roleColor%>SCP-079 [%generators%%engaging%/3] Nivel: %079level% Exp: %079experience% Energía: %079energy%</color>" },
            { RoleTypeId.Scp096, "<color=%roleColor%>SCP-096 [%healthpercent%%] %distance%</color>" },
            { RoleTypeId.Scp106, "<color=%roleColor%>SCP-106 [%healthpercent%%] Vigor: %106vigor%% %distance%</color>" },
            { RoleTypeId.Scp173, "<color=%roleColor%>SCP-173 [%healthpercent%%] %distance%</color>" },
            { RoleTypeId.Scp939, "<color=%roleColor%>SCP-939 [%healthpercent%%] %distance%</color>" },
            { RoleTypeId.Scp3114, "<color=%roleColor%>SCP-3114 [%healthpercent%%] %distance%</color>" }
        };
        
        [Description("Integraciones con roles personalizados. Formato: variable de sesión que indica que el jugador pertenece a ese rol, texto que se muestra.")]
        public Dictionary<string, string> CustomRolesIntegrations { get; set; } = new Dictionary<string, string>();
        
        [Description("Alineación del texto mostrado. Valores posibles: Left, Center, Right.")]
        public TextAlignment TextAlignment { get; set; } = TextAlignment.Right;
        
        [Description("Desplazamiento vertical del texto mostrado.")]
        public sbyte TextPositionOffset { get; set; } = 30;

        [Description("El jugador que ve la lista se resaltará con el marcador especial situado a la izquierda.")]
        public bool MarkPlayerInList { get; set; } = true;
        
        [Description("El jugador que ve la lista se resaltará con el marcador especial situado a la izquierda. Déjalo vacío para desactivarlo.")]
        public string PlayersMarker { get; set; } = "<color=%roleColor%>Tú --></color>";
        
        [Description("¿Mostrar mensajes de depuración en la consola del servidor?")]
        public bool Debug { get; set; } = false;
    }
}