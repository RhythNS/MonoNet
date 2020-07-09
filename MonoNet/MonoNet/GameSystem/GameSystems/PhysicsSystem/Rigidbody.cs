using Microsoft.Xna.Framework;
using MonoNet.ECS;
using System;

namespace MonoNet.GameSystems.PhysicsSystem
{
    public delegate void OnTriggerEnter(Rigidbody other);
    public delegate void OnTriggerStay(Rigidbody other);
    public delegate void OnTriggerExit(Rigidbody other);

    public class Rigidbody : Component, Interfaces.IUpdateable, IDisposable
    {
        public event OnTriggerEnter OnTriggerEnter;
        public event OnTriggerStay OnTriggerStay;
        public event OnTriggerExit OnTriggerExit;

        protected override void OnInitialize()
        {
            Physic.Instance.Register(this);
        }

        private static float gConst = 98.1f;

        public Vector2 velocity = Vector2.Zero;
        public float height;
        public float width;
        public bool isStatic;
        public bool grounded;
        public bool isSquare = true;
        public bool isTrigger = false;

        public void Update()
        {
            if (isStatic == false)
                velocity.Y += gConst * Time.Delta;
        }

        public void Dispose()
        {
            Physic.Instance.DeRegister(this);
        }

        public void FireEventEnter(Rigidbody other) => OnTriggerEnter?.Invoke(other);

        public void FireEventStay(Rigidbody other) => OnTriggerStay?.Invoke(other);

        public void FireEventExit(Rigidbody other) => OnTriggerExit?.Invoke(other);

    }
}
