using CustomPlayerEffects;
using LabApi.Events.Handlers;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("Ghostly", Rarity.Legendary)]
    public class Ghostly(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Fantasmal";

        public override string Description => "Atraviesas las puertas y te vuelves más transparente cuanto menos salud tienes.\nSi te quedas quieto, te vuelves invisible.";

        public float HealthPercentage => Player.Health / Player.MaxHealth;

        Vector3 lastCheckedPosition;
        private byte currentDegree;

        public byte CurrentDegree
        {
            get => currentDegree;
            private set
            {
                if (currentDegree != value)
                    Player.EnableEffect<Fade>(value);

                currentDegree = value;
            }
        }

        public float Moving { get; private set; }

        public override void Init()
        {
            base.Init();
            PlayerEvents.ChangedRole += OnChangedRole;
            Player.EnableEffect<CustomPlayerEffects.Ghostly>();
            lastCheckedPosition = Player.Position;
        }

        private void OnChangedRole(LabApi.Events.Arguments.PlayerEvents.PlayerChangedRoleEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            Player.EnableEffect<CustomPlayerEffects.Ghostly>();
            lastCheckedPosition = Player.Position;
        }

        public override void Tick()
        {
            base.Tick();

            CurrentDegree = (byte)Mathf.Lerp(255, Mathf.Lerp(255, 0, HealthPercentage), Moving);
            bool moving = lastCheckedPosition != Player.Position;
            Moving = Mathf.MoveTowards(Moving, moving ? 1f : 0f, Time.fixedDeltaTime * (moving ? 2f : 0.25f));

            lastCheckedPosition = Player.Position;
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.ChangedRole -= OnChangedRole;
            Player.DisableEffect<CustomPlayerEffects.Ghostly>();
        }
    }
}
