using Microsoft.Xna.Framework;
using MonoNet.Tiled;
using System;
using System.Collections.Generic;
using TiledSharp;

namespace MonoNet.LevelManager
{
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

        public void ClearLocations() => GunLocations.Clear();

        public Vector2 GetRandomLocation() => GunLocations[GameManager.Random.Next(GunLocations.Count)];
    }
}
