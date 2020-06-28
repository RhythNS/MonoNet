using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TiledSharp;

namespace MonoNet.Testing.Tiled
{
    class TiledTestGame : TestGame
    {
        private TmxMap map;
        private Texture2D tileset;

        private int tileWidth;
        private int tileHeight;
        private int tilesetTilesXSize;
        private int tilesetTilesYSize;

        protected override void LoadContent()
        {
            base.LoadContent();

            map = Content.Load<TmxMap>("Test/DatMapWithObjectLayer");
            string tileLocation = map.Tilesets[0].Image.Source;
            tileLocation = tileLocation.Substring(8, tileLocation.Length - 8 - 4);
            tileset = Content.Load<Texture2D>(tileLocation);

            tileWidth = map.Tilesets[0].TileWidth;
            tileHeight = map.Tilesets[0].TileHeight;

            tilesetTilesXSize = tileset.Width / tileWidth;
            tilesetTilesYSize = tileset.Height / tileHeight;
        }

        protected override void InSpriteBatchDraw(SpriteBatch batch)
        {
            for (int indexLayer = 0; indexLayer < map.TileLayers.Count; indexLayer++)
            {
                for (int indexTile = 0; indexTile < map.TileLayers[indexLayer].Tiles.Count; indexTile++)
                {
                    int gid = map.TileLayers[indexLayer].Tiles[indexTile].Gid;

                    // Empty tile, do nothing
                    if (gid == 0)
                        continue;

                    int tileFrame = gid - 1;
                    int column = tileFrame % tilesetTilesXSize;
                    int row = (int)Math.Floor((double)tileFrame / (double)tilesetTilesXSize);

                    float x = (indexTile % map.Width) * map.TileWidth;
                    float y = (float)Math.Floor(indexTile / (double)map.Width) * map.TileHeight;

                    Rectangle positionInSheet = new Rectangle(tileWidth * column, tileHeight * row, tileWidth, tileHeight);

                    spriteBatch.Draw(tileset, new Rectangle((int)x, (int)y, tileWidth, tileHeight), positionInSheet, Color.White);
                }
            }
        }
    }
}
