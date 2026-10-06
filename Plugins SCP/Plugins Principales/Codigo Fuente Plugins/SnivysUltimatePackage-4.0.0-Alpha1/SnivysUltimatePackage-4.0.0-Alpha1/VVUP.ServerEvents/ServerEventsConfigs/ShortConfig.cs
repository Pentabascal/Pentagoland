using System.Collections.Generic;
using System.ComponentModel;

namespace VVUP.ServerEvents.ServerEventsConfigs
{
    public class ShortConfig
    {
        public string StartEventCassieMessage { get; set; } = "Small Humans Test Activation";
        public string StartEventCassieText { get; set; } = "Small Humans Test Activation (Short Event Started)";

        public string EndEventCassieMessage { get; set; } = "Small Humans Test Deactivation";
        public string EndEventCassieText { get; set; } = "Small Humans Test Deactivation (Short Event Ended)";

        [Description("Determina el tamaño de los jugadores.")]
        public float PlayerSize { get; set; } = 0.25f;

        [Description("¿Con qué objeto empezarán los jugadores?")]
        public List<ItemType> StartingItems { get; set; } = new()
        {
            ItemType.KeycardJanitor
        };
    }
}