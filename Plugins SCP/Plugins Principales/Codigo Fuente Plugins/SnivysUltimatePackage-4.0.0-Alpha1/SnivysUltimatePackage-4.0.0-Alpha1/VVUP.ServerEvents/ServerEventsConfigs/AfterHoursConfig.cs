using System.Collections.Generic;
using System.ComponentModel;

namespace VVUP.ServerEvents.ServerEventsConfigs
{
    public class AfterHoursConfig
    {
        public string StartEventCassieMessage { get; set; } = "Fuera de horario";
        public string StartEventCassieText { get; set; } = "Fuera de horario. (Ha comenzado el evento Fuera de horario)";
        public int TeslaActivationChance { get; set; } = 25;
        [Description("Frecuencia, en segundos, con la que se actualizará la probabilidad.")]
        public float TeslaActivationChanceCycle { get; set; } = 5;
        public int IntercomTime { get; set; } = 5;
        [Description("¿Con qué objeto empezarán los jugadores?")]
        public List<ItemType> StartingItems { get; set; } = new()
        {
            ItemType.Lantern
        };
    }
}