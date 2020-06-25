using Microsoft.Xna.Framework.Graphics;

namespace MonoNet.Interfaces
{
    /// <summary>
    /// Interface for drawing something onto the screen via a spritebatch.
    /// </summary>
    public interface IDrawable
    {
        void Draw(SpriteBatch spriteBatch);
    }
}
