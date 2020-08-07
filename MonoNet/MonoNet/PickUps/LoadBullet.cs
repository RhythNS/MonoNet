using Microsoft.Xna.Framework;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Graphics;

namespace MonoNet.PickUps
{
    public static class LoadBullet
    {
        public static TextureRegion region;
        public static Stage stage;

        public static void Shoot(Vector2 direction, float velocity, Actor actor)
        {
            direction.Normalize();
            Actor bullet = actor.Stage.CreateActor(actor.Layer);
            bullet.AddComponent<Transform2>();
            bullet.GetComponent<Transform2>().WorldPosition = actor.GetComponent<Transform2>().WorldPosition + Vector2.UnitX * 50f;
            Rigidbody body = bullet.AddComponent<Rigidbody>();
            body.velocity += direction * velocity * Time.Delta;
            DrawTextureRegionComponent drawTexture = bullet.AddComponent<DrawTextureRegionComponent>();
            drawTexture.region = region;
            body.Set(width: 20, height: 20, collisionLayer: 2, isStatic: false, isSquare: true, isTrigger: true);
            bullet.AddComponent<Bullet>();

        }
    }
}
