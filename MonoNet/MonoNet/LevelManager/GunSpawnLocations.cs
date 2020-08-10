using Microsoft.Xna.Framework;
using MonoNet.Tiled;
using System;
using System.Collections.Generic;
using TiledSharp;

namespace MonoNet.LevelManager
{
    /// <summary>
    /// Holds references to locations of where guns spawn in a level.
    /// </summary>
    class GunSpawnLocations
    {
        public static GunSpawnLocations Instance { get; private set; }

        public GunSpawnLocations()
        {
            Instance = this;
        }

        public List<Vector2> GunLocations { get; private set; } = new List<Vector2>();

        public void OnObjectLoaded(List<TiledMapComponent> allMapComponents, TmxObject loadedObject)
        {
            if (loadedObject.Type.Equals("gunSpawn", StringComparison.CurrentCultureIgnoreCase) == false)
                return;

            GunLocations.Add(new Vector2((float)loadedObject.X, (float)loadedObject.Y));
        }

        /// <summary>
        /// Should be called when a level change occured.
        /// </summary>
        public void ClearLocations() => GunLocations.Clear();

        /// <summary>
        /// Get a random location of a gun spawn.
        /// </summary>
        /// <returns>The location of a random gun spawn.</returns>
        public Vector2 GetRandomLocation() => GunLocations[GameManager.Random.Next(GunLocations.Count)];
    }
}
