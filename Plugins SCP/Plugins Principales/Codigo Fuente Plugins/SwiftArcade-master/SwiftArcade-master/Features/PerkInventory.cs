using CustomPlayerEffects;
using Hints;
using LabApi.Features.Wrappers;
using PlayerStatsSystem;
using SwiftArcadeMode.Features.Humans.Perks.Crafting;
using SwiftArcadeMode.Features.SCPs.Upgrades;
using SwiftArcadeMode.Utils.Extensions;
using SwiftArcadeMode.Utils.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils.NonAllocLINQ;

namespace SwiftArcadeMode.Features
{
    public class PerkInventory(Player targetPlayer)
    {
        public readonly Player Parent = targetPlayer;
        public readonly List<PerkBase> Perks = [];
        public readonly UpgradeQueue UpgradeQueue = new(targetPlayer);

        public readonly List<LimitAdditive> LimitAdditives = [];

        public int BaseLimit = 5;

        public class LimitAdditive
        {
            public int Additive;

            public static implicit operator int(LimitAdditive add) => add.Additive;
        }

        public LimitAdditive CreateLimitAdditive(int value = 0)
        {
            LimitAdditive additive = new()
            {
                Additive = value
            };

            LimitAdditives.Add(additive);
            return additive;
        }

        public void RemoveLimitAdditive(LimitAdditive adder) => LimitAdditives.Remove(adder);

        public int Limit
        {
            get
            {
                int b = BaseLimit;

                foreach (int add in LimitAdditives)
                    b += add;

                return b;
            }
        }

        public int LimitUsage
        {
            get
            {
                int total = 0;
                foreach (PerkBase perk in Perks)
                    total += perk.SlotUsage;
                return total;
            }
        }

        public void OnPerksUpdated()
        {
            foreach (Player spec in Parent.CurrentSpectators)
                spec.UpdateSpectatorDisplay(Parent);
        }

        public bool AddPerk(PerkAttribute type)
        {
            if (type == null || type.Perk.IsAbstract || type.Perk != typeof(PerkBase) && !type.Perk.IsSubclassOf(typeof(PerkBase)))
                return false;

            PerkManager.PerkProfile prof = type.Profile;

            if (Parent.HasRestrictions(type))
            {
                Parent.SendHint($"Tu rol no puede obtener {prof.FancyName}.", [HintEffectPresets.FadeOut()], 5f);
                return false;
            }

            if (type.HasConflicts(this, out PerkBase conf))
            {
                Parent.SendHint($"{prof.FancyName} entra en conflicto con {conf.FancyName}.", [HintEffectPresets.FadeOut()], 5f);
                return false;
            }

            PerkBase perk = Perks.FirstOrDefault((p) => p.GetType() == type.Perk);

            if (perk != null)
            {
                RemovePerk(perk);
                return true;
            }

            PerkBase p = (PerkBase)Activator.CreateInstance(type.Perk, this);

            if (LimitUsage >= Limit && p.SlotUsage > 0)
            {
                Parent.SendHint("¡Has alcanzado el límite de ventajas!", [HintEffectPresets.FadeOut()], 5f);
                return false;
            }

            p.Rarity = type.Rarity;
            p.Restriction = type.Restriction;
            Perks.Add(p);
            p.Init();
            Parent.SendHint($"Ventaja obtenida ({LimitUsage}/{Limit}): {prof.FancyName}\n{prof.Description}\n\nPulsa \"~\" y escribe \".sp\" para ver más detalles\nO asigna una tecla en <b>Ajustes específicos del servidor</b> para consultar tus ventajas.", [HintEffectPresets.FadeOut()], 10f);
            OnPerksUpdated();

            Parent.CheckCrafts();
            return true;
        }

        public void RemovePerk(Type type)
        {
            if (type == null)
                return;

            PerkBase perk = Perks.FirstOrDefault((p) => p.GetType() == type);

            if (perk == null)
                return;

            RemovePerk(perk);
        }

        public bool HasPerk(Type t) => GetPerk(t) != null;

        public bool TryGetPerk(Type t, out PerkBase perk)
        {
            perk = GetPerk(t);
            return perk != null;
        }

        public PerkBase GetPerk(Type t) => Perks.FirstOrDefault((p) => p.GetType() == t);

        public void ClearPerks()
        {
            foreach (PerkBase perk in Perks)
                perk.Remove();

            Perks.Clear();
        }

        public void RemovePerk(PerkBase perk)
        {
            if (perk == null)
                return;

            Perks.Remove(perk);
            perk.Remove();
            Parent.SendHint($"Ventaja eliminada: {perk.FancyName}\n\nPulsa \"~\" y escribe \".sp\" para ver más detalles\nO asigna una tecla en <b>Ajustes específicos del servidor</b> para consultar tus ventajas.", [HintEffectPresets.FadeOut()], 10f);
        }

        public PerkBase RemoveRandom()
        {
            if (Perks.Count <= 0)
                return null;

            PerkBase perk = Perks.GetRandom();
            RemovePerk(perk);
            return perk;
        }

        public void Tick()
        {
            if (!Parent.IsAlive)
                return;

            for (int i = 0; i < Perks.Count; i++)
                Perks[i]?.Tick();
        }
    }
}
