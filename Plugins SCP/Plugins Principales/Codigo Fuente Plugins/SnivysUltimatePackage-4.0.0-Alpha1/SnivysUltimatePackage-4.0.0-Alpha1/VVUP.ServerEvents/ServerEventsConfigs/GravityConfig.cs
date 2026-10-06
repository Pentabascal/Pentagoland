using System.ComponentModel;
using UnityEngine;

namespace VVUP.ServerEvents.ServerEventsConfigs
{
    public class GravityConfig
    {
        public string StartEventCassieMessage { get; set; } = "Low Gravity";
        public string StartEventCassieText { get; set; } = "Evento de gravedad reducida";

        [Description("¿Qué gravedad se debe aplicar a todos los jugadores?")]
        public Vector3 GravityChanges { get; set; } = new Vector3()
        {
            x = 0,
            y = -12.60f,
            z = 0
        };
    }
}