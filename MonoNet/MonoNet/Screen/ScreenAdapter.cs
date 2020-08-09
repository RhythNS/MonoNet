using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoNet.Screen
{
    /// <summary>
    /// Helper class for extending a screen without the need to implement every method of screen.
    /// </summary>
    public abstract class ScreenAdapter : IScreen
    {
        public void Initialize()
        {
        }

        public void LoadContent()
        {
        }

        public void UnloadContent()
        {
        }

        public abstract void Draw(SpriteBatch batch);

        public abstract void Update(GameTime gameTime);
    }
}
