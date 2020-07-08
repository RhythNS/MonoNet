using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.Util;
using System;
using System.Collections.Generic;
using TiledSharp;

namespace MonoNet.Tiled
{
    /// <summary>
    /// A representation of a map. Holds the reference of the map and its layers.
    /// </summary>
    public abstract class TiledMapComponent : Component, Interfaces.IDrawable, IDisposable
    {
        public TmxMap Map { get; protected set; }

        public int TileWidth { get; protected set; }
        public int TileHeight { get; protected set; }

        public int Width { get; protected set; }
        public int Height { get; protected set; }

        public TiledBase TiledBase { get; protected set; }
        public List<TiledMapComponent> ConnectedMaps { get; protected set; }

        protected Transform2 transform;

        /// <summary>
        /// Transforms from tiled coordinates to world coordinates.
        /// </summary>
        /// <param name="tiledPos">The coordinates in the tiled system.</param>
        /// <returns>The transformed world coordinates.</returns>
        public Vector2 TiledToWorld(Vector2 tiledPos) => transform.WorldPosition + tiledPos * transform.WorldScale;

        public void SetConnectedMaps(List<TiledMapComponent> connectedMaps)
        {
            ConnectedMaps = connectedMaps;
        }

        /// <summary>
        /// Sets all internal variables.
        /// </summary>
        /// <param name="map">The reference of the tiled map that should be loaded.</param>
        /// <param name="tiledBase">A reference to the base from where the load call was made.</param>
        protected void Set(TmxMap map, TiledBase tiledBase)
        {
            Map = map;

            // If the actor does not have a transform then print an error.
            if (Actor.TryGetComponent(out transform) == false)
                Log.Error("There is no transform on TiledMap!");

            TileWidth = map.TileWidth;
            TileHeight = map.TileHeight;

            Width = map.Width;
            Height = map.Height;

            TiledBase = tiledBase;
        }

        public void Dispose()
        {
            ConnectedMaps.Remove(this);
            if (ConnectedMaps.Count == 0)
                TiledBase.RemoveMap(Map);
        }

        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
