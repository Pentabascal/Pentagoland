using PlayerStatsSystem;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("Marathoner", Rarity.Common)]
    public class Marathoner(PerkInventory inv) : PerkTriggerCooldownBase(inv)
    {
        public override string Name => "Maratonista";

        public override string Description => $"Recuperas un {Amount * 100f}% de resistencia cada {Cooldown} segundos.";

        public override string PerkDescription => "";

        public virtual float Amount => 0.1f;

        public override string ReadyMessage => "";

        public override float Cooldown => 8f;

        public override void Effect()
        {
            StaminaStat stat = Player.GetStatModule<StaminaStat>();
            if (Player.StaminaRemaining < stat.MaxValue)
                Player.StaminaRemaining += stat.MaxValue * Amount;
        }
    }
}
