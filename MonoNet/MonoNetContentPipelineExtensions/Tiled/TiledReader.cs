using Microsoft.Xna.Framework.Content;
using TiledSharp;

namespace MonoNetContentPipelineExtensions.Tiled
{
    public class TiledReader : ContentTypeReader<TmxMap>
    {
        protected override TmxMap Read(ContentReader input, TmxMap existingInstance)
        {
            return input.ReadObject<TmxMap>();
        }
    }
}
