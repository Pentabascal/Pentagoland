using LabApi.Features.Wrappers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace SwiftArcadeMode.Utils.Effects
{
    public class CustomEffectContainer(Player p)
    {
        public readonly Player Player = p;

        public readonly List<CustomEffectBase> ActiveEffects = [];

        public CustomEffectContainer(ReferenceHub refHub) : this(Player.Get(refHub)) { }

        public void Tick()
        {
            for (int i = ActiveEffects.Count - 1; i >= 0; i--)
            {
                ActiveEffects[i].Tick();
                ActiveEffects[i].EffectTimer.Tick(Time.fixedDeltaTime);

                if (ActiveEffects[i].EffectTimer.Ended)
                    RemoveEffect(i);
            }
        }

        public void ClearEffects()
        {
            for (int i = ActiveEffects.Count - 1; i >= 0; i--)
                RemoveEffect(i);
        }

        public void RemoveEffect(int i)
        {
            ActiveEffects[i].Remove();
            ActiveEffects.RemoveAt(i);
        }

        public void RemoveEffect(CustomEffectBase eff)
        {
            if (!ActiveEffects.Contains(eff))
                return;

            eff.Remove();
            ActiveEffects.Remove(eff);
        }

        public void AddEffect(CustomEffectBase effect)
        {
            effect.Init(this);

            bool canAdd = true;

            List<CustomEffectBase> existing = [.. ActiveEffects.Where(c => c.GetType() == effect.GetType())];

            if (existing.Count > 0)
            {
                if (effect.StackCount >= existing.Count)
                    RemoveEffect(existing[0]);
                else
                    canAdd = effect.Stack(existing[0]);
            }

            if (canAdd)
            {
                ActiveEffects.Add(effect);
                effect.Add();
            }
        }
    }
}
