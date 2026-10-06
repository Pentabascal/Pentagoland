using CustomPlayerEffects;
using InventorySystem.Items.Firearms.Modules;
using LabApi.Features.Wrappers;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("FastHands", Rarity.Common)]
    public class FastHands(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Manos rápidas";

        public override string Description => "Recargas y descargas las armas más rápido.";

        public bool Reloading
        {
            get => _reloading;
            private set
            {
                if (value == _reloading)
                    return;

                _reloading = value;

                if (_reloading)
                {
                    originalIntensity = Player.GetEffect<Scp1853>().Intensity;
                    hasPoison = Player.TryGetEffect(out Poisoned pois) && pois.IsEnabled;
                }

                Player.EnableEffect<Scp1853>(_reloading ? (byte)50 : originalIntensity);

                if (!hasPoison)
                    Player.DisableEffect<Poisoned>();
            }
        }

        bool hasPoison;
        bool _reloading;
        byte originalIntensity;

        public override void Tick()
        {
            base.Tick();
            Reloading = Player.CurrentItem is FirearmItem f && f.Base.TryGetModule(out IReloaderModule mod) && mod.IsReloadingOrUnloading;
        }
    }
}
