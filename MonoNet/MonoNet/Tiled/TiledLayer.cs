using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoNet.ECS.Components;
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
        public TiledLayer(TiledBase tiledBase, TmxMap map, TiledMapComponent mapComponent, int layerIndex, bool enabled = true)
        {
            TmxLayer layer = map.TileLayers[layerIndex];
            Transform2 transform = mapComponent.Actor.GetComponent<Transform2>();

            this.enabled = enabled;
            tiles = new List<Tile>();

            int tileWidth = map.TileWidth;
            int tileHeight = map.TileHeight;
            int width = map.Width;

            // Go through all tiles
            for (int i = 0; i < layer.Tiles.Count; i++)
            {
                TmxLayerTile layerTile = layer.Tiles[i];
                int gid = layerTile.Gid;

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

                float x = layerTile.X * tileWidth;
                float y = layerTile.Y * tileHeight;

                if (toFindTileset.Tiles.TryGetValue(gid, out TmxTilesetTile tilesetTile) == true)
                {
                    // Get all hitboxes
                    for (int groupIndex = 0; groupIndex < tilesetTile.ObjectGroups.Count; groupIndex++)
                    {
                        for (int objectIndex = 0; objectIndex < tilesetTile.ObjectGroups[groupIndex].Objects.Count; objectIndex++)
                        {
                            tiledBase.NotifyHitboxLoaded(mapComponent, transform, tilesetTile.ObjectGroups[groupIndex].Objects[objectIndex], x, y);
                        }
                    }
                }
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
