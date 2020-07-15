using Microsoft.Xna.Framework;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.Testing.Tiled;
using MonoNet.Tiled;

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

            Actor baseActor = stage.CreateActor(0);
            TiledBase tiledBase = baseActor.AddComponent<TiledBase>();
            tiledBase.Set(Content);

            TiledMapComponent[] components = tiledBase.AddMap(stage, "Test/FinalWorld", true, true);

            float width = components[0].Width * components[0].TileWidth * 0.5f;
            float height = components[0].Height * components[0].TileHeight * 0.5f;

            components[0].Actor.GetComponent<SharedPositionTransform2>().WorldPosition = new Vector2(-width, -height);
            components[0].Actor.AddComponent<TiledTestMoverAndScaler>();
        }

    }
}
