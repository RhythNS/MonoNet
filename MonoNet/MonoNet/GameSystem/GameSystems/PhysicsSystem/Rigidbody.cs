using Microsoft.Xna.Framework;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.Util.Datatypes;
using MonoNet.Util.Overlap;
using System;

namespace MonoNet.GameSystems.PhysicsSystem
{
    public delegate void OnTriggerEnter(Rigidbody other);
    public delegate void OnTriggerStay(Rigidbody other);
    public delegate void OnTriggerExit(Rigidbody other);

    public class Rigidbody : Component, Interfaces.IUpdateable, IDisposable, IOverlapable
    {
        public event OnTriggerEnter OnTriggerEnter;
        public event OnTriggerStay OnTriggerStay;
        public event OnTriggerExit OnTriggerExit;

        private static float gConst = 600f;

        public Vector2 velocity = Vector2.Zero;
        private Vector2 prevPos = Vector2.Zero;

        public int collisionLayer;

        public float height;
        public float width;

        public bool isStatic;
        public bool isGrounded;
        public bool isSquare = true;
        public bool isTrigger = false;
        private bool registered = false;

        private Transform2 transform;

        protected override void OnInitialize()
        {
            transform = Actor.GetComponent<Transform2>();
        }

        public void Set(float width = 1, float height = 1, int collisionLayer = 0, bool isStatic = false, bool isSquare = true, bool isTrigger = false)
        {
            this.width = width;
            this.height = height;
            this.collisionLayer = collisionLayer;
            this.isStatic = isStatic;
            this.isSquare = isSquare;
            this.isTrigger = isTrigger;

            if (registered == true)
                Physic.Instance.DeRegister(this);
            registered = true;

            Physic.Instance.Register(this);
        }

        public void Update()
        {
            if (isStatic == false && isGrounded == false)
            {
                velocity.Y += gConst * Time.Delta;
            }

        }

        public void Dispose()
        {
            Physic.Instance.DeRegister(this);
        }

        public void FireEventEnter(Rigidbody other) => OnTriggerEnter?.Invoke(other);

        public void FireEventStay(Rigidbody other) => OnTriggerStay?.Invoke(other);

        public void FireEventExit(Rigidbody other) => OnTriggerExit?.Invoke(other);

        public Box2D GetBox()
        {
            Vector2 position = transform.WorldPosition;
            Vector2 scale = transform.WorldScale;
            scale.X *= width;
            scale.Y *= height;
            return new Box2D(position, scale);
        }

        public bool Overlaps(IOverlapable other) => GetBox().Intersecting(other.GetBox());

        public bool Overlaps(Box2D other) => GetBox().Intersecting(other);
    }
}
