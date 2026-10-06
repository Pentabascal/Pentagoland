using SwiftArcadeMode.Utils.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SwiftArcadeMode.Utils.Structures
{
    [Serializable]
    public class ModifiableStatistic : IOverrideParent
    {
        [field: SerializeField]
        public float BaseValue { get; private set; } = 1f;

        [field: SerializeField]
        public List<Modifier> Values { get; private set; } = [];

        public event Action<float> OnUpdate;
        public virtual void UpdateValue()
        {
            IsDirty = true;
            OnUpdate?.Invoke(GetValue());
        }

        public readonly List<ModifiableStatistic> Additives = [];

        public ModifiableStatistic() { }

        public ModifiableStatistic(float baseValue) { BaseValue = baseValue; }

        public bool IsDirty { get; set; } = true;
        float cache;

        public float GetValue()
        {
            if (!IsDirty)
                return cache;

            float value = BaseValue;

            if (Values.Count > 0)
                for (int i = 0; i < Values.Count; i++)
                    value = Values[i].Modify(value);

            if (Additives.Count > 0)
                for (int i = 0; i < Additives.Count; i++)
                    for (int j = 0; j < Additives[i].Values.Count; j++)
                        value = Additives[i].Values[j].Modify(value);

            IsDirty = false;
            cache = value;
            return value;
        }

        public Modifier AddModifier()
        {
            Modifier modifier = new(this);
            Values.Add(modifier);
            IsDirty = true;
            return modifier;
        }

        public void RemoveOverride(object target)
        {
            Values.Remove((Modifier)target);
            IsDirty = true;
        }

        public static implicit operator float(ModifiableStatistic stat) => stat.GetValue();

        [Serializable]
        public class Modifier(ModifiableStatistic parent) : OverrideBase<ModifiableStatistic>(parent)
        {
            public float Value
            {
                get => value;
                set
                {
                    this.value = value;
                    Parent.UpdateValue();
                }
            }
            [SerializeField]
            float value;

            [field: SerializeField]
            public ModifierType Type { get; set; }

            public float Modify(float value) => Type switch
            {
                ModifierType.Addition => value + Value,
                ModifierType.Subtraction => value - Value,
                ModifierType.Division => value / Value,
                ModifierType.Multiplication => value * Value,
                ModifierType.Mutation => Value,
                _ => value,
            };
        }

        public enum ModifierType
        {
            Addition,
            Subtraction,
            Multiplication,
            Division,
            Mutation
        }
    }
}
