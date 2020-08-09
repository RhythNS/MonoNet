using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoNet.Screen
{
    /// <summary>
    /// Manages gamestates via IScreens.
    /// </summary>
    public class ScreenManager
    {
        private IScreen currentScreen;

        /// <summary>
        /// Creates a new ScreenManager with an currently selected screen.
        /// </summary>
        /// <param name="currentScreen"></param>
        public ScreenManager(IScreen currentScreen)
        {
            this.currentScreen = currentScreen;
        }

        /// <summary>
        /// Sets a new screen after unloading content from the previous one.
        /// </summary>
        /// <param name="screen">The screen to change to.</param>
        public void SetScreen(IScreen screen)
        {
            // Prepare old screen for deletion
            currentScreen.UnloadContent();

            // set new screen
            screen.Initialize();
            screen.LoadContent();
            currentScreen = screen;
        }

        public void Draw(SpriteBatch batch)
        {
            currentScreen.Draw(batch);
        }

        public void Initialize()
        {
            currentScreen.Initialize();
        }

        public void LoadContent()
        {
            currentScreen.LoadContent();
        }

        public void UnloadContent()
        {
            currentScreen.UnloadContent();
        }

        public void Update(GameTime gameTime)
        {
            currentScreen.Update(gameTime);
        }
    }
}
