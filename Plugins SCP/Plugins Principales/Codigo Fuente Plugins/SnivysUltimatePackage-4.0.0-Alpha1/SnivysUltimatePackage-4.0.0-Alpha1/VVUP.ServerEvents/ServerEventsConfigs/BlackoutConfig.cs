using System.Collections.Generic;
using System.ComponentModel;

namespace VVUP.ServerEvents.ServerEventsConfigs
{
    public class BlackoutConfig
    {
        public string StartEventCassieMessage { get; set; } = "Fallo del sistema eléctrico";
        public string StartEventCassieText { get; set; } = "Fallo del sistema eléctrico. (Ha comenzado el evento de apagón)";

        public string EndEventCassieMessage { get; set; } = "Sistema eléctrico restablecido";
        public string EndEventCassieText { get; set; } = "Sistema eléctrico restablecido. (Ha terminado el evento de apagón)";

        [Description("¿Debe terminar el evento de apagón cuando se activen los tres generadores?")]
        public bool GeneratorEndsEvent { get; set; } = true;

        [Description("¿Qué objeto debe recibir el jugador al comenzar el evento de apagón?")]
        public List<ItemType> BlackoutEventStartingItem { get; set; } = new()
        {
            ItemType.Lantern
        };
        [Description("Actívalo si quieres que las luces parpadeen antes de apagarse definitivamente.")]
        public bool FlickerLights { get; set; } = false;
        [Description("¿Cuánto tiempo deben parpadear las luces antes de apagarse? (en segundos).")]
        public int FlickerDuration { get; set; } = 5;
    }
}