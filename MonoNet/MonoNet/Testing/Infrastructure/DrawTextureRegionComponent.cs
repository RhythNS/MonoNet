using Microsoft.Xna.Framework.Graphics;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.Graphics;
using MonoNet.Interfaces;

namespace MonoNet.Testing.Infrastructure
{
    public class DrawTextureRegionComponent : Component, IDrawable
    {
        public TextureRegion region;
        private Transform2 transform;

        protected override void OnInitialize()
        {
            Actor.GetComponent(out transform);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Microsoft.Xna.Framework.Rectangle rec = new Microsoft.Xna.Framework.Rectangle
              ((int)transform.position.X, (int)transform.position.Y, (int)(region.sourceRectangle.Width * transform.scale.X), (int)(region.sourceRectangle.Height * transform.scale.Y));
            region.Draw(spriteBatch, rec, Microsoft.Xna.Framework.Color.White, 0, new Microsoft.Xna.Framework.Vector2(0, 0), SpriteEffects.None, 0);
        }
    }
}
