using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.Util;
using System;
using TiledSharp;

namespace MonoNet.Tiled
{
    /// <summary>
    /// A representation of a map. Holds the reference of the map and its layers.
    /// </summary>
    public class TiledMapComponent : Component, Interfaces.IDrawable, IDisposable
    {
        public TmxMap Map { get; private set; }
        public TiledLayer[] Layers { get; private set; }

        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        private TiledBase tiledBase;
        private Transform2 transform;

        /// <summary>
        /// Transforms from tiled coordinates to world coordinates.
        /// </summary>
        /// <param name="tiledPos">The coordinates in the tiled system.</param>
        /// <returns>The transformed world coordinates.</returns>
        public Vector2 TiledToWorld(Vector2 tiledPos) => transform.WorldPosition + tiledPos * transform.WorldScale;

        /// <summary>
        /// Loads the map onto this component.
        /// </summary>
        /// <param name="map">The reference of the tiled map that should be loaded.</param>
        /// <param name="tiledBase">A reference to the base from where the load call was made.</param>
        public void Set(TmxMap map, TiledBase tiledBase)
        {
            Map = map;

            // If the actor does not have a transform then print an error.
            if (Actor.TryGetComponent(out transform) == false)
                Log.Error("There is no transform on TiledMap!");

            TileWidth = map.TileWidth;
            TileHeight = map.TileHeight;

            Width = map.Width;
            Height = map.Height;

            this.tiledBase = tiledBase;

            // Create all layers
            Layers = new TiledLayer[Map.TileLayers.Count];
            for (int i = 0; i < Layers.Length; i++)
                Layers[i] = new TiledLayer(tiledBase, Map, i);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 worldPosition = transform.WorldPosition;
            Vector2 scale = transform.WorldScale;

            for (int i = 0; i < Layers.Length; i++)
                Layers[i].Draw(spriteBatch, worldPosition, scale);
        }

        public void Dispose()
        {
            tiledBase.RemoveMap(this);
        }
    }
}
