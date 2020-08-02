using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Graphics;
using MonoNet.Testing.Infrastructure;

namespace MonoNet.Testing.Physics
{
    public class PhysicTest : TestGame
    {

        protected override void LoadContent()
        {
            base.LoadContent();

            float width = GraphicsDevice.Viewport.Width;
            float height = GraphicsDevice.Viewport.Height;

            Texture2D testingLayers = Content.Load<Texture2D>("Test/testingLayers");
            TextureRegion[] layerRegions = TextureRegion.CreateAllFromSheet(testingLayers, 20, 20);

            for (int i = 0; i < 5; i++)
            {
                Actor physActor = stage.CreateActor(0);
                Transform2 physTrans = physActor.AddComponent<Transform2>();
                physTrans.WorldPosition = new Vector2(300, height * 0.2f * i);
                physTrans.LocalScale = new Vector2(1f, 1f);
                physActor.AddComponent<Rigidbody>().velocity = new Vector2(20 * i, 0);
                physActor.AddComponent<DrawTextureRegionComponent>().region = layerRegions[i];
                physActor.GetComponent<Rigidbody>().height = layerRegions[0].sourceRectangle.Height;
                physActor.GetComponent<Rigidbody>().width = layerRegions[0].sourceRectangle.Width;
                physActor.GetComponent<Rigidbody>().isSquare = true;
            }

            Actor triggerBox = stage.CreateActor(0);
            Transform2 triggerTrans = triggerBox.AddComponent<Transform2>();
            triggerTrans.WorldPosition = new Vector2(300, height * 0.6f);
            triggerBox.AddComponent<DrawTextureRegionComponent>().region = layerRegions[0];
            Rigidbody body = triggerBox.AddComponent<Rigidbody>();
            body.width = layerRegions[0].sourceRectangle.Width;
            body.height = layerRegions[0].sourceRectangle.Height;
            body.isTrigger = true;
            body.isStatic = true;
            triggerBox.AddComponent<TriggerTest>();
        }
    }
}
