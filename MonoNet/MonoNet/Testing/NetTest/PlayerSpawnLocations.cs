using Microsoft.Xna.Framework;
using MonoNet.Tiled;
using System;
using System.Collections.Generic;
using TiledSharp;

namespace MonoNet.Testing.NetTest
{
    public class PlayerSpawnLocations
    {
        private Random random;
        public static PlayerSpawnLocations Instance { get; private set; }

        public List<Vector2> SpawnLocations { get; private set; } = new List<Vector2>();

        public PlayerSpawnLocations()
        {
            Instance = this;
            random = new Random();
        }

        public void OnObjectLoaded(List<TiledMapComponent> allMapComponents, TmxObject loadedObject)
        {
            if (loadedObject.Type.Equals("playerSpawn", StringComparison.CurrentCultureIgnoreCase) == false)
                return;

            SpawnLocations.Add(new Vector2((float)loadedObject.X, (float)loadedObject.Y));
        }

        public Vector2 GetRandomLocation() => SpawnLocations[random.Next(SpawnLocations.Count)];
    }
}
