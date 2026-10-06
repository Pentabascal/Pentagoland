using System.ComponentModel;
using Exiled.API.Interfaces;
using PlayerRoles;

namespace VVUP.MicroDamageReduction
{
    public class MicroDamageReductionConfig : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; }

        [Description("SCPs that get the damage reduction from Micro")]
        public RoleTypeId[] ScpDamageReduction { get; set; } =
        {
            RoleTypeId.Scp096,
        };

        [Description("The reduced damage that the Micro does (multiplied by, so 0.5 is by half)")]
        public float ScpDamageReductionValue { get; set; } = 0.5f;
    }
}