using Microsoft.Xna.Framework;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Graphics;
using MonoNet.LevelManager;
using MonoNet.Network;
using MonoNet.Player;

namespace MonoNet.PickUps
{
    public class Bullet : Component, Interfaces.IUpdateable
    {
        private Transform2 transform;
        public PlayerManager shooter;
        private Vector2 size;

        protected override void OnInitialize()
        {
            Actor.GetComponent<Rigidbody>().OnCollision += OnCollision;
            transform = Actor.GetComponent<Transform2>();
        }

        public void Set(float velocity, PlayerManager shooter)
        {
            Vector2 direction = shooter.GetLookingVector();
            direction.Normalize();

            Rigidbody body = Actor.GetComponent<Rigidbody>();
            body.velocity += direction * velocity;
            Rigidbody[] ignore = new Rigidbody[1];
            ignore[0] = shooter.Actor.GetComponent<Rigidbody>();
            body.IgnoreBodies = ignore;
            Actor.GetComponent<Bullet>().shooter = shooter;
            Rectangle rec = Actor.GetComponent<DrawTextureRegionComponent>().region.sourceRectangle;
            size = new Vector2(rec.Width, rec.Height);
        }

        public void Update()
        {
            if (IsOutOfBounds())
                ServerConnectionComponent.Instance.DestroySyncable(Actor.GetComponent<NetSyncComponent>().Id);
        }

        private void OnCollision(Rigidbody other)
        {
            if (other.Actor.TryGetComponent(out PlayerManager player))
            {
                if (player == shooter)
                    return;

                player.TakeDamage();
            }
            ServerConnectionComponent.Instance.DestroySyncable(Actor.GetComponent<NetSyncComponent>().Id);
        }

        private bool IsOutOfBounds()
        {
            Vector2 screenDims = GameManager.screenDimensions;
            Vector2 pos = transform.WorldPosition;

            return pos.X + size.X < 0 || pos.X > screenDims.X || pos.Y + size.Y > screenDims.Y || pos.Y < 0;
        }
    }
}
