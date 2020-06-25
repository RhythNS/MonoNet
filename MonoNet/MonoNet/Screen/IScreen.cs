using Microsoft.Xna.Framework;

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
        void Draw(GameTime gameTime);
    }
}
