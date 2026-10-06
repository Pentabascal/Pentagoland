using SwiftArcadeMode.Utils.Interfaces;

namespace SwiftArcadeMode.Utils.Structures
{
    public abstract class OverrideBase
    {
        public abstract void Dispose();
    }

    public abstract class OverrideBase<T1>(T1 parent) : OverrideBase where T1 : IOverrideParent
    {
        public readonly T1 Parent = parent;

        public override void Dispose() => Parent.RemoveOverride(this);
    }
}
