using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoNet.GameSystems;
using MonoNet.Network.MasterServerConnection;
using MonoNet.Screen;
using MonoNet.Testing.Infrastructure;
using MonoNet.Testing.UI;
using MonoNet.Util;
using Myra;

namespace MonoNet
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MonoNet : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private GameSystemManager manager;

        private static IScreen currentScreen;

        public MonoNet()
        {
            // connect to the master server
            MasterServerConnector masterServerConnector = new MasterServerConnector();
            masterServerConnector.Start();

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // start in MainMenu
            currentScreen = new MainMenu(this);

            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            new Log(Log.Level.PrintMessages, Log.Level.PrintMessagesAndStackTrace, Log.Level.PrintMessagesAndStackTrace);

            manager = new GameSystemManager();
            manager.Add(new Time(), new Input());

            currentScreen.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            // needs to be set for Myra
            MyraEnvironment.Game = this;

            currentScreen.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here

            currentScreen.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            currentScreen.Update(gameTime);

            base.Update(gameTime);

            manager.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            currentScreen.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
