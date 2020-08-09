using Microsoft.Xna.Framework.Graphics;
using MonoNet.ECS;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Graphics;
using MonoNet.LevelManager;
using MonoNet.Testing.NetTest;
using MonoNet.Testing.World;
using MonoNet.Tiled;
using MonoNet.Util.Datatypes;

namespace MonoNet.Testing
{
    public class WorldTest : TestGame
    {
        protected override int LayersForStage => 2;

        protected override void LoadContent()
        {
            base.LoadContent();
            ComponentFactory factory = new ComponentFactory(Content);

            /* 
            graphics.PreferredBackBufferWidth = 1920;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = 1080;   // set this value to the desired height of your window
            graphics.ApplyChanges();

            TextureRegion orangeRegion = new TextureRegion(Content.Load<Texture2D>("Test/orangeSquare"), 0, 0, 32, 32);

            HitboxLoader hitboxLoader = new HitboxLoader(stage);
            BoxSpawn boxSpawn = new BoxSpawn(factory.playerTex, stage);
            PlayerSpawnLocations playerSpawn = new PlayerSpawnLocations(factory.playerTex, stage);
            GunSpawn gunSpawn = new GunSpawn(factory.gunRegions, stage);

            Physic.Instance.collisionRules.Add(new MultiKey<int>(1, 2), false);

            Actor tiledBaseActor = stage.CreateActor(0);
            TiledBase tiledBase = tiledBaseActor.AddComponent<TiledBase>();
            tiledBase.Set(Content);
            tiledBase.OnCollisionHitboxLoaded += hitboxLoader.OnCollisionHitboxLoaded;
            tiledBase.OnObjectLoaded += boxSpawn.OnObjectLoaded;
            tiledBase.OnObjectLoaded += playerSpawn.OnObjectLoaded;
            tiledBase.OnObjectLoaded += gunSpawn.OnObjectLoaded;

            TiledMapComponent[] components = tiledBase.AddMap(stage, "maps/level1", true, true);

            playerSpawn.LoadOnePlayer();

            //float width = components[0].Width * components[0].TileWidth * 0.5f;
            //float height = components[0].Height * components[0].TileHeight * 0.5f;

            //components[0].Actor.GetComponent<SharedPositionTransform2>().WorldPosition = new Vector2(-width, -height);
        */
        }

    }
}
