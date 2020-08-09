using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoNet.Screen
{
    /// <summary>
    /// Represents a gamestate. Used for the ScreenManager.
    /// </summary>
    public interface IScreen
    {
        void Initialize();
        void LoadContent();
        void UnloadContent();
        void Update(GameTime gameTime);
        void Draw(SpriteBatch batch);
    }
}
