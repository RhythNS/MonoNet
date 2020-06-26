using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using TiledSharp;

namespace MonoNetContentPipelineExtensions.Tiled
{
    [ContentTypeWriter]
    public class TiledWriter : ContentTypeWriter<TmxMap>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "MonoNetContentPipelineExtensions.Tiled.TiledReader";
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(TmxMap).AssemblyQualifiedName;
        }

        protected override void Write(ContentWriter output, TmxMap value)
        {
            output.WriteObject(value);
        }
    }
}
