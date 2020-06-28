using Microsoft.Xna.Framework.Content.Pipeline;

namespace MonoNetContentPipelineExtensions.Tiled
{
    [ContentProcessor(DisplayName = "TsxProcessor")]
    public class TileSetProcessor : ContentProcessor<TiledMapData, TiledMapData>
    {
        public override TiledMapData Process(TiledMapData input, ContentProcessorContext context)
        {
            return new TiledMapData(input.Data.Trim());
        }
    }
}
