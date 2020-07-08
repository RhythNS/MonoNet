using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems;
using MonoNet.Graphics;
using MonoNet.Testing.Infrastructure;
using MonoNet.Tiled;
using System;
using TiledSharp;

namespace MonoNet.Testing.Tiled
{
    public class TiledTestPlayerLoader
    {
        private Stage stage;
        private readonly TextureRegion textureRegion;

        public TiledTestPlayerLoader(Stage stage, TextureRegion textureRegion)
        {
            this.stage = stage;
            this.textureRegion = textureRegion;
        }

        public void OnObjectLoaded(TiledMapComponent onMap, TmxObject loadedObject)
        {
            if (loadedObject.Type.Equals("player", StringComparison.CurrentCultureIgnoreCase))
            {
                Actor playerActor = stage.CreateActor(1);
                playerActor.AddComponent<Transform2>().WorldPosition = onMap.TiledToWorld(new Vector2((float)loadedObject.X, (float)loadedObject.Y));
                playerActor.AddComponent<TiledTestPlayer>();
                playerActor.AddComponent<DrawTextureRegionComponent>().region = textureRegion;
            }
        }
    }

    class TiledTestPlayer : Component, Interfaces.IUpdateable
    {
        private Transform2 transform;

        protected override void OnInitialize()
        {
            transform = Actor.GetComponent<Transform2>();
        }

        public void Update()
        {
            Vector2 position = transform.LocalPosition;
            if (Input.KeyDown(Keys.Up))
                position.Y += 20 * Time.Delta;
            if (Input.KeyDown(Keys.Left))
                position.X -= 20 * Time.Delta;
            if (Input.KeyDown(Keys.Down))
                position.Y -= 20 * Time.Delta;
            if (Input.KeyDown(Keys.Right))
                position.X += 20 * Time.Delta;
            transform.LocalPosition = position;
        }
    }
}
