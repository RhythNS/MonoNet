using Microsoft.Xna.Framework;

namespace MonoNet.ECS.Components
{
    /// <summary>
    /// Holds position and scale of an Actor.
    /// </summary>
    public class Transform2 : Component
    {
        public Vector2 position;
        public Vector2 scale;

        protected override void OnInitialize()
        {
            position = new Vector2();
            scale = new Vector2(1, 1);
        }
    }
}
