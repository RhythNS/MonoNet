using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems;

namespace MonoNet.Testing.Tiled
{
    public class TiledTestMoverAndScaler : Component, Interfaces.IUpdateable
    {
        private Transform2 transform;

        protected override void OnInitialize()
        {
            Actor.GetComponent(out transform);
        }

        public void Update()
        {
            Vector2 position = transform.LocalPosition;
            if (Input.KeyDown(Keys.W))
                position.Y += 10 * Time.Delta;
            if (Input.KeyDown(Keys.A))
                position.X -= 10 * Time.Delta;
            if (Input.KeyDown(Keys.S))
                position.Y -= 10 * Time.Delta;
            if (Input.KeyDown(Keys.D))
                position.X += 10 * Time.Delta;

            Vector2 scale = transform.LocalScale;
            if (Input.KeyDown(Keys.T))
                scale.Y += 2 * Time.Delta;
            if (Input.KeyDown(Keys.F))
                scale.X -= 2 * Time.Delta;
            if (Input.KeyDown(Keys.G))
                scale.Y -= 2 * Time.Delta;
            if (Input.KeyDown(Keys.H))
                scale.X += 2 * Time.Delta;

            transform.LocalPosition = position;
            transform.LocalScale = scale;
        }
    }
}
