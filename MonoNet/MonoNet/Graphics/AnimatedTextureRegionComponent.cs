using Microsoft.Xna.Framework.Graphics;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems;
using MonoNet.Interfaces;
using MonoNet.Util;

namespace MonoNet.Graphics
{
    class AnimatedTextureRegionComponent : Component, IUpdateable, IDrawable
    {
        public Animation<TextureRegion> animation;
        public float animationDuration;
        public Microsoft.Xna.Framework.Color color = Microsoft.Xna.Framework.Color.White;

        private Transform2 transform;

        protected override void OnInitialize()
        {
            Actor.TryGetComponent(out transform);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            TextureRegion region = animation.GetKeyframe(animationDuration);
            Microsoft.Xna.Framework.Rectangle rec = new Microsoft.Xna.Framework.Rectangle
                ((int)transform.WorldPosition.X, (int)transform.WorldPosition.Y, (int)(region.sourceRectangle.Width * transform.WorldScale.X), (int)(region.sourceRectangle.Height * transform.WorldScale.Y));
            region.Draw(spriteBatch, rec, color, 0, new Microsoft.Xna.Framework.Vector2(0, 0), SpriteEffects.None, 0);
        }

        public void Update()
        {
            animationDuration += Time.Delta;
        }
    }
}
