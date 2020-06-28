using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.IO;

namespace MonoNetContentPipelineExtensions.Tiled
{
    [ContentImporter(".tsx", DisplayName = "TsxImporter", DefaultProcessor = "TsxProcessor")]
    public class TileSetImporter : ContentImporter<TiledMapData>
    {
        public override TiledMapData Import(string filename, ContentImporterContext context)
        {
            try
            {
                context.Logger.LogMessage("Importing TSX file: {0}", filename);

                string read = File.ReadAllText(filename);

                return new TiledMapData(read);
            }
            catch (Exception ex)
            {
                context.Logger.LogMessage("Error {0}", ex);
                throw;
            }
        }
    }
}
