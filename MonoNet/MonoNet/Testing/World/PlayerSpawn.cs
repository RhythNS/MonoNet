using Microsoft.Xna.Framework;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Graphics;
using MonoNet.Testing.Infrastructure;
using MonoNet.Tiled;
using System;
using System.Collections.Generic;
using TiledSharp;
using MonoNet.Player;
using MonoNet.Testing.NetTest;

namespace MonoNet.Testing.World
{
    public class PlayerSpawn
    {
        private TextureRegion textureRegion;
        private Stage stage;

        public Actor playerActor;

        public PlayerSpawn(TextureRegion textureRegion, Stage stage)
        {
            this.textureRegion = textureRegion;
            this.stage = stage;
        }

        public void OnObjectLoaded(List<TiledMapComponent> allMapComponents, TmxObject loadedObject)
        {
            if (loadedObject.Type.Equals("playerSpawn", StringComparison.CurrentCultureIgnoreCase) == false)
                return;

            Actor actor = stage.CreateActor(2);
            Transform2 trans = actor.AddComponent<ScaledTransform2>();
            trans.LocalPosition = new Vector2((float)loadedObject.X, (float)loadedObject.Y);
            //Rigidbody body = actor.AddComponent<Rigidbody>();
            //body.Set(width: textureRegion.sourceRectangle.Width, height: textureRegion.sourceRectangle.Height, collisionLayer: 1, isStatic: false, isSquare: true, isTrigger: false);
            actor.AddComponent<SimpleMoveComponent>();
            DrawTextureRegionComponent drawTexture = actor.AddComponent<DrawTextureRegionComponent>();
            drawTexture.region = textureRegion;

            playerActor = actor;
        }
    }
}
