using Microsoft.Xna.Framework;

namespace MonoNet.GameSystems
{
    /// <summary>
    /// Gamesystem which needs to be updated every frame.
    /// </summary>
    public abstract class GameSystem
    {
        public abstract void Update(GameTime gameTime);
    }
}