using System.ComponentModel;

namespace VVUP.ServerEvents.ServerEventsConfigs
{
    public class VariableLightsConfig
    {
        public string StartEventCassieMessage { get; set; } = "Facility Power Surge Detected";
        public string StartEventCassieText { get; set; } = "Facility Power Surge Detected (Lights variable event)";

        [Description("Determina cuánto tardan las luces en cambiar.")]
        public float TimeForChange { get; set; } = 20;

        [Description("¿Las luces solo deben aumentar y reducir su intensidad o también pueden cambiar de color?")]
        public bool ColorChanging { get; set; } = false;

        [Description("¿Las luces deben ser iguales en todas las salas o variar de una sala a otra?")]
        public bool DifferentLightsPerRoom { get; set; } = true;
    }
}