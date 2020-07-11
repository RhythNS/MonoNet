using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledSharp;

namespace MonoNet.Tiled
{
    public class TiledMultiLayerComponent : TiledMapComponent
    {
        public TiledLayer[] Layers { get; protected set; }

        /// <summary>
        /// Loads the specified mapLayers onto this component.
        /// </summary>
        /// <param name="map">The reference of the tiled map that should be loaded.</param>
        /// <param name="tiledBase">A reference to the base from where the load call was made.</param>
        /// <param name="fromLayer">Layer to start loading from.</param>
        /// <param name="amountToLoad">How many layers to load.</param>
        public void Set(TmxMap map, TiledBase tiledBase, int fromLayer, int amountToLoad)
        {
            Set(map, tiledBase);

            // Create all layers
            Layers = new TiledLayer[amountToLoad];
            int index = -1;
            for (int i = fromLayer; i < fromLayer + amountToLoad; i++)
                Layers[++index] = new TiledLayer(tiledBase, Map, this, i);
        }

        /// <summary>
        /// Loads all mapLayers onto this component.
        /// </summary>
        /// <param name="map">The reference of the tiled map that should be loaded.</param>
        /// <param name="tiledBase">A reference to the base from where the load call was made.</param>
        public new void Set(TmxMap map, TiledBase tiledBase)
        {
            base.Set(map, tiledBase);

            Layers = new TiledLayer[map.TileLayers.Count];
            for (int i = 0; i < Layers.Length; i++)
                Layers[i] = new TiledLayer(tiledBase, Map, this, i);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 worldPosition = transform.WorldPosition;
            Vector2 scale = transform.WorldScale;

            for (int i = 0; i < Layers.Length; i++)
                Layers[i].Draw(spriteBatch, worldPosition, scale);
        }
    }
}
