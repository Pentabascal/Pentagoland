using SwiftArcadeMode.Utils.Structures;

namespace SwiftArcadeMode.Utils.Effects
{
    public abstract class CustomEffectBase(float duration)
    {
        public CustomEffectContainer Parent { get; private set; }
        public virtual int StackCount => int.MaxValue;

        public readonly Timer EffectTimer = new(duration, false);

        public virtual void Init(CustomEffectContainer cont) => Parent = cont;
        public virtual bool Stack(CustomEffectBase existingEffect) => true;
        public abstract void Add();
        public abstract void Tick();
        public abstract void Remove();
    }
}
