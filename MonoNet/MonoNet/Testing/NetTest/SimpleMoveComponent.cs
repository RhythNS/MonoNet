using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems;

namespace MonoNet.Testing.NetTest
{
    public class SimpleMoveComponent : Component, Interfaces.IUpdateable
    {
        private Transform2 transform;

        protected override void OnInitialize()
        {
            transform = Actor.GetComponent<Transform2>();
        }

        public void Update()
        {
            Vector2 vector = new Vector2();

            if (Input.KeyDown(Keys.T))
                vector.Y -= 60;
            if (Input.KeyDown(Keys.G))
                vector.Y += 60;
            if (Input.KeyDown(Keys.F))
                vector.X -= 60;
            if (Input.KeyDown(Keys.H))
                vector.X += 60;

            transform.WorldPosition += vector * Time.Delta;
        }
    }
}
