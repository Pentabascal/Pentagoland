using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SwiftArcadeMode.Utils.Primitives
{
    public class ToyGroup(params ToyBase[] primitives)
    {
        public readonly List<ToyBase> Toys = [.. primitives];
        public virtual void Spawn()
        {
            for (int i = 0; i < Toys.Count; i++)
                Toys[i].Spawn();
        }
        public virtual void Tick()
        {
            for (int i = 0; i < Toys.Count; i++)
                Toys[i].Tick();
        }
        public virtual void Destroy()
        {
            for (int i = 0; i < Toys.Count; i++)
                Toys[i].Destroy();
        }
    }

    public class ToyBase(AdminToy toy)
    {
        public AdminToy Toy { get; private set; } = toy;
        public Action TickAction { get; set; }

        public virtual void Spawn()
        {
            if (Toy != null)
                return;

            Init();
            Toy.Spawn();
        }

        public virtual void Init() { }
        public virtual void Tick() => TickAction?.Invoke();

        public virtual void Destroy() => Toy?.Destroy();

        public static implicit operator ToyBase(AdminToy toy) => new(toy);
    }
}
