using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Player;

namespace MonoNet.PickUps
{
    public class Bullet : Component, Interfaces.IUpdateable
    {
        private Transform2 transform;

        protected override void OnInitialize()
        {
            Actor.GetComponent<Rigidbody>().OnTriggerEnter += OnTriggerEnter;
            transform = Actor.GetComponent<Transform2>();
        }

        public void Update()
        {
            if (OfScreen())
            {
                Actor.Stage.DeleteActor(Actor);
            }
        }

        public void OnTriggerEnter(Rigidbody other)
        {
            if (other.Actor.TryGetComponent(out PlayerManager player))
            {
                player.TakeDamage();
            }
            Actor.Stage.DeleteActor(Actor);
        }

        private bool OfScreen()
        {
            bool ofScreen = false;
            float dist;

            dist = transform.WorldPosition.X * transform.WorldPosition.X + transform.WorldPosition.Y * transform.WorldPosition.Y;

            if (dist > 50000)
            {
                ofScreen = true;
            }
            return ofScreen;
        }
    }
}
