using Exiled.API.Interfaces;
using System.ComponentModel;

namespace UsefulHints
{
    public class Translations : ITranslation
    {
        [Description("Traducciones específicas del servidor para UsefulHints.\n Avisos:")]
        public string ShowHints { get; set; } = "Mostrar avisos";
        public string ShowHintsDescription { get; set; } = "Activa o desactiva la visualización de avisos.";

        [Description("Temporizadores:")]
        public string ShowTimers { get; set; } = "Mostrar temporizadores";
        public string ShowTimersDescription { get; set; } = "SCP-268, SCP-2176 y SCP-1576.";

        [Description("Avisos sobre objetos:")]
        public string ItemWarningHints { get; set; } = "Avisos sobre objetos";
        public string ItemWarningHintsDescription { get; set; } = "Evita usar SCP-207, Anti SCP-207 y SCP-1853 al mismo tiempo.";

        [Description("Avisos de fuego amigo:")]
        public string FriendlyFireWarning { get; set; } = "Aviso de fuego amigo";
        public string FriendlyFireWarningDescription { get; set; } = "Muestra un aviso cuando dañes a un compañero o recibas daño de uno.";
        
        [Description("Contador de eliminaciones:")]
        public string KillCounter { get; set; } = "Contador de eliminaciones";
        public string KillCounterDescription { get; set; } = "Activa o desactiva la visualización del contador de eliminaciones.";
        
        [Description("Opciones de activación y desactivación:")]
        public string ButtonOn { get; set; } = "Activado";
        public string ButtonOff { get; set; } = "Desactivado";

        [Description("Nombres de zona y equipo para LastHumanBroadcast:")]
        public string Lcz { get; set; } = "Zona de Contención Ligera";
        public string Hcz { get; set; } = "Zona de Contención Pesada";
        public string Entrance { get; set; } = "Zona de Entrada";
        public string Superficie { get; set; } = "Superficie";
        public string FoundationForces { get; set; } = "<color=#0096FF>Fuerza Operativa Móvil</color>";
        public string ClassD { get; set; } = "<color=#FF8E00>Clase D</color>";
        public string Scientists { get; set; } = "<color=#FFFF7C>Científico</color>";
        public string ChaosInsurgency { get; set; } = "<color=#15853D>Insurgencia del Caos</color>";
    }
}