using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoNet.Graphics;
using System.Collections.Generic;
using TiledSharp;

namespace MonoNet.Tiled
{
    /// <summary>
    /// Represents a single layer on a TiledMap.
    /// </summary>
    public class TiledLayer
    {
        /// <summary>
        /// Determines wheter the layer should be drawn or not.
        /// </summary>
        public bool enabled;
        private readonly List<Tile> tiles;

        /// <summary>
        /// Creates a new TiledLayer with given parameters.
        /// </summary>
        /// <param name="tiledBase">Instance of the tiledBase from where this map was created.</param>
        /// <param name="map">Instance of the map from where this layer is from.</param>
        /// <param name="layerIndex">The index of the layer that should be loaded from the map.</param>
        /// <param name="enabled">If it should be rendered after initialization.</param>
        public TiledLayer(TiledBase tiledBase, TmxMap map, int layerIndex, bool enabled = true)
        {
            TmxLayer layer = map.TileLayers[layerIndex];

            this.enabled = enabled;
            tiles = new List<Tile>();

            int tileWidth = map.TileWidth;
            int tileHeight = map.TileHeight;
            int width = map.Width;

            // Go through all tiles
            for (int i = 0; i < layer.Tiles.Count; i++)
            {
                TmxLayerTile tile = layer.Tiles[i];
                int gid = tile.Gid;

                // Empty tile, do nothing
                if (gid == 0)
                    continue;

                // Get the acctual gid value
                gid--;

                // Get the correct tileset with the gid value
                TmxTileset toFindTileset = null;
                for (int j = 0; j < map.Tilesets.Count; j++)
                {
                    gid -= map.Tilesets[j].TileCount ?? 0;
                    if (gid <= 0)
                    {
                        toFindTileset = map.Tilesets[j];
                        break;
                    }
                }

                // set gid to a positive value again
                gid += toFindTileset.TileCount ?? 0;

                Texture2D tilesetImage = tiledBase.GetTilesetImage(toFindTileset.Image.Source);

                int columns = toFindTileset.Columns.Value;

                int column = gid % columns;
                int row = gid / columns;

                float x = tile.X * tileWidth;
                float y = tile.Y * tileHeight;

                // TODO: Add AnimationTile
                tiles.Add(new StaticTile(x, y, new TextureRegion(tilesetImage, column * tileWidth, row * tileHeight, tileWidth, tileHeight)));
            }
        }

        /// <summary>
        /// Draws the layer.
        /// </summary>
        /// <param name="spriteBatch">The spriteBatch to draw the layer on.</param>
        /// <param name="basePosition">The origin position of the map.</param>
        /// <param name="scale">The scale of the map.</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 basePosition, Vector2 scale)
        {
            if (enabled == true)
                for (int i = 0; i < tiles.Count; i++)
                    tiles[i].Draw(spriteBatch, basePosition, scale);
        }
    }
}
