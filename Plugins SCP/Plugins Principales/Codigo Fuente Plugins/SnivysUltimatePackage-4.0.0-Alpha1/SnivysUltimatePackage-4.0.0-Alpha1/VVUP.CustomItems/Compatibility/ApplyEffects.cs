using Exiled.API.Enums;

namespace VVUP.CustomItems
{
    public class ApplyEffects
    {
        public EffectType EffectType { get; set; }
        public byte Intensity { get; set; } = 1;
        public float Duration { get; set; }
        public bool AddDurationIfActive { get; set; }
    }
}
