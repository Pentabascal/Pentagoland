using CustomPlayerEffects;

using EffectDisplay.Features.Sereliazer;

using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using PlayerRoles;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace EffectDisplay
{
    public class Configs : IConfig
    {
        [Description("Indica si el plugin está activado.")]
        public bool IsEnabled { get; set; } = true;
        [Description("Indica si se muestra información de depuración para ayudar a detectar errores.")]
        public bool Debug { get; set; } = false;
        [Description("Indica si se utilizará una base de datos.")]
        public bool DataBaseEnabled { get; set; } = true;
        [Description("Intervalo de actualización de la información.")]
        public float UpdateTime { get; set; } = 0.9f;
        [Description("Línea que se mostrará para cada tipo de efecto; puedes personalizarla.")]
        public Dictionary<StatusEffectBase.EffectClassification, string> EffectLine { get; set; } = new Dictionary<StatusEffectBase.EffectClassification, string>()
        {
            {StatusEffectBase.EffectClassification.Mixed, $"<color=purple>%effect%</color> -> %time%/%duration% NIVEL: %intensity%" },
            {StatusEffectBase.EffectClassification.Positive, $"<color=green>%effect%</color> -> %time%/%duration% NIVEL: %intensity%" },
            {StatusEffectBase.EffectClassification.Negative, $"<color=red>%effect%</color> -> %time%/%duration% NIVEL: %intensity%" },
            {StatusEffectBase.EffectClassification.Technical, " " }
        };
        [Description("Lista de efectos que no verá el jugador (se ocultan los efectos del proceso técnico).")]
        public List<EffectType> BlackList { get; set; } = new List<EffectType>()
        {
            EffectType.InsufficientLighting,
            EffectType.SoundtrackMute,
            EffectType.FogControl
        };
        [Description("https://discord.com/channels/656673194693885975/1172647045237067788/1172647045237067788 Permite asignar a un efecto de la lista el nombre que indiques.")]
        public Dictionary<EffectType, string> EffectTranslation { get; set; } = new Dictionary<EffectType, string>()
        {
            { EffectType.None, "Efecto desconocido" }
        };
        [Description("Ruta completa de la base de datos.")]
        public string DataPath { get; set; } = Path.Combine(Paths.Configs, "EffectDisplay", "data.db");
        [Description("Roles para los que no se mostrarán los efectos (los roles de jugadores muertos se ignoran sin necesidad de configurarlos).")]
        public List<RoleTypeId> IgnoredRoles { get; set; } = new List<RoleTypeId>()
        {
            RoleTypeId.None,
            RoleTypeId.Spectator
        };
        [Description("Ajustes predeterminados de visualización, utilizados si no hay proveedores de Hint compatibles.")]
        public NativeHintSettings NativeHintSettings { get; set; } = new NativeHintSettings();
        [Description("Texto que verá el usuario al pasar el cursor sobre el signo de interrogación en los ajustes.")]
        public string EnabledDisplayDescription { get; set; } = "Indica si se muestran los efectos activos; sustituye al comando .display de la consola.";
        [Description("Indica si el plugin te avisará cuando haya una actualización.")]
        public bool CheckForUpdate { get; set; } = true;
        [Description("ID del componente SSS para el campo principal (no lo dupliques).")]
        public int HeaderId { get; set; } = 2030;
        [Description("Texto que se muestra al pasar el cursor sobre el signo de interrogación.")]
        public string HeaderDescription { get; set; } = "Proporciona los ajustes de Effect Display.";
        [Description("ID del componente SSS TwoButton (no lo dupliques).")]
        public int TwoButtonId { get; set; } = 2031;
        [Description("Nombre del campo TwoButton.")]
        public string TwoButtonLabel { get; set; } = "Mostrar efectos activos";
        [Description("Texto de la primera opción.")]
        public string TwoButtonEnabled { get; set; } = "ACTIVADO";
        [Description("Texto de la segunda opción.")]
        public string TwoButtonDisabled { get; set; } = "DESACTIVADO";
        [Description("Mensaje que verá el jugador si la base de datos está desactivada o no se encuentra el archivo, por lo que no se puede usar esta función.")]
        public string MessageWnenDataBaseDisabled { get; set; } = "Esta función no está disponible en este servidor.";
        [Description("Mensaje que verá el jugador al activar la visualización de efectos.")]
        public string MessageWhenPlayerEnabled { get; set; } = "Has <b>activado</b> la visualización de los efectos activos.";
        [Description("Mensaje que verá el jugador al desactivar la visualización de efectos.")]
        public string MessageWhenPlayerDisabled { get; set; } = "Has <b>desactivado</b> la visualización de los efectos activos.";
        [Description("Mensaje que verá el jugador si el servidor no encuentra el componente del jugador o al propio jugador (por ejemplo, si se ejecuta desde el servidor dedicado).")]
        public string MessageWhenErrorOccurred { get; set; } = "No se ha encontrado el componente que muestra los efectos ni al propio jugador.";
        /// <summary>
        /// Return effect name from <see cref="EffectTranslation"/> if not found return <see cref="EffectType"/> as <see cref="string"></see>
        /// </summary>
        public string GetName(EffectType effectType)
        {
            string name = string.Empty;
            if (EffectTranslation.TryGetValue(effectType, out name))
            {
                return name;
            }
            return effectType.ToString();
        }
    }
}