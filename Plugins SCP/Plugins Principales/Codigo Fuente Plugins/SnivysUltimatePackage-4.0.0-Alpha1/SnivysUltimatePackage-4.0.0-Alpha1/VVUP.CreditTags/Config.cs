using System.ComponentModel;
using Exiled.API.Interfaces;

namespace VVUP.CreditTags
{
    public class Config : IConfig
    {
        [Description("Sirve para dar las gracias a quienes han contribuido al complemento, ya sea probándolo o aportando código.")]
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        [Description("Sustituye las insignias del jugador etiquetado, si ya tiene otra.")]
        public bool BadgeOverride { get; set; } = false;
        [Description("Sustituye la información personalizada del jugador etiquetado, si ya tiene otra. Se recomienda desactivarlo, ya que los roles personalizados también la utilizan.")]
        public bool CustomInfoOverride { get; set; } = false;
        [Description("¿Se debe ignorar la opción DNT del jugador? Al activarlo, siempre se comprobará al jugador, aunque tenga DNT activado.")]
        public bool IgnoreDntFlag { get; set; } = false;
    }
}