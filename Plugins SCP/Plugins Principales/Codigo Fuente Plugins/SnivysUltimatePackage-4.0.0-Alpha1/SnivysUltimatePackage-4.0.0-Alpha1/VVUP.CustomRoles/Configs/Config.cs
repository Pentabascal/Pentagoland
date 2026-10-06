using System.ComponentModel;
using Exiled.API.Interfaces;

namespace VVUP.CustomRoles.Configs
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        [Description("Retraso, en segundos, antes de registrar las condiciones de aparición de los roles personalizados al iniciar el servidor. Permite que se registren casi todos los roles que usan la API de este módulo. Usa 0 para registrarlos inmediatamente.")]
        public float DelayBeforeRegisteringCustomRolesSpawnConditions { get; set; } = 2f;
        public CustomRolesConfig CustomRolesConfig { get; set; } = new();
        //public CustomRoleWrapperConfig CustomRoleWrapperConfig { get; set; } = new();
        public SsssConfig SsssConfig { get; set; } = new();
    }
}