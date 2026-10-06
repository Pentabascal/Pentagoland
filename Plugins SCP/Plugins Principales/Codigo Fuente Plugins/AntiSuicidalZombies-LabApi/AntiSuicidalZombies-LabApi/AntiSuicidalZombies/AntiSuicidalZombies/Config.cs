using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;

namespace AntiSuicidalZombies
{
    public class Config
    {
        [Description("¿Activar los mensajes de depuración?")]
        public bool Debug { get; set; } = false;

        [Description("Efectos aplicados a los zombis al atravesar una puerta Tesla.")]
        public Dictionary<string, EffectParameters> TeslaEffects { get; set; } = new()
        {
            {
               "Blurred",
               new EffectParameters
               {
                   Intensity = 1,
                   Duration = 10f
               }
            },
            {
               "Burned",
               new EffectParameters
               {
                   Intensity = 1,
                   Duration = 30f
               }
            }
        };

        [Description("Efectos aplicados a los zombis al ser aplastados por una puerta o un ascensor.")]
        public Dictionary<string, EffectParameters> CrushedEffects { get; set; } = new()
        {
            {
               "Concussed", new()
               {
                   Duration = 10f,
                   Intensity = 5
               }
            },
            {
               "Deafened", new()
               {
                   Duration = 20f,
                   Intensity = 1
               }
            },
            {
               "Slowness", new()
               {
                   Duration = 30f,
                   Intensity = 30
               }
            }
        };
    }

    public class EffectParameters
    {
        public float Duration { get; set; }
        public byte Intensity { get; set; }        
    }
}
