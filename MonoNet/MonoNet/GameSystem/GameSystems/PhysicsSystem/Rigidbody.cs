using MonoNet.ECS;
using MonoNet.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoNet.ECS.Components;

namespace MonoNet.GameSystems.PhysicsSystem
{
    public class Rigidbody : Component, Interfaces.IUpdateable, IDisposable
    {
        protected override void OnInitialize()
        {
            Physic.Instance.Register(this);
        }

        private static float gConst = 98.1f;

        public Vector2 velocity = Vector2.Zero;
        public int height;
        public int width;
        public bool isStatic;
        public bool grounded;
        public bool isSquare = true;

        public void Update()
        {
            if (isStatic == false)
                velocity.Y += gConst * Time.Delta;
        }

        public void Dispose()
        {
            Physic.Instance.DeRegister(this);
        }


    }
}
