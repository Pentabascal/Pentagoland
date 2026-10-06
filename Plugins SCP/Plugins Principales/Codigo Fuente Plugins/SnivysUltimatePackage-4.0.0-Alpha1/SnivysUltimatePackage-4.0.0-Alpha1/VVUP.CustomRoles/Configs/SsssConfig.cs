using System.ComponentModel;

namespace VVUP.CustomRoles.Configs
{
    public class SsssConfig
    {
        public bool SsssEnabled { get; set; } = true;
        [Description("Si está activado, los jugadores que activen sus habilidades mediante SSSS recibirán sugerencias; de lo contrario, recibirán un aviso. **NOTA:** solo se aplica a las activaciones mediante SSSS; si usan Alt izquierdo (noclip), SIEMPRE recibirán una sugerencia.")]
        public bool UseHints { get; set; } = true;
        public string Header { get; set; } = "Roles personalizados de Vicious Vikki";
        public int ActiveCamoId { get; set; } = 10101;
        public int ChargeId { get; set; } = 10102;
        public int DetectId { get; set; } = 10103;
        public int DoorPickingId { get; set; } = 10104;
        public int HealingMistId { get; set; } = 10105;
        public int RemoveDisguiseId { get; set; } = 10106;
        //public int ReviveMistId { get; set; } = 10107;
        public int TeleportId { get; set; } = 10108;
        public int SoundBreakerId { get; set; } = 10109;
        public int ReplicatorId { get; set; } = 10110;
        public int CustomRoleTextId { get; set; } = 2;
        public string ActiveCamoHint { get; set; } = "Pulsa la tecla asignada para activar el camuflaje activo y volverte invisible durante un breve periodo (habilidad personalizada).";
        public string ChargeHint { get; set; } = "Pulsa la tecla asignada para activar la carga y abalanzarte sobre un objetivo (habilidad personalizada).";
        public string DetectHint { get; set; } = "Pulsa la tecla asignada para activar la detección y localizar a los jugadores cercanos (habilidad personalizada).";
        public string DoorPickingHint { get; set; } = "Pulsa la tecla asignada para activar el ganzuado y abrir puertas (habilidad personalizada).";
        public string HealingMistHint { get; set; } = "Pulsa la tecla asignada para activar la niebla curativa y sanar a jugadores cercanos y a ti mismo (habilidad personalizada).";
        public string RemoveDisguiseHint { get; set; } = "Pulsa la tecla asignada para quitarte el disfraz (habilidad personalizada).";
        //public string ReviveMistHint { get; set; } = "Pulsa la tecla asignada para activar la niebla de reanimación y reanimar a los jugadores cercanos (habilidad personalizada).";
        public string TeleportHint { get; set; } = "Pulsa la tecla asignada para activar el teletransporte a una ubicación seleccionada (habilidad personalizada).";
        public string SoundBreakerHint { get; set; } = "Pulsa la tecla asignada para reiniciar el tiempo de recarga del parpadeo y reducir el intervalo y la distancia del siguiente parpadeo (habilidad personalizada).";
        public string ReplicatorHint { get; set; } = "Pulsa la tecla asignada para crear un señuelo y realizar un reconocimiento seguro (habilidad personalizada).";
        public string ActiveCamoSsssText { get; set; } = "Camuflaje activo";
        public string ChargeSsssText { get; set; } = "Carga";
        public string DetectSsssText { get; set; } = "Detección";
        public string DoorPickingSsssText { get; set; } = "Ganzuar";
        public string HealingMistSsssText { get; set; } = "Niebla curativa";
        public string RemoveDisguiseSsssText { get; set; } = "Quitar disfraz";
        //public string ReviveMistSsssText { get; set; } = "Niebla de reanimación";
        public string TeleportSsssText { get; set; } = "Teletransporte";
        public string SoundBreakerSsssText { get; set; } = "Rompesonidos";
        public string ReplicatorSsssText { get; set; } = "Replicador";
        public string SsssActiveCamoActivationMessage { get; set; } = "Camuflaje activo activado";
        public string SsssChargeActivationMessage { get; set; } = "Carga activada";
        public string SsssDoorPickingActivationMessage { get; set; } = "Ganzuado activado. Interactúa con la puerta que quieras forzar.";
        public string SsssHealingMistActivationMessage { get; set; } = "Niebla curativa activada";
        public string SsssRemoveDisguiseActivationMessage { get; set; } = "Quitando el disfraz";
        //public string SsssReviveMistActivationMessage { get; set; } = "Niebla de reanimación activada";
        public string SsssTeleportActivationMessage { get; set; } = "Teletransporte activado";
        public string SsssSoundBreakerActivationMessage { get; set; } = "Rompesonidos activado";
        public string SsssReplicatorActivationMessage { get; set; } = "Replicador activado";
        public int RoundStartRolesId { get; set; } = 10111;
        public int RespawnWaveRolesId { get; set; } = 10112;
        public int Scp049ReviveRolesId { get; set; } = 10113;
        public string RoundStartRolesSsssText { get; set; } = "Activar la asignación de roles personalizados al inicio de la ronda";
        public string RespawnWaveRolesSsssText { get; set; } = "Activar la asignación de roles personalizados en las oleadas de reaparición";
        public string Scp049ReviveRolesSsssText { get; set; } = "Activar la asignación de roles personalizados al reanimar como SCP-049-2";
        public string CustomRoleReceivingEnabledText { get; set; } = "Sí";
        public string CustomRoleReceivingDisabledText { get; set; } = "No";
    }
}