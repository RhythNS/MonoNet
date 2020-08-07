using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Graphics;
using MonoNet.Testing;
using MonoNet.Testing.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
            bullet.AddComponent<DrawTextureRegionComponent>();
            DrawTextureRegionComponent drawTexture = bullet.AddComponent<DrawTextureRegionComponent>();
            drawTexture.region = region;
            body.Set(width: 20, height: 20, collisionLayer: 2, isStatic: false, isSquare: true, isTrigger: true);
            bullet.AddComponent<Bullet>();

        }
    }
}
