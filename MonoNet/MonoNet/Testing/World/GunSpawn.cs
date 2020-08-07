using Microsoft.Xna.Framework;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Graphics;
using MonoNet.Tiled;
using System;
using System.Collections.Generic;
using TiledSharp;
using MonoNet.Player;
using MonoNet.PickUps;

namespace MonoNet.Testing.World
{
    public class GunSpawn
    {
        private TextureRegion[] regions;
        private Stage stage;
        private Random random;

        public GunSpawn(TextureRegion[] regions, Stage stage)
        {
            this.regions = regions;
            this.stage = stage;
            random = new Random();
        }

        public void OnObjectLoaded(List<TiledMapComponent> allMapComponents, TmxObject loadedObject)
        {
            if (loadedObject.Type.Equals("gunSpawn", StringComparison.CurrentCultureIgnoreCase) == false)
                return;

            Actor actor = stage.CreateActor(2);
            Transform2 trans = actor.AddComponent<ScaledTransform2>();
            trans.LocalPosition = new Vector2((float)loadedObject.X, (float)loadedObject.Y);
            Rigidbody body = actor.AddComponent<Rigidbody>();

            TextureRegion textureRegion = regions[random.Next(regions.Length - 1)]; // last one is empty
            body.Set(width: textureRegion.sourceRectangle.Width, height: textureRegion.sourceRectangle.Height, collisionLayer: 2, isStatic: false, isSquare: true, isTrigger: false);

            DrawTextureRegionComponent drawTexture = actor.AddComponent<DrawTextureRegionComponent>();
            drawTexture.region = textureRegion;

            Pistol pistol = actor.AddComponent<Pistol>();
        }
    }
}
