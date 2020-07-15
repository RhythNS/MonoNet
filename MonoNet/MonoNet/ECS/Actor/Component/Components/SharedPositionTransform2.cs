using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MonoNet.ECS.Components
{
    public class SharedPositionTransform2 : Transform2
    {
        private SharedVariables sharedVariables;

        private class SharedVariables
        {
            public Vector2 position, scale;

            public SharedVariables(Vector2 position, Vector2 scale)
            {
                this.position = position;
                this.scale = scale;
            }
        }

        protected override void OnInitialize() { }

        public override Vector2 LocalPosition { get => sharedVariables.position; set => sharedVariables.position = value; }

        public override Vector2 LocalScale { get => sharedVariables.scale; set => sharedVariables.scale = value; }

        public void SetGroup(params SharedPositionTransform2[] transforms)
        {
            sharedVariables = new SharedVariables(new Vector2(), new Vector2(1, 1));
            for (int i = 0; i < transforms.Length; i++)
                transforms[i].sharedVariables = sharedVariables;
        }

        public void SetGroup(List<SharedPositionTransform2> transforms)
        {
            sharedVariables = new SharedVariables(new Vector2(), new Vector2(1, 1));
            for (int i = 0; i < transforms.Count; i++)
                transforms[i].sharedVariables = sharedVariables;
        }

    }
}
