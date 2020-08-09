using Microsoft.Xna.Framework;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Graphics;
using MonoNet.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledSharp;

namespace MonoNet.Testing.World
{
    public class BoxSpawn
    {
        private TextureRegion textureRegion;
        private Stage stage;

        public BoxSpawn(TextureRegion textureRegion, Stage stage)
        {
            this.textureRegion = textureRegion;
            this.stage = stage;
        }

        public void OnObjectLoaded(List<TiledMapComponent> allMapComponents, TmxObject loadedObject)
        {
            if (loadedObject.Type.Equals("boxSpawn", StringComparison.CurrentCultureIgnoreCase) == false)
                return;

            Actor actor = stage.CreateActor(2);
            Transform2 trans = actor.AddComponent<ScaledTransform2>();
            trans.LocalPosition = new Vector2((float)loadedObject.X, (float)loadedObject.Y);
            Rigidbody body = actor.AddComponent<Rigidbody>();
            body.Set(width: textureRegion.sourceRectangle.Width, height: textureRegion.sourceRectangle.Height, isStatic: false, isSquare: true, isTrigger: false);
            DrawTextureRegionComponent drawTexture = actor.AddComponent<DrawTextureRegionComponent>();
            drawTexture.region = textureRegion;
        }
    }
}
