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

        public void OnObjectLoaded(List<TiledMapComponent> allMapComponents, TmxObject loadedObject)
        {
            if (loadedObject.Type.Equals("playerSpawn", StringComparison.CurrentCultureIgnoreCase) == false)
                return;

            SpawnLocations.Add(new Vector2((float)loadedObject.X, (float)loadedObject.Y));
        }

        public void ClearLocations() => SpawnLocations.Clear();

        public Vector2 GetRandomLocation() => SpawnLocations[GameManager.Random.Next(SpawnLocations.Count)];
    }
}
