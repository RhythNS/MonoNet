using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems;
using MonoNet.Interfaces;

namespace MonoNet.Testing.Infrastructure
{
    class GoRightComponent : Component, IUpdateable
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
            transform.position.X = transform.position.X + (speed * Time.Delta);
            if (transform.position.X > resetAt)
                transform.position.X = 0;
        }
    }
}
