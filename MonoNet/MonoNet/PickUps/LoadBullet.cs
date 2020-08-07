using Microsoft.Xna.Framework;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Graphics;
using MonoNet.Player;

namespace MonoNet.PickUps
{
    public static class LoadBullet
    {
        public static TextureRegion region;
        public static Stage stage;

        public static void Shoot(Vector2 direction, float velocity, PlayerManager shooter)
        {
            direction.Normalize();
            Actor shooterActor = shooter.Actor;
            Actor bullet = shooterActor.Stage.CreateActor(shooterActor.Layer);
            bullet.AddComponent<Transform2>();
            bullet.GetComponent<Transform2>().WorldPosition = shooterActor.GetComponent<Transform2>().WorldPosition + Vector2.UnitX * 50f;
            Rigidbody body = bullet.AddComponent<Rigidbody>();
            DrawTextureRegionComponent drawTexture = bullet.AddComponent<DrawTextureRegionComponent>();
            drawTexture.region = region;
            body.Set(width: 20, height: 20, collisionLayer: 2, isStatic: false, isSquare: true, isTrigger: false, ignoreGravity: true);
            body.velocity += direction * velocity;
            bullet.AddComponent<Bullet>().shooter = shooter;
        }
    }
}
