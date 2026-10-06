using LabApi.Features.Wrappers;
using SwiftArcadeMode.Utils.Extensions;
using SwiftArcadeMode.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ReflectionExtensions = SwiftArcadeMode.Utils.Extensions.ReflectionExtensions;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.SixthSense
{
    [Perk("SixthSense", Rarity.Uncommon)]
    public class SixthSense(PerkInventory inv) : PerkTriggerCooldownBase(inv)
    {
        public static HashSet<Type> SenseCache
        {
            get
            {
                _senseCache ??= ReflectionExtensions.GetAllNonAbstractSubclasses<SenseBase>();
                return _senseCache;
            }
        }

        private static HashSet<Type> _senseCache;

        public readonly List<SenseBase> Senses = [];

        public override string Name => "Sexto sentido";

        public override string Description => "Te proporciona información poco clara, pero útil, sobre tus enemigos.";

        public override string PerkDescription => "";

        public override float Cooldown => 10f;

        public override string ReadyMessage
        {
            get
            {
                checkedBases.Clear();
                SenseBase get() => Senses.Where((p) => !checkedBases.Contains(p)).ToList().GetRandom();
                SenseBase sense = get();
                while (sense != null && !sense.Message(out string msg))
                {
                    checkedBases.Add(sense);
                    sense = get();
                }

                return sense != null && sense.Message(out string m) ? m : "";
            }
        }

        private readonly List<SenseBase> checkedBases = [];

        public override void Init()
        {
            base.Init();
            foreach (Type t in SenseCache)
                Senses.Add((SenseBase)Activator.CreateInstance(t, this));
        }

        public override void Effect() { }

        public static void RegisterSenses()
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();

            List<Type> types = [.. callingAssembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(SenseBase).IsAssignableFrom(t))];

            foreach (Type t in types)
                if (!SenseCache.Contains(t))
                    SenseCache.Add(t);
        }
    }

    public abstract class SenseBase(SixthSense parent) : IWeight
    {
        public SixthSense Parent { get; private set; } = parent;

        public Player Player => Parent.Player;

        public virtual int Weight => 1;

        public abstract bool Message(out string msg);
    }
}
