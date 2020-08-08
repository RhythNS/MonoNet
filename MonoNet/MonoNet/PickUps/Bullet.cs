using Microsoft.Xna.Framework;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Player;

namespace MonoNet.PickUps
{
    public class Bullet : Component, Interfaces.IUpdateable
    {
        private Transform2 transform;
        public PlayerManager shooter;

        protected override void OnInitialize()
        {
            Actor.GetComponent<Rigidbody>().OnCollision += OnCollision;
            transform = Actor.GetComponent<Transform2>();
        }

        public void Set(float velocity, PlayerManager shooter)
        {
            Vector2 direction = shooter.GetLookingVector();
            direction.Normalize();

            Actor.GetComponent<Rigidbody>().velocity += direction * velocity;
            Actor.AddComponent<Bullet>().shooter = shooter;
        }

        public void Update()
        {
            if (OfScreen())
            {
                Actor.Stage.DeleteActor(Actor);
            }
        }

        private void OnCollision(Rigidbody other)
        {
            if (other.Actor.TryGetComponent(out PlayerManager player))
            {
                if (player == shooter)
                    return;

                player.TakeDamage();
            }
            Actor.Stage.DeleteActor(Actor);
        }

        private bool OfScreen()
        {
            return false;
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
