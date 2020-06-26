using Microsoft.Xna.Framework.Content.Pipeline;
using System;
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
    [ContentImporter(".tmx", DisplayName = "Tmx Importer", DefaultProcessor = "Tmx Processor")]
    public class TiledImporter : ContentImporter<TmxMap>
    {
        public override TmxMap Import(string filename, ContentImporterContext context)
        {
            try
            {
                context.Logger.LogMessage("Importing TMX file: {0}", filename);

                return new TmxMap(filename);
            }
            catch (Exception ex)
            {
                context.Logger.LogMessage("Error {0}", ex);
                throw;
            }
        }

    }

}
    