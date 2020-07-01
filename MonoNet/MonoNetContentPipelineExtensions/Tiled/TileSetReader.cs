using Microsoft.Xna.Framework.Content;
using System.Xml.Linq;
using TiledSharp;

namespace MonoNetContentPipelineExtensions.Tiled
{
    public class TileSetReader : ContentTypeReader<TmxTileset>
    {
        protected override TmxTileset Read(ContentReader input, TmxTileset existingInstance)
        {
            int length = input.ReadInt32();
            char[] charArr = input.ReadChars(length);
            string s = new string(charArr);
            XDocument document = XDocument.Parse(s);
            return new TmxTileset(document.Element("tileset"));
        }
    }
}
