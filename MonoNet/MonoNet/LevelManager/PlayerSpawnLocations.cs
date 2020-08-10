using Microsoft.Xna.Framework;
using MonoNet.Tiled;
using System;
using System.Collections.Generic;
using TiledSharp;

namespace MonoNet.LevelManager
{
    public class PlayerSpawnLocations
    {
        public List<Vector2> SpawnLocations { get; private set; } = new List<Vector2>();
        private int at;

        public void OnObjectLoaded(List<TiledMapComponent> allMapComponents, TmxObject loadedObject)
        {
            if (loadedObject.Type.Equals("playerSpawn", StringComparison.CurrentCultureIgnoreCase) == false)
                return;

            SpawnLocations.Add(new Vector2((float)loadedObject.X, (float)loadedObject.Y));
        }

        /// <summary>
        /// Should be called when a level change occured.
        /// </summary>
        public void ClearLocations() => SpawnLocations.Clear();

        /// <summary>
        /// Randomizes the position of where players should spawn.
        /// </summary>
        public void Randomize() => at = GameManager.Random.Next(SpawnLocations.Count);

        /// <summary>
        /// Get a random location of a player spawn.
        /// </summary>
        /// <returns>The location of a random player spawn.</returns>
        public Vector2 GetRandomLocation()
        {
            if (++at >= SpawnLocations.Count)
                at = 0;
            return SpawnLocations[at];
        }
    }
}
