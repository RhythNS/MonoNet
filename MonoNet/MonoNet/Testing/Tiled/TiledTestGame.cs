using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.GameSystems;
using MonoNet.Graphics;
using MonoNet.Testing.Infrastructure;
using MonoNet.Util;
using MonoNet.Util.Pools;
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
            tileLocation = tileLocation.Substring(0, tileLocation.Length - 4);
            tileset = Content.Load<Texture2D>("Test/" + tileLocation);

            tileWidth = map.Tilesets[0].TileWidth;
            tileHeight = map.Tilesets[0].TileHeight;

            tilesetTilesXSize = tileset.Width / tileWidth;
            tilesetTilesYSize = tileset.Height / tileHeight;
        }

        protected override void InSpriteBatchDraw(SpriteBatch batch)
        {
            for (var i = 0; i < map.TileLayers.Count; i++)
            {
                int gid = map.TileLayers[i].Tiles[i].Gid;

                // Empty tile, do nothing
                if (gid == 0)
                    continue;

                int tileFrame = gid - 1;
                int column = tileFrame % tilesetTilesXSize;
                int row = (int)Math.Floor((double)tileFrame / (double)tilesetTilesXSize);

                float x = (i % map.Width) * map.TileWidth;
                float y = (float)Math.Floor(i / (double)map.Width) * map.TileHeight;

                Rectangle tilesetRec = new Rectangle(tileWidth * column, tileHeight * row, tileWidth, tileHeight);

                spriteBatch.Draw(tileset, new Rectangle((int)x, (int)y, tileWidth, tileHeight), tilesetRec, Color.White);

            }
        }
    }
}
