using Microsoft.Xna.Framework.Graphics;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems;
using MonoNet.Interfaces;

namespace MonoNet.Graphics
{
    public class AnimatedTextureRegionComponent : Component, IUpdateable, IDrawable
    {
        public Animation<TextureRegion>[] animations;
        public int selectedAnimation = 0;
        public float animationDuration;
        public Microsoft.Xna.Framework.Color color = Microsoft.Xna.Framework.Color.White;
        public bool mirrored;

        private Transform2 transform;

        protected override void OnInitialize()
        {
            Actor.TryGetComponent(out transform);
        }

        public void Set(params Animation<TextureRegion>[] animations)
        {
            this.animations = animations;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            TextureRegion region = animations[selectedAnimation].GetKeyframe(animationDuration);
            Microsoft.Xna.Framework.Rectangle rec = new Microsoft.Xna.Framework.Rectangle
                ((int)transform.WorldPosition.X, (int)transform.WorldPosition.Y, (int)(region.sourceRectangle.Width * transform.WorldScale.X), (int)(region.sourceRectangle.Height * transform.WorldScale.Y));
            region.Draw(spriteBatch, rec, color, 0, new Microsoft.Xna.Framework.Vector2(0, 0), mirrored == true ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public void Update()
        {
            animationDuration += Time.Delta;
        }
    }
}
