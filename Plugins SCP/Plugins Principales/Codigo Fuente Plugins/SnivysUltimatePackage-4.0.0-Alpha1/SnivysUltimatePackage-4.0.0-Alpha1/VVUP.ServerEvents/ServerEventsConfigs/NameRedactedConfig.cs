using System.ComponentModel;

namespace VVUP.ServerEvents.ServerEventsConfigs
{
    public class NameRedactedConfig
    {
        public string StartEventCassieMessage { get; set; } = "Name Gone";
        public string StartEventCassieText { get; set; } = "Name Gone (Redacted Names Event)";

        [Description("¿Qué apodo deben tener todos los jugadores?")]
        public string NameRedactedName { get; set; } = "[REDACTED]";
    }
}