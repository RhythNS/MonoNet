using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MonoNet.GameSystems
{
    /// <summary>
    /// Manages and updates GameSystems.
    /// </summary>
    public class GameSystemManager
    {
        private static GameSystemManager instance;

        private List<GameSystem> gameSystems;

        public GameSystemManager()
        {
            instance = this;
            gameSystems = new List<GameSystem>(2);
        }

        /// <summary>
        /// Adds a GameSystem to the manager.
        /// </summary>
        /// <param name="systems">The GameSystem to be added.</param>
        public void Add(params GameSystem[] systems)
        {
            gameSystems.AddRange(systems);
        }

        /// <summary>
        /// Tries to get the GameSystem with the given type.
        /// </summary>
        /// <typeparam name="T">The type of GameSystem to be found.</typeparam>
        /// <param name="gameSystem">The GameSystem that was requested.</param>
        /// <returns>If it found the GameSystem.</returns>
        public bool Get<T>(out T gameSystem) where T : GameSystem
        {
            for (int i = 0; i < gameSystems.Count; i++)
            {
                if (gameSystems[i].GetType() == typeof(T))
                {
                    gameSystem = (T)gameSystems[i];
                    return true;
                }
            }
            gameSystem = default;
            return false;
        }

        /// <summary>
        /// Updates each System.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < gameSystems.Count; i++)
                gameSystems[i].Update(gameTime);
        }
    }
}
