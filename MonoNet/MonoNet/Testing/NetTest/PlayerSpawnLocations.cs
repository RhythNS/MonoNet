using Microsoft.Xna.Framework;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Graphics;
using MonoNet.Player;
using MonoNet.Tiled;
using System;
using System.Collections.Generic;
using TiledSharp;

namespace MonoNet.Testing.NetTest
{
    public class PlayerSpawnLocations
    {
        private Random random;
        public static PlayerSpawnLocations Instance { get; private set; }

        public List<Vector2> SpawnLocations { get; private set; } = new List<Vector2>();
        private TextureRegion textureRegion;
        private Stage stage;

        public PlayerSpawnLocations()
        {
            Instance = this;
            random = new Random();
        }

        public PlayerSpawnLocations(TextureRegion textureRegion, Stage stage) : this()
        {
            this.textureRegion = textureRegion;
            this.stage = stage;
        }


        public void OnObjectLoaded(List<TiledMapComponent> allMapComponents, TmxObject loadedObject)
        {
            if (loadedObject.Type.Equals("playerSpawn", StringComparison.CurrentCultureIgnoreCase) == false)
                return;

            SpawnLocations.Add(new Vector2((float)loadedObject.X, (float)loadedObject.Y));
        }

        public void LoadOnePlayer()
        {
            Actor actor = stage.CreateActor(2);
            Transform2 trans = actor.AddComponent<ScaledTransform2>();
            Vector2 randomPos = GetRandomLocation();
            trans.LocalPosition = new Vector2(randomPos.X, randomPos.Y);
            Rigidbody body = actor.AddComponent<Rigidbody>();
            body.Set(width: textureRegion.sourceRectangle.Width, height: textureRegion.sourceRectangle.Height, collisionLayer: 1, isStatic: false, isSquare: true, isTrigger: false);
            DrawTextureRegionComponent drawTexture = actor.AddComponent<DrawTextureRegionComponent>();
            drawTexture.region = textureRegion;
            
            actor.AddComponent<PlayerManager>();
        }

        public Vector2 GetRandomLocation() => SpawnLocations[random.Next(SpawnLocations.Count)];
    }
}
