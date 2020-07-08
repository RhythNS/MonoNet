using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Graphics;
using MonoNet.Testing.Infrastructure;
using MonoNet.Util;
using MonoNet.Util.Pools;

namespace MonoNet.Testing.ECS
{
    public class InfraTestGame : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private GameSystemManager manager;
        private Stage stage;

        public InfraTestGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            new Log(Log.Level.PrintMessages, Log.Level.PrintMessagesAndStackTrace, Log.Level.PrintMessagesAndStackTrace);

            manager = new GameSystemManager();
            manager.Add(new Time(), new Input(), new Physic());

            stage = new Stage(5, new Pool<Actor>(50, 10));

            // various testing components
            stage.CreateActor(0).AddComponent<InputTesterComponent>();
            stage.CreateActor(0).AddComponent<CoroutineTest>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            float width = GraphicsDevice.Viewport.Width;
            float height = GraphicsDevice.Viewport.Height;

            Texture2D spritesheet = Content.Load<Texture2D>("Test/spritesheet");

            TextureRegion.CreateFromSheet(out TextureRegion[] right, spritesheet, 34, 34, new Point(0, 0), new Point(1, 0), new Point(2, 0), new Point(3, 0), new Point(4, 0));

            Animation<TextureRegion> loop = new Animation<TextureRegion>(right, 0.2f, Animation<TextureRegion>.PlaybackMode.NormalLoop);
            Animation<TextureRegion> revLopp = new Animation<TextureRegion>(right, 0.2f, Animation<TextureRegion>.PlaybackMode.ReversedLoop);

            Animation<TextureRegion> pingPong = new Animation<TextureRegion>(right, 0.2f, Animation<TextureRegion>.PlaybackMode.PingPongLoop);

            Actor loopActor = stage.CreateActor(0);
            loopActor.AddComponent<Transform2>().LocalPosition = new Vector2(width * 0.25f, height * 0.5f);
            loopActor.AddComponent<AnimatedTextureRegionComponent>().animation = loop;

            Actor revLoopActor = stage.CreateActor(0);
            revLoopActor.AddComponent<Transform2>().LocalPosition = new Vector2(width * 0.5f, height * 0.5f);
            revLoopActor.AddComponent<AnimatedTextureRegionComponent>().animation = revLopp;

            Actor pingPongActor = stage.CreateActor(0);
            pingPongActor.AddComponent<Transform2>().LocalPosition = new Vector2(width * 0.75f, height * 0.5f);
            pingPongActor.AddComponent<AnimatedTextureRegionComponent>().animation = pingPong;

            Texture2D testingLayers = Content.Load<Texture2D>("Test/testingLayers");
            TextureRegion[] layerRegions = TextureRegion.CreateAllFromSheet(testingLayers, 20, 20);

            /*for (int i = 0; i < layerRegions.Length-4; i++)
            {
                Actor layerActor = stage.CreateActor(i);
                Transform2 layerTrans = layerActor.AddComponent<Transform2>();
                layerTrans.WorldPosition = new Vector2(width * 0.5f, height * 0.25f);
                layerTrans.LocalScale = new Vector2(2f, 2f);
                layerActor.AddComponent<DrawTextureRegionComponent>().region = layerRegions[i];
                layerActor.AddComponent<GoRightComponent>().Set(20 + i * 60, width);
                layerActor.AddComponent<Rigidbody>();

                Actor childActor = stage.CreateActor(i);
                Transform2 childTrans = childActor.AddComponent<Transform2>();
                childTrans.LocalPosition = new Vector2(0, -50);
                childTrans.Parent = layerTrans;
                childTrans.LocalScale = new Vector2(0.5f, 0.5f);
                childActor.AddComponent<DrawTextureRegionComponent>().region = layerRegions[i];
                childActor.AddComponent<Rigidbody>();
            }*/

       
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
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            manager.Update(gameTime);

            if (Input.IsKeyDownThisFrame(Keys.Escape))
                Exit();

            stage.Update();

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            stage.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
