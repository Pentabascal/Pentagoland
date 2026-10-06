using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using MEC;
using SwiftArcadeMode.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Gambler
{
    [Perk("Gambler", Rarity.Legendary)]
    public class Gambler(PerkInventory inv) : PerkBase(inv)
    {
        public static readonly List<GamblerEffectBase> PositiveEffects = [];
        public static readonly List<GamblerEffectBase> NegativeEffects = [];

        public static void RegisterEffects()
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();

            List<Type> types = [.. callingAssembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(GamblerEffectBase).IsAssignableFrom(t))];

            foreach (Type t in types)
            {
                GamblerEffectBase ef = Activator.CreateInstance(t) as GamblerEffectBase;
                if (ef.Positive)
                    PositiveEffects.Add(ef);
                else
                    NegativeEffects.Add(ef);
            }
        }

        public override string Name => "Apostador";

        public override string Description => "Lanza una moneda y juégate la vida...";

        public override void Init()
        {
            base.Init();
            PlayerEvents.FlippedCoin += OnFlippedCoin;
            PlayerEvents.ChangedRole += OnPlayerChangedRole;

            if (Player.IsInventoryFull)
                Pickup.Create(ItemType.Coin, Player.Position).Spawn();
            else
                Player.AddItem(ItemType.Coin);
        }

        private void OnPlayerChangedRole(PlayerChangedRoleEventArgs ev)
        {
            if (ev.Player != Player || !Player.IsAlive)
                return;

            if (Player.IsInventoryFull)
                Pickup.Create(ItemType.Coin, Player.Position).Spawn();
            else
                Player.CurrentItem = Player.AddItem(ItemType.Coin);
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.FlippedCoin -= OnFlippedCoin;
            PlayerEvents.ChangedRole -= OnPlayerChangedRole;
        }

        private void OnFlippedCoin(LabApi.Events.Arguments.PlayerEvents.PlayerFlippedCoinEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            Player.SendHint("Te ha tocado...");

            bool isTails = ev.IsTails;
            Timing.CallDelayed(1.65f, () =>
            {
                Player.SendHint(isTails ? "Cruz" : "Cara");
                Timing.CallDelayed(0.35f, () =>
                {
                    GamblerEffectBase eff = ev.IsTails ? NegativeEffects.GetWeightedRandom() : PositiveEffects.GetWeightedRandom();
                    eff.Effect(Player);
                    SendMessage(eff.Explanation, eff.ExplanationDuration);
                });
            });
        }
    }
}
