using Hints;
using LabApi.Features.Wrappers;

namespace SwiftArcadeMode.Features
{
    public abstract class PerkBase(PerkInventory inv) : IPerkInfo
    {
        public Rarity Rarity { get; set; }

        public PerkRestriction Restriction { get; set; }

        public string FancyName => Name.FancifyPerkName(Rarity);

        public virtual int SlotUsage => 1;

        public abstract string Name { get; }

        public abstract string Description { get; }

        public readonly PerkInventory Inventory = inv;
        public Player Player => Inventory.Parent;

        /// <summary>
        /// Calls when the player acquires the perk.
        /// </summary>
        public virtual void Init() { }

        /// <summary>
        /// Runs every FixedUpdate.
        /// </summary>
        public virtual void Tick() { }

        /// <summary>
        /// Calls when the perk gets removed from the player.
        /// </summary>
        public virtual void Remove() { }

        public virtual void SendMessage(string message, float duration = 3) => Player.SendHint($"<size=36>{FancyName}\n</size><size=24>{message}</size>", [HintEffectPresets.FadeOut()], duration);
    }

    public static class RarityExtensions
    {
        public static string GetColor(this Rarity rarity) =>
            rarity switch
            {
                Rarity.Common => "#FFFFFF",
                Rarity.Uncommon => "#00FF00",
                Rarity.Rare => "#00FFFF",
                Rarity.Epic => "#FF00FF",
                Rarity.Legendary => "#FFFF00",
                Rarity.Mythic => "#0000FF",
                Rarity.Secret => "#FF0000",
                _ => "#FFFFFF"
            };
    }

    public enum Rarity : int
    {
        Common = 52,
        Uncommon = 38,
        Rare = 26,
        Epic = 12,
        Legendary = 7,
        Mythic = 3,
        Secret = 1
    }
}
