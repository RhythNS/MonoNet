using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using TiledSharp;

namespace MonoNetContentPipelineExtensions.Tiled
{
    [ContentTypeWriter]
    public class TileSetWriter : ContentTypeWriter<TiledMapData>
    {
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(TmxTileset).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(TileSetReader).AssemblyQualifiedName;
        }

        protected override void Write(ContentWriter output, TiledMapData value)
        {
            char[] valArr = value.Data.ToCharArray();
            output.Write(valArr);
        }
    }
}
