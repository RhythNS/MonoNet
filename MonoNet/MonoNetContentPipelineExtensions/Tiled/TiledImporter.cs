using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.IO;
using TiledSharp;

namespace MonoNetContentPipelineExtensions
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to import a file from disk into the specified type, TImport.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// TODO: change the ContentImporter attribute to specify the correct file
    /// extension, display name, and default processor for this importer.
    /// </summary>
    [ContentImporter(".tmx", DisplayName = "TiledImporter", DefaultProcessor = "TiledProcessor")]
    public class TiledImporter : ContentImporter<TiledMapData>
    {
        public override TiledMapData Import(string filename, ContentImporterContext context)
        {
            try
            {
                context.Logger.LogMessage("Importing TMX file: {0}", filename);

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
