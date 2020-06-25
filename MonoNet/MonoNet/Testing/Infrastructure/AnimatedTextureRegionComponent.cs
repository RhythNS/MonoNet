﻿using Microsoft.Xna.Framework.Graphics;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems;
using MonoNet.Graphics;
using MonoNet.Interfaces;
using MonoNet.Util;

namespace MonoNet.Testing.Infrastructure
{
    class AnimatedTextureRegionComponent : Component, IUpdateable, IDrawable
    {
        public Animation<TextureRegion> animation;
        public float animationDuration;
        public Microsoft.Xna.Framework.Color color = Microsoft.Xna.Framework.Color.White;

        private Transform2 transform;

        protected override void OnInitialize()
        {
            Actor.GetComponent(out transform);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            TextureRegion region = animation.GetKeyframe(animationDuration);
            Microsoft.Xna.Framework.Rectangle rec = new Microsoft.Xna.Framework.Rectangle
                ((int)transform.position.X, (int)transform.position.Y, (int)(region.sourceRectangle.Width * transform.scale.X), (int)(region.sourceRectangle.Height * transform.scale.Y));
            region.Draw(spriteBatch, rec, color, 0, new Microsoft.Xna.Framework.Vector2(0, 0), SpriteEffects.None, 0);
        }

        public void Update()
        {
            animationDuration += Time.Delta;
        }
    }
}
