using System;

namespace SwiftArcadeMode.Utils.Structures
{
    public class PlayerStat(Action<float> setValue) : ModifiableStatistic()
    {
        public readonly Action<float> SetValue = setValue;

        public override void UpdateValue()
        {
            base.UpdateValue();
            SetValue?.Invoke(GetValue());
        }
    }
}
