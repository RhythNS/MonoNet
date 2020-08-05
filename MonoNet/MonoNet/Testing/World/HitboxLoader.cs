﻿using Microsoft.Xna.Framework;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Graphics;
using MonoNet.Testing.Infrastructure;
using MonoNet.Tiled;
using MonoNet.Util;
using TiledSharp;

namespace MonoNet.Testing.World
{
    public class HitboxLoader
    {
        private Stage stage;
        private TextureRegion debugRegion;

        public HitboxLoader(Stage stage, TextureRegion debugRegion)
        {
            this.stage = stage;
            this.debugRegion = debugRegion;
        }

        private static int counter = -1;

        public void OnCollisionHitboxLoaded(TiledMapComponent onComponent, Transform2 componentTrans, TmxObject hitbox, float localX, float localY)
        {
            if (hitbox.ObjectType != TmxObjectType.Basic)
            {
                Log.Info("Can only load basic objects (not " + hitbox.ObjectType + ") into physics!" + (++counter));
                return;
            }

            Actor hitboxActor = stage.CreateActor(onComponent.Actor.Layer + 1);
            Transform2 trans = hitboxActor.AddComponent<ScaledTransform2>();
            trans.LocalPosition = new Vector2(localX + (float)hitbox.X, localY + (float)hitbox.Y);
            Rigidbody body = hitboxActor.AddComponent<Rigidbody>();
            body.Set(width: (float)hitbox.Width, height: (float)hitbox.Height, isStatic: true, isSquare: true, isTrigger: false);
            //DrawTextureRegionComponent drawTexture = hitboxActor.AddComponent<DrawTextureRegionComponent>();
            //drawTexture.region = debugRegion;
            trans.Parent = componentTrans;
        }
    }
}
