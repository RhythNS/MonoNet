using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoNet.Graphics
{
    public class TextureRegion
    {
        public Texture2D texture;
        public Rectangle sourceRectangle;

        public TextureRegion(Texture2D texture, int x, int y, int width, int height)
        {
            this.texture = texture;
            sourceRectangle = new Rectangle(x, y, width, height);
        }

        public TextureRegion(Texture2D texture, Rectangle sourceRectangle)
        {
            this.texture = texture;
            this.sourceRectangle = sourceRectangle;
        }

        public void Draw(SpriteBatch batch, Rectangle destination, Color color, Single rotation, Vector2 origin, SpriteEffects effects, Single layerDepth)
        {
            batch.Draw(texture, destination, sourceRectangle, color, rotation, origin, effects, layerDepth);
        }
    }
}
