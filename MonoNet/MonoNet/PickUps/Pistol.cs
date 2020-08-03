using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoNet.PickUps
{
    class Pistol : Weapon
    {
        public override void CoreMethod()
        {
            /* Actor bullet = Actor.Stage.CreateActor(0);
            Rigidbody rb = bullet.AddComponent<Rigidbody>();
            rb.OnTriggerEnter += OnTriggerEnter; */
        }
    }
}
