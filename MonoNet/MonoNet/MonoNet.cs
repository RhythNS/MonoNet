using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoNet.GameSystems;
using MonoNet.GameSystems.PhysicsSystem;
using MonoNet.Network.Commands;
using MonoNet.Network.MasterServerConnection;
using MonoNet.Screen;
using MonoNet.Testing.UI;
using MonoNet.Util;
using Myra;
using System.Threading;

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
        public ScreenManager ScreenManager { get; private set; }

        MasterServerConnector masterServerConnector = new MasterServerConnector();

        private void ConnectToMasterServer() {
            masterServerConnector.Start();
        }

        public MonoNet() {
            //MasterServerConnector masterServerConnector = new MasterServerConnector();
            //masterServerConnector.Start();
            Thread msConnect = new Thread(ConnectToMasterServer);
            msConnect.Start();

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
            // start in MainMenu
            ScreenManager = new ScreenManager(new MainMenu(this)); 
            // ScreenManager = new ScreenManager(new DebugGameStartScreen(this)); // TODO: Replace this with mainmenu

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
            base.Initialize();

            new EventHandlerDictionary();
            new Log(Log.Level.PrintMessages, Log.Level.PrintMessagesAndStackTrace, Log.Level.PrintMessagesAndStackTrace);

            manager = new GameSystemManager();
            manager.Add(new Time(), new Input(), new Physic());

            ScreenManager.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 640;
            graphics.ApplyChanges();

            // needs to be set for Myra
            MyraEnvironment.Game = this;

            ScreenManager.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            ScreenManager.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();

            ScreenManager.Update(gameTime);

            base.Update(gameTime);

            manager.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            graphics.GraphicsDevice.Clear(Color.Black);

            ScreenManager.Draw(spriteBatch);
        }
    }
}
