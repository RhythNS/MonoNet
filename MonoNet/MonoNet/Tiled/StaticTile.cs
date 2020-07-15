using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoNet.Graphics;

namespace MonoNet.Tiled
{
    /// <summary>
    /// Representation of a single image tile.
    /// </summary>
    public class StaticTile : Tile
    {
        private TextureRegion textureRegion;

        public StaticTile(float x, float y, TextureRegion textureRegion) : base(x, y)
        {
            this.textureRegion = textureRegion;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 basePosition, Vector2 scale)
        {
            Rectangle position = new Rectangle((int)(basePosition.X + x * scale.X), (int)(basePosition.Y + y * scale.Y),
                (int)(textureRegion.sourceRectangle.Width * scale.X), (int)(textureRegion.sourceRectangle.Height * scale.Y));
            textureRegion.Draw(spriteBatch, position, Color.White, 0, new Vector2(), SpriteEffects.None, 0);
        }
    }
}
