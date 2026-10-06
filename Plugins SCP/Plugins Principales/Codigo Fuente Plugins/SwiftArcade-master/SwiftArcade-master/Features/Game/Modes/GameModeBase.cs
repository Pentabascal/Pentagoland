using SwiftArcadeMode.Features.Humans.Perks;

namespace SwiftArcadeMode.Features.Game.Modes
{
    public abstract class GameModeBase
    {
        public abstract PerkSpawnRulesBase OverrideSpawnRules { get; }

        public abstract void Start();
        public abstract void Tick();
        public abstract void End();
    }
}
