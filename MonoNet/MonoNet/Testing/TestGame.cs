using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoNet.ECS;
using MonoNet.GameSystems;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Graphics;
using MonoNet.Network.Commands;
using MonoNet.Testing.Infrastructure;
using MonoNet.Util;
using MonoNet.Util.Pools;

namespace MonoNet.Testing
{
    /// <summary>
    /// Class used for extending a test so one does not have to deal with basic Game setup.
    /// </summary>
    public abstract class TestGame : Game
    {
        protected GraphicsDeviceManager graphics;
        protected SpriteBatch spriteBatch;
        protected GameSystemManager manager;
        protected Stage stage;
        protected Camera camera;

        protected virtual int LayersForStage => 5;
        protected virtual bool CreateCameraTestComponent => true;

        public TestGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            new EventHandlerDictionary();
            new Log(Log.Level.PrintMessages, Log.Level.PrintMessagesAndStackTrace, Log.Level.PrintMessagesAndStackTrace);

            manager = new GameSystemManager();
            manager.Add(new Time(), new Input(), new Physic());

            stage = new Stage(LayersForStage, new Pool<Actor>(50, 10));

            camera = new Camera(GraphicsDevice.Viewport);

            if (CreateCameraTestComponent == true)
                stage.CreateActor(0).AddComponent<CameraTestComponent>().camera = camera;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            PreUpdate(gameTime);

            manager.Update(gameTime);

            AfterManagerPreStageUpdate(gameTime);

            if (Input.IsKeyDownThisFrame(Keys.Escape))
                Exit();
            stage.Update();

            AfterUpdate(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// Called before every system and stage is updated.
        /// </summary>
        protected virtual void PreUpdate(GameTime time) { }

        /// <summary>
        /// Called after every system and before stage is updated.
        /// </summary>
        protected virtual void AfterManagerPreStageUpdate(GameTime time) { }

        /// <summary>
        /// Called after every system and stage is updated.
        /// </summary>
        protected virtual void AfterUpdate(GameTime time) { }

        protected override void Draw(GameTime gameTime)
        {
            PreDraw();

            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(transformMatrix: camera.GetViewMatrix());
            stage.Draw(spriteBatch);
            InSpriteBatchDraw(spriteBatch);
            spriteBatch.End();

            //base.Draw(gameTime);
        }

        protected virtual void InSpriteBatchDraw(SpriteBatch batch) { }

        protected virtual void PreDraw() { }

    }
}
