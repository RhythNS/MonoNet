using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoNet.Tiled
{
    /// <summary>
    /// Representation of a Tile in a Tiled map.
    /// </summary>
    public abstract class Tile
    {
        protected float x, y;

        protected Tile(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public abstract void Draw(SpriteBatch spriteBatch, Vector2 basePosition, Vector2 scale);
    }
}
