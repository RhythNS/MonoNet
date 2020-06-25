using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoNet.Util;

namespace MonoNet.Graphics
{
    /// <summary>
    /// A region inside a texture. Usually used when dealing with a spritesheet.
    /// </summary>
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

        public void Draw(SpriteBatch batch, Rectangle destination, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
        {
            batch.Draw(texture, destination, sourceRectangle, color, rotation, origin, effects, layerDepth);
        }

        /// <summary>
        /// Creates multiple Texture regions from paramteres. Usefull for when a texture is a spritesheet.
        /// </summary>
        /// <param name="regions">Out parameter of the final TextureRegions.</param>
        /// <param name="texture">The texture of the spritesheet.</param>
        /// <param name="singleWidth">The single width of a sprite on the spritesheet.</param>
        /// <param name="singleHeight">The single height of a sprite on the spritesheet.</param>
        /// <param name="points">X and Y index of desired sprite. These are multiplied with the single width to get the TextureRegion.</param>
        /// <returns>Wheter it successeded or an error occured.</returns>
        public static bool CreateFromSheet(out TextureRegion[] regions, Texture2D texture, int singleWidth, int singleHeight, params Point[] points)
        {
            regions = new TextureRegion[points.Length];
            int xSize = texture.Width / singleWidth;
            int ySize = texture.Height / singleHeight;
            for (int i = 0; i < regions.Length; i++)
            {
                if (points[i].X < 0 || points[i].X >= xSize || points[i].Y < 0 || points[i].Y >= ySize)
                {
                    Log.Error("Point was out of bounds: X(" + points[i].X + "/" + xSize + ", Y(" + points[i].Y + "/" + ySize + ")");
                    regions = default;
                    return false;
                }
                regions[i] = new TextureRegion(texture, points[i].X * singleWidth, points[i].Y * singleHeight, singleWidth, singleHeight);
            }
            return true;
        }

        /// <summary>
        /// Creates all Texture regions from a Texture. Usefull for when a texture is a spritesheet.
        /// </summary>
        /// <param name="texture">The texture of the spritesheet.</param>
        /// <param name="singleWidth">The single width of a sprite on the spritesheet.</param>
        /// <param name="singleHeight">The single height of a sprite on the spritesheet.</param>
        /// <returns>All sprites from the Texture.</returns>
        public static TextureRegion[] CreateAllFromSheet(Texture2D texture, int singleWidth, int singleHeight)
        {
            int xSize = texture.Width / singleWidth;
            int ySize = texture.Height / singleHeight;
            TextureRegion[] regions = new TextureRegion[xSize * ySize];
            int counter = -1;

            for (int y = 0; y < ySize; y++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    regions[++counter] = new TextureRegion(texture, x * singleWidth, y * singleHeight, singleWidth, singleHeight);
                }
            }

            return regions;
        }
    }
}
