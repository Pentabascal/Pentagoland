/*using System.Collections.Generic;
using System.ComponentModel;
using Exiled.CustomRoles.API.Features;
using VVUP.CustomRoles.API;

namespace VVUP.CustomRoles.Configs
{
    public class CustomRoleWrapperConfig
    {
        [Description("Permite que los roles personalizados que no usan la API de este complemento sean compatibles con su sistema de aparición.")]
        public bool EnableCustomRoleCompatibility { get; set; } = false;
        [Description("Lista de roles personalizados que se registrarán con su equipo inicial y probabilidad de aparición. VVUP.CR, VVUP.FCR y VVUP.HK ya admiten este sistema y no es necesario volver a configurarlos.")]
        public List<CustomRoleWrapper> CustomRoles { get; set; } = new List<CustomRoleWrapper>
        {
            new CustomRoleWrapper
            {
                CustomRoleId = 0,
                StartTeam = StartTeam.Other,
                Chance = 0,
                Limit = 0,
            },
        };
    }

    public class CustomRoleWrapper
    {
        public uint CustomRoleId { get; set; } = 0;
        public StartTeam StartTeam { get; set; } = StartTeam.Other;
        public int Chance { get; set; } = 0;
        public int Limit { get; set; } = 0;
    }
    
    public class CustomRoleWrapperAdapter : ICustomRole
    {
        private readonly CustomRoleWrapper _wrapper;
        private CustomRole _cachedRole;
        
        public CustomRoleWrapperAdapter(CustomRoleWrapper wrapper)
        {
            _wrapper = wrapper;
            StartTeam = wrapper.StartTeam;
            Chance = wrapper.Chance;
        }

        public StartTeam StartTeam { get; set; }
        public int Chance { get; set; }
        
        public static implicit operator CustomRole(CustomRoleWrapperAdapter adapter)
        {
            return adapter._cachedRole ??= CustomRole.Get(adapter._wrapper.CustomRoleId);
        }
    }
}*/