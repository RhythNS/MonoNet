using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using TiledSharp;

namespace MonoNetContentPipelineExtensions.Tiled
{
    [ContentTypeWriter]
    public class TiledWriter : ContentTypeWriter<byte[]>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(TiledReader).AssemblyQualifiedName;
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(TmxMap).AssemblyQualifiedName;
        }

        protected override void Write(ContentWriter output, byte[] value)
        {
            output.Write(value.Length);
            output.Write(value);
        }
    }
}
