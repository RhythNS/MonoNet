﻿using Microsoft.Xna.Framework.Content.Pipeline;
using System.Text;

namespace MonoNetContentPipelineExtensions
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// TODO: change the ContentProcessor attribute to specify the correct
    /// display name for this processor.
    /// </summary>
    [ContentProcessor(DisplayName = "TiledProcessor")]
    public class TiledProcessor : ContentProcessor<TiledMapData, byte[]>
    {
        public override byte[] Process(TiledMapData input, ContentProcessorContext context)
        {
            return Encoding.ASCII.GetBytes(input.Data.Trim());
        }
    }
}