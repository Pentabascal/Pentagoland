using CustomPlayerEffects;
using SwiftArcadeMode.Utils.Structures;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("Ninjutsu", Rarity.Rare)]
    public class Ninjutsu(PerkInventory inv) : PerkCooldownBase(inv)
    {
        public override string Name => "Ninjutsu";

        public override string PerkDescription => $"Te vuelves invisible con menos de {HealthThreshold} HP durante {Duration}s.";

        public virtual float HealthThreshold => 20f;

        public virtual float Duration => 5f;

        public override float Cooldown => 120f;

        private readonly Timer activationTimer = new();

        public override void Init()
        {
            base.Init();
            activationTimer.OnTimerEnd += OnTimerEnd;
        }

        public override void Remove()
        {
            base.Remove();
            activationTimer.OnTimerEnd -= OnTimerEnd;
        }

        protected virtual void OnTimerEnd() => Player.DisableEffect<Invisible>();

        public override void Effect()
        {
            Player.EnableEffect<Invisible>();
            Player.CurrentItem = null;
            activationTimer.Reset(Duration);
        }

        public override void Tick()
        {
            base.Tick();

            if (Player.Health <= HealthThreshold)
                Trigger();

            if (!activationTimer.Ended)
                SendMessage("Tiempo de invisibilidad: " + Mathf.Round(activationTimer.CurrentValue) + "s");

            activationTimer.Tick(Time.fixedDeltaTime);
        }
    }
}
