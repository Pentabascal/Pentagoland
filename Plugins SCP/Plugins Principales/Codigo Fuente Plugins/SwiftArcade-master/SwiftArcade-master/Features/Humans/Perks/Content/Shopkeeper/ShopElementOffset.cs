using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Shopkeeper
{
    public abstract class ShopElementOffset(Vector3 offset) : ShopElement
    {
        public readonly Vector3 Offset = offset;

        public virtual Vector3 Position => Parent.Shop.Position + Parent.Shop.Rotation * Offset;
        public virtual Quaternion Rotation => Parent.Shop.Rotation;
    }
}
