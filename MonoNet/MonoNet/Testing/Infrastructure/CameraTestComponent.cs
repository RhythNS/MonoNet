using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoNet.ECS;
using MonoNet.GameSystems;
using MonoNet.Graphics;

namespace MonoNet.Testing.Infrastructure
{
    class CameraTestComponent : Component, Interfaces.IUpdateable
    {
        public Camera camera;

        public void Update()
        {
            float deltaTime = Time.Delta;

            if (Input.KeyDown(Keys.I))
                camera.Rotation -= deltaTime;

            if (Input.KeyDown(Keys.O))
                camera.Rotation += deltaTime;

            if (Input.KeyDown(Keys.K))
                camera.Zoom -= deltaTime;

            if (Input.KeyDown(Keys.L))
                camera.Zoom += deltaTime;

            if (Input.KeyDown(Keys.Up))
                camera.Position -= new Vector2(0, 250) * deltaTime;

            if (Input.KeyDown(Keys.Down))
                camera.Position += new Vector2(0, 250) * deltaTime;

            if (Input.KeyDown(Keys.Left))
                camera.Position -= new Vector2(250, 0) * deltaTime;

            if (Input.KeyDown(Keys.Right))
                camera.Position += new Vector2(250, 0) * deltaTime;
        }
    }
}
