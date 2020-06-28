using Microsoft.Xna.Framework.Content.Pipeline;
using System.Text;

namespace MonoNetContentPipelineExtensions
{
    [ContentProcessor(DisplayName = "TiledProcessor")]
    public class TiledProcessor : ContentProcessor<TiledMapData, byte[]>
    {
        public override byte[] Process(TiledMapData input, ContentProcessorContext context)
        {
            string data = input.Data;

            string filePath = input.FileName;
            int contentLocation = filePath.IndexOf("/Content") + 1;
            int lastSlashLocation = filePath.LastIndexOf("/");
            filePath = filePath.Substring(contentLocation, lastSlashLocation - contentLocation + 1);

            data = data.Replace("source=\"", "source=\"" + filePath);

            return Encoding.ASCII.GetBytes(data.Trim());
        }
    }
}