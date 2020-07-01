using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.IO;

namespace MonoNetContentPipelineExtensions
{
    [ContentImporter(".tmx", DisplayName = "TiledImporter", DefaultProcessor = "TiledProcessor")]
    public class TiledImporter : ContentImporter<TiledMapData>
    {
        public override TiledMapData Import(string filename, ContentImporterContext context)
        {
            try
            {
                context.Logger.LogMessage("Importing TMX file: {0}", filename);

                string read = File.ReadAllText(filename);

                return new TiledMapData(read, filename);
            }
            catch (Exception ex)
            {
                context.Logger.LogMessage("Error {0}", ex);
                throw;
            }
        }

    }

}
