using Microsoft.Xna.Framework;

namespace MonoNet.Screen
{
    public interface IScreen
    {
        void Initialize();
        void LoadContent();
        void UnloadContent();
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
    }
}
