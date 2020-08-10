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
using Myra;

namespace MonoNet.Testing.ECS
{
    public class InfraTestGame : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private GameSystemManager manager;
        private Stage stage;

        private Camera camera;

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

            camera = new Camera(GraphicsDevice.Viewport);

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
            loopActor.AddComponent<AnimatedTextureRegionComponent>().Set(loop);

            Actor revLoopActor = stage.CreateActor(0);
            revLoopActor.AddComponent<Transform2>().LocalPosition = new Vector2(width * 0.5f, height * 0.5f);
            revLoopActor.AddComponent<AnimatedTextureRegionComponent>().Set(revLopp);

            Actor pingPongActor = stage.CreateActor(0);
            pingPongActor.AddComponent<Transform2>().LocalPosition = new Vector2(width * 0.75f, height * 0.5f);
            pingPongActor.AddComponent<AnimatedTextureRegionComponent>().Set(pingPong);

            Texture2D testingLayers = Content.Load<Texture2D>("Test/testingLayers");
            TextureRegion[] layerRegions = TextureRegion.CreateAllFromSheet(testingLayers, 20, 20);
            
            MyraEnvironment.Game = this;

            stage.CreateActor(0).AddComponent<CameraTestComponent>().camera = camera;
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

            spriteBatch.Begin(transformMatrix: camera.GetViewMatrix());
            stage.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
