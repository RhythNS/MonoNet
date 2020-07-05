using Microsoft.Xna.Framework;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.Tiled;
using System;

namespace MonoNet.Testing
{
    public class WorldTest : TestGame
    {
        protected override int LayersForStage => 2;

        protected override void LoadContent()
        {
            base.LoadContent();

            Actor baseActor = stage.CreateActor(0);
            TiledBase tiledBase = baseActor.AddComponent<TiledBase>();
            tiledBase.Set(Content);

            TiledMapComponent[] components = tiledBase.AddMap(stage, "Test/FinalWorld", true);

            float width = components[0].Width * components[0].TileWidth * 0.5f;
            float height = components[0].Height * components[0].TileHeight * 0.5f;

            Array.ForEach(components, x => x.Actor.GetComponent<Transform2>().WorldPosition = new Vector2(-width, -height));
        }

    }
}
