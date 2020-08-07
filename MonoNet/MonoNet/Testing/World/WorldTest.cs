using Microsoft.Xna.Framework.Graphics;
using MonoNet.ECS;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Graphics;
using MonoNet.Testing.Infrastructure;
using MonoNet.Testing.Tiled;
using MonoNet.PickUps;
using MonoNet.Testing.World;
using MonoNet.Tiled;
using MonoNet.Util.Datatypes;
using MonoNet.Testing.NetTest;

namespace MonoNet.Testing
{
    public class WorldTest : TestGame
    {
        protected override int LayersForStage => 2;

        protected override void LoadContent()
        {
            base.LoadContent();

            graphics.PreferredBackBufferWidth = 1920;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = 1080;   // set this value to the desired height of your window
            graphics.ApplyChanges();

            TextureRegion orangeRegion = new TextureRegion(Content.Load<Texture2D>("Test/orangeSquare"), 0, 0, 32, 32);
            TextureRegion[] gunRegions = TextureRegion.CreateAllFromSheet(Content.Load<Texture2D>("Test/guns"), 32, 15);
            TextureRegion playerRegion = new TextureRegion(Content.Load<Texture2D>("Test/testingLayers"), 0, 0, 20, 20);
            TextureRegion bulletRegion = new TextureRegion(Content.Load<Texture2D>("Test/testingLayers"), 0, 0, 20, 20);

            HitboxLoader hitboxLoader = new HitboxLoader(stage, orangeRegion);
            BoxSpawn boxSpawn = new BoxSpawn(playerRegion, stage);
            PlayerSpawnLocations playerSpawn = new PlayerSpawnLocations(playerRegion, stage);
            GunSpawn gunSpawn = new GunSpawn(gunRegions, stage);
            LoadBullet.region = bulletRegion;
            LoadBullet.stage = stage;

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
        }
   
    }
}
