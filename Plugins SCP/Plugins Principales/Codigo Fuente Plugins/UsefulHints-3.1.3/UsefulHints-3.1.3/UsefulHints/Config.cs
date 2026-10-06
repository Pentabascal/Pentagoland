using Exiled.API.Interfaces;
using System.ComponentModel;

namespace UsefulHints
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        
        [Description("Ajustes específicos del servidor:")]
        public bool EnableServerSettings { get; set; } = true;
        
        [Description("Ajustes de los avisos:")]
        public bool EnableHints { get; set; } = true;
        public string Scp096LookMessage { get; set; } = "¡Has mirado a SCP-096!";
        public string Scp268TimeLeftMessage { get; set; } = "Tiempo restante: {0} s";
        public string Scp2176TimeLeftMessage { get; set; } = "Tiempo restante: {0} s";
        public string Scp1576TimeLeftMessage { get; set; } = "Tiempo restante: {0} s";
        public string GrenadeDamageHint { get; set; } = "Daño: {0}";
        public string JailbirdUseMessage { get; set; } = "Cargas restantes: {0}";
        public string Scp207HintMessage { get; set; } = "Tienes {0} dosis de SCP-207.";
        public string AntiScp207HintMessage { get; set; } = "Tienes {0} dosis de Anti SCP-207.";
        public bool ShowHintOnEquipItem { get; set; } = false;
        
        [Description("Avisos sobre objetos:")]
        public bool EnableWarnings { get; set; } = true;
        public string Scp207Warning { get; set; } = "<color=yellow>\u26A0</color> Ya tienes activo el efecto de <color=#A60C0E>SCP-207</color>.";
        public string AntiScp207Warning { get; set; } = "<color=yellow>\u26A0</color> Ya tienes activo el efecto de <color=#2969AD>Anti SCP-207</color>.";
        public string Scp1853Warning { get; set; } = "<color=yellow>\u26A0</color> Ya tienes activo el efecto de <color=#1CAA21>SCP-1853</color>.";
        
        [Description("Avisos de fuego amigo:")]
        public bool EnableFfWarning { get; set; } = true;
        public string FriendlyFireWarning { get; set; } = "<size=27><color=yellow>\u26A0 No dañes a tus compañeros.</color></size>";
        public string DamageTakenWarning { get; set; } = "<size=27><color=red>{0}</color> <color=yellow>(tu compañero) te ha golpeado.</color></size>";
        public bool ClassDAreTeammates { get; set; } = true;
        public bool EnableCuffedWarning { get; set; } = false;
        public string CuffedAttackerWarning { get; set; } = "<size=27><color=yellow>\u26A0 El jugador está esposado.</color></size>";
        public string CuffedPlayerWarning { get; set; } = "<size=27><color=red>{0}</color> <color=yellow>te ha golpeado mientras estabas esposado.</color></size>";
        
        [Description("Contador de eliminaciones:")]
        public bool EnableKillCounter { get; set; } = true;
        public string KillCountMessage { get; set; } = "{0} eliminaciones";
        public bool CountPocketKills { get; set; } = false;
        
        [Description("Resumen de la ronda:")]
        public bool EnableRoundSummary { get; set; } = true;
        public ushort RoundSummaryMessageDuration { get; set; } = 10;
        public string HumanKillMessage { get; set; } = "<size=27><color=#70EE9C>{0}</color> consiguió más eliminaciones como <color=green>humano</color>: <color=yellow>{1}</color></size>";
        public string ScpKillMessage { get; set; } = "<size=27><color=#70EE9C>{0}</color> consiguió más eliminaciones como <color=red>SCP</color>: <color=yellow>{1}</color></size>";
        public string TopDamageMessage { get; set; } = "<size=27><color=#70EE9C>{0}</color> infligió más daño: <color=yellow>{1}</color></size>";
        public string FirstScpKillerMessage { get; set; } = "<size=27><color=#70EE9C>{0}</color> fue quien primero eliminó a un <color=red>SCP</color></size>";
        public string EscaperMessage { get; set; } = "<size=27><color=#70EE9C>{0}</color> fue el primero en escapar de la instalación: <color=yellow>{1}:{2}</color></size>";
        
        [Description("Compañeros de equipo:")]
        public bool EnableTeammates { get; set; } = true;
        public float TeammateHintDelay { get; set; } = 4f;
        public string TeammateHintMessage { get; set; } = "<align=left><size=28><color=#70EE9C>Tus compañeros</color></size> \n<size=25><color=yellow>{0}</color></size></align>";
        public float TeammateMessageDuration { get; set; } = 8f;
        public string AloneHintMessage { get; set; } = "<align=left><color=red>Estás jugando en solitario.</color></align>";
        public float AloneMessageDuration { get; set; } = 4f;
        
        [Description("Aviso del último humano:")]
        public bool EnableLastHumanBroadcast { get; set; } = true;
        public string BroadcastForHuman { get; set; } = "<color=red>¡Eres el último humano con vida!</color>";
        public string BroadcastForScp { get; set; } = "<color=#70EE9C>{0}</color> es el último humano con vida y juega como {1} en <color=yellow>{2}</color>.";
        public bool IgnoreTutorialRole { get; set; } = true;
        
        [Description("Avisos del mapa:")]
        public bool EnableMapBroadcast { get; set; } = true;
        public string BroadcastWarningLcz { get; set; } = "<color=yellow>La Zona de Contención Ligera</color> será descontaminada dentro de 5 minutos.";
    }
}