using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Interfaces;
using PlayerRoles;
using UnityEngine;
using VVUP.Base;

namespace VVUP.PostNukeEnhancements
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        [Description("¿Se debe activar la radiación tras la detonación de la ojiva?")]
        public bool RadiationEnabled { get; set; } = true;
        [Description("Finaliza el proceso de radiación si, por algún motivo, la ojiva deja de figurar como detonada (por ejemplo, si un administrador la reinicia y luego cancela la cuenta atrás).")]
        public bool EnsureWarheadDetonatedForEffects { get; set; } = true;
        [Description("¿Cuánto debe tardar en empezar la radiación (en segundos)?")]
        public float RadiationDelay { get; set; } = 300;
        [Description("Daño por radiación en cada ciclo.")]
        public float RadiationDamage { get; set; } = 1f;
        [Description("Multiplicador del daño por radiación para los SCP.")]
        public float RadiationDamageScpMultiplier { get; set; } = 25f;
        [Description("Tiempo entre ciclos de daño por radiación (en segundos).")]
        public float RadiationIntervalTime { get; set; } = 1f;
        [Description("Mensaje que se muestra a los jugadores cuando la radiación empieza a afectar la zona.")]
        public string RadiationStartMessage { get; set; } = "Sientes una repentina oleada de náuseas y mareo cuando la radiación empieza a afectar la zona.";
        [Description("¿Durante cuántos segundos se debe mostrar el aviso de inicio de la radiación?")]
        public float RadiationMessageDuration { get; set; } = 5f;
        [Description("Si está activado, se mostrarán pistas; de lo contrario, se mostrará un aviso.")]
        public bool UseHints { get; set; } = true;
        [Description("Mensaje que se muestra a los jugadores cuando mueren por envenenamiento por radiación.")]
        public string RadiationDeathMessage { get; set; } = "Has sucumbido al envenenamiento por radiación.";
        [Description("Efectos aplicados a los jugadores mientras estén irradiados. Se aplicarán en cada intervalo.")]
        public List<ApplyEffects> RadiationEffects { get; set; } = new()
        {
            new()
            {
                EffectType = EffectType.Burned,
                Intensity = 1,
                Duration = 0,
            },
            new()
            {
                EffectType = EffectType.Exhausted,
                Intensity = 1,
                Duration = 0,
            },
            new()
            {
                EffectType = EffectType.Concussed,
                Intensity = 1,
                Duration = 0,
            }
        };
        [Description("¿Qué roles son inmunes al daño por radiación?")]
        public List<RoleTypeId> RadiationImmuneRoles { get; set; } = new()
        {
            RoleTypeId.Overwatch,
        };
        [Description("Activa el apagón de la superficie tras la detonación de la ojiva.")]
        public bool SurfaceBlackout { get; set; } = true;
        [Description("¿Cuánto debe durar el apagón de la superficie (en segundos)?")]
        public float SurfaceBlackoutDuration { get; set; } = 10f;
        [Description("Al terminar el apagón de la superficie, ¿con qué color deben comenzar las luces?")]
        public Color SurfaceStartColor { get; set; } = new Color(-1, -1, -1, -1);
        [Description("Al terminar el apagón de la superficie, ¿con qué color deben quedar las luces? Usa -1 para el color predeterminado.")]
        public Color SurfaceEndColor { get; set; } = new Color(-1, -1, -1, -1);
        [Description("Determina si el cambio al color objetivo de las luces de la superficie es instantáneo o gradual (se aplica al terminar el apagón y utiliza el color final de la superficie).")]
        public bool SurfaceColorInstant { get; set; } = false;
        [Description("¿Cada cuánto se actualiza el color de la superficie si el cambio no es instantáneo (en segundos)?")]
        public float SurfaceColorUpdateInterval { get; set; } = 0.1f;
        [Description("¿Cuánto debe cambiar aproximadamente el color en cada actualización si el cambio no es instantáneo (entre 0 y 1)?")]
        public float SurfaceColorChangeAmount { get; set; } = 0.01f;
        [Description("¿Debe cerrarse la puerta de superficie tras la detonación de la ojiva?")]
        public bool SurfaceGateClose { get; set; } = true;
        [Description("¿Debe desbloquearse la puerta de superficie tras la detonación de la ojiva?")]
        public bool SurfaceGateUnlock { get; set; } = true;
    }
}