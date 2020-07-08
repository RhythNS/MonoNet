using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.Graphics;
using MonoNet.Tiled;

namespace MonoNet.Testing.Tiled
{
    class TiledTestGame : TestGame
    {
        protected override void LoadContent()
        {
            base.LoadContent();

            Actor baseActor = stage.CreateActor(0);
            TiledBase tiledBase = baseActor.AddComponent<TiledBase>();
            tiledBase.Set(Content);
            
            TextureRegion region = new TextureRegion(Content.Load<Texture2D>("Test/testingLayers"), 0, 0, 20, 20);
            tiledBase.OnObjectLoaded += new TiledTestPlayerLoader(stage, region).OnObjectLoaded;

            Actor mapActor = stage.CreateActor(0);
            mapActor.AddComponent<Transform2>().WorldPosition = new Vector2(0, 0);
            tiledBase.AddMap(mapActor, "Test/DatMapWithObjectLayer");
            mapActor.AddComponent<TiledTestMoverAndScaler>();
        }
    }
}
