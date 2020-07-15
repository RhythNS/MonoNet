using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledSharp;

namespace MonoNet.Tiled
{
    public class TiledSingleLayerComponent : TiledMapComponent
    {
        public TiledLayer Layer { get; protected set; }

        /// <summary>
        /// Loads the specififed Layer onto this component.
        /// </summary>
        /// <param name="map">The reference of the tiled map that should be loaded.</param>
        /// <param name="tiledBase">A reference to the base from where the load call was made.</param>
        /// <param name="layer">Which layer to load.</param>
        public void Set(TmxMap map, TiledBase tiledBase, int layer)
        {
            Set(map, tiledBase);

            Layer = new TiledLayer(tiledBase, map, this, layer);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 worldPosition = transform.WorldPosition;
            Vector2 scale = transform.WorldScale;

            Layer.Draw(spriteBatch, worldPosition, scale);
        }
    }
}
