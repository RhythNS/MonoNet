using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems;
using MonoNet.Graphics;
using MonoNet.Testing.Infrastructure;
using MonoNet.Util;
using MonoNet.Util.Pools;

namespace MonoNet.Testing
{
    public abstract class TestGame : Game
    {
        protected GraphicsDeviceManager graphics;
        protected SpriteBatch spriteBatch;
        protected GameSystemManager manager;
        protected Stage stage;

        public TestGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            new Log(Log.Level.PrintMessages, Log.Level.PrintMessagesAndStackTrace, Log.Level.PrintMessagesAndStackTrace);

            manager = new GameSystemManager();
            manager.Add(new Time(), new Input());

            stage = new Stage(5, new Pool<Actor>(50, 10));
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            PreUdate(gameTime);

            manager.Update(gameTime);
            if (Input.IsKeyDownThisFrame(Keys.Escape))
                Exit();
            stage.Update();

            AfterUpdate(gameTime);
        }

        protected virtual void PreUdate(GameTime time) { }

        protected virtual void AfterUpdate(GameTime time) { }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            stage.Draw(spriteBatch);
            InSpriteBatchDraw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected virtual void InSpriteBatchDraw(SpriteBatch batch) { }

    }
}
