using Microsoft.Xna.Framework.Content;
using System.Text;
using System.Xml.Linq;
using TiledSharp;

namespace MonoNetContentPipelineExtensions.Tiled
{
    public class TiledReader : ContentTypeReader<TmxMap>
    {
        protected override TmxMap Read(ContentReader input, TmxMap existingInstance)
        {
            int length = input.ReadInt32();
            byte[] data = input.ReadBytes(length);
            XDocument document = XDocument.Parse(Encoding.ASCII.GetString(data));
            return new TmxMap(document);
        }
    }
}
