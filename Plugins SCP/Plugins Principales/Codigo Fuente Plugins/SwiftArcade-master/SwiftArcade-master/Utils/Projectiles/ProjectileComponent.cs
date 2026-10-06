using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SwiftArcadeMode.Utils.Projectiles
{
    public class ProjectileComponent : MonoBehaviour
    {
        public ProjectileBase projectile;

        private void OnCollisionEnter(Collision cols) => projectile.OnCollide(cols);
    }
}
