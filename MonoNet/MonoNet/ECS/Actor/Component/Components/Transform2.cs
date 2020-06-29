using Microsoft.Xna.Framework;
using MonoNet.Util;
using System.Collections.Generic;

namespace MonoNet.ECS.Components
{
    /// <summary>
    /// Holds position and scale of an Actor.
    /// </summary>
    public class Transform2 : Component
    {
        private List<Transform2> children = new List<Transform2>();
        private Transform2 parent;
        public Transform2 Parent
        {
            get => parent;
            set
            {
                if (parent == value)
                    return;

                // If the old parent is not null then remove the reference to this transfrom
                if (parent != null)
                    parent.children.Remove(this);

                // If the new parent is not null then add it a reference to it
                if (value != null)
                    value.children.Add(this);

                // lastly assign the parent to the new value
                parent = value;
            }
        }

        /// <summary>
        /// Scale relative to this parents scale or acctual scale if this transform has no parents.
        /// </summary>
        public Vector2 LocalScale { get; set; }

        /// <summary>
        /// The acctual scale this transform has.
        /// </summary>
        public Vector2 WorldScale
        {
            get => parent == null ? LocalScale : LocalScale * parent.WorldScale;
            set
            {
                if (value == Vector2.Zero)
                {
                    Log.Warn("Scale of 0 is not allowed!");
                    return;
                }

                Vector2 scale = parent == null ? new Vector2() : parent.WorldScale;
                LocalScale = value / scale;
            }
        }

        /// <summary>
        /// The position relative to this parents location or acctual position if this transform has no parent.
        /// </summary>
        public Vector2 LocalPosition { get; set; }

        /// <summary>
        /// The acctual position this transform has.
        /// </summary>
        public Vector2 WorldPosition
        {
            get => parent == null ? LocalPosition : LocalPosition + parent.WorldPosition;
            set
            {
                Vector2 position = parent == null ? new Vector2() : parent.WorldPosition;
                LocalPosition = value - position;
            }
        }

        protected override void OnInitialize()
        {
            LocalPosition = new Vector2();
            LocalScale = new Vector2(1, 1);
        }

        /// <summary>
        /// Gets the count of all children attached to this transform.
        /// </summary>
        public int GetChildrenCount => children.Count;

        /// <summary>
        /// Gets the child at specified index.
        /// </summary>
        /// <param name="index">The index of the children array.</param>
        /// <returns>The requested child at index.</returns>
        public Transform2 GetChildAt(int index) => children[index];
    }
}
