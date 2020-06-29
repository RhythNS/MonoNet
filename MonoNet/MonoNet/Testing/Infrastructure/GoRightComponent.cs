using Microsoft.Xna.Framework;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems;

namespace MonoNet.Testing.Infrastructure
{
    class GoRightComponent : Component, Interfaces.IUpdateable
    {
        private float speed, resetAt;
        private Transform2 transform;

        protected override void OnInitialize()
        {
            Actor.GetComponent(out transform);
        }

        public void Set(float speed, float clampAt)
        {
            this.speed = speed;
            this.resetAt = clampAt;
        }

        public void Update()
        {
            Vector2 pos = transform.WorldPosition;
            pos.X = transform.WorldPosition.X + (speed * Time.Delta);
            if (pos.X > resetAt)
                pos.X = 0;
            transform.WorldPosition = pos;
        }
    }
}
