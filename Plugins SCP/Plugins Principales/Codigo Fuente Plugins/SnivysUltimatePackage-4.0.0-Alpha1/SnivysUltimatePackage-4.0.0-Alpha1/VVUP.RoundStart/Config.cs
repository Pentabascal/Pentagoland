using System.ComponentModel;
using Exiled.API.Interfaces;

namespace VVUP.RoundStart
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        
        [Description("Determina si la puerta final de escape debe abrirse al inicio de la ronda.")]
        public bool EscapeDoorOpen { get; set; } = true;

        [Description("¿Debe permanecer bloqueada la puerta final de escape al inicio de la ronda?")]
        public bool EscapeDoorLock { get; set; } = true;

        [Description("Permite modificar la descontaminación.")]
        public bool DecontaminationChanges { get; set; } = false;

        [Description("Tiempo establecido para la descontaminación. El valor predeterminado es de 705 segundos (11 minutos y 45 segundos, juego base); sumar 195 lo aumentará a 900 segundos (15 minutos).")]
        public float DecontaminationTime { get; set; } = 195f;
    }
}