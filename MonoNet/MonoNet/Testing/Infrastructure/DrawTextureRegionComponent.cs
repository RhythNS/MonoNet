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
            Actor.TryGetComponent(out transform);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Microsoft.Xna.Framework.Rectangle rec = new Microsoft.Xna.Framework.Rectangle
              ((int)transform.WorldPosition.X, (int)transform.WorldPosition.Y, (int)(region.sourceRectangle.Width * transform.WorldScale.X), (int)(region.sourceRectangle.Height * transform.WorldScale.Y));
            region.Draw(spriteBatch, rec, Microsoft.Xna.Framework.Color.White, 0, new Microsoft.Xna.Framework.Vector2(0, 0), SpriteEffects.None, 0);
        }
    }
}
