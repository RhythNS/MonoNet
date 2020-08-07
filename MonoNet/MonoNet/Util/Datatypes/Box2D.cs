using Microsoft.Xna.Framework;

namespace MonoNet.Util.Datatypes
{
    public struct Box2D
    {
        public float x, y, width, height;

        public Box2D(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public Box2D(Vector2 position, Vector2 size)
        {
            x = position.X;
            y = position.Y;
            width = size.X;
            height = size.Y;
        }

        public Box2D(Box2D other)
        {
            x = other.x;
            y = other.y;
            width = other.width;
            height = other.height;
        }

        public bool Intersecting(Box2D other) =>
            x < other.x + other.width && x + width > other.x && y < other.y + height && y + height > other.y;

    }
}
