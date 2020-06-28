using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoNet.ECS;
using System.Collections.Generic;
using TiledSharp;

namespace MonoNet.Tiled
{
    public delegate void ObjectLoaded(TmxObject loadedObject);

    /// <summary>
    /// Used to manage Tiled resources.
    /// </summary>
    public class TiledBase : Component
    {
        public bool unloadUnusedImages;
        public event ObjectLoaded OnObjectLoaded;

        private ContentManager content;
        private Dictionary<string, TextureWithReferenceCount> pathForImage;

        /// <summary>
        /// Helper class to create a pointer to an image with a reference counter.
        /// </summary>
        public class TextureWithReferenceCount
        {
            public Texture2D image;
            public int referenceCount;

            public TextureWithReferenceCount(Texture2D image)
            {
                this.image = image;
                referenceCount = 0;
            }
        }

        /// <summary>
        /// Inits the TiledBase.
        /// </summary>
        /// <param name="content">The content from where the tilesets are loaded from.</param>
        public void Set(ContentManager content)
        {
            this.content = content;
            pathForImage = new Dictionary<string, TextureWithReferenceCount>();
        }

        /// <summary>
        /// Adds a map.
        /// </summary>
        /// <param name="path">The path of the map inside content.</param>
        /// <returns>The map in form of a TiledMapComponent.</returns>
        public TiledMapComponent AddMap(string path)
        {
            TmxMap map = content.Load<TmxMap>(path);

            // Load every tileset
            for (int i = 0; i < map.Tilesets.Count; i++)
            {
                // Get the location for the image from the tileset
                string imageLocation = GetPath(map.Tilesets[i].Image.Source);

                // If the image is already loaded then ignore it
                if (pathForImage.ContainsKey(imageLocation) == false)
                {
                    Texture2D image = content.Load<Texture2D>(imageLocation);

                    pathForImage.Add(imageLocation, new TextureWithReferenceCount(image));
                }

                // increase the reference count
                pathForImage[imageLocation].referenceCount++;
            }

            // Send messages for every loaded object
            if (OnObjectLoaded != null)
            {
                for (int i = 0; i < map.ObjectGroups.Count; i++)
                {
                    for (int j = 0; j < map.ObjectGroups[i].Objects.Count; j++)
                    {
                        OnObjectLoaded.Invoke(map.ObjectGroups[i].Objects[j]);
                    }
                }
            }

            // Create the TiledMapComponent
            TiledMapComponent tiledMapComponent = Actor.AddComponent<TiledMapComponent>();
            tiledMapComponent.Set(map);
            return tiledMapComponent;
        }

        /// <summary>
        /// Removes a map.
        /// </summary>
        /// <param name="component">The map to be removed.</param>
        public void RemoveMap(TiledMapComponent component)
        {
            TmxMap map = component.Map;

            for (int i = 0; i < map.Tilesets.Count; i++)
            {
                string imageLocation = GetPath(map.Tilesets[i].Image.Source);

                // Try and get the reference with the string location
                if (pathForImage.TryGetValue(imageLocation, out TextureWithReferenceCount reference) == false)
                    continue;

                // decrement the reference count and check if it 0 or lower
                if (--reference.referenceCount <= 0)
                {
                    // If images should be disposed then delete the image
                    if (unloadUnusedImages == true)
                        reference.image.Dispose();

                    // delete the TexutreWithReferenceCount from pathForImage
                    pathForImage.Remove(imageLocation);
                }
            }
        }

        private string GetPath(string imageSource)
        {
            return imageSource.Substring(8, imageSource.Length - 8 - 4); // remove "content/" and ".tmx"
        }

        /// <summary>
        /// Gets an image from an imageLocation. The map must have been loaded from this base beforehand!
        /// </summary>
        /// <param name="imageSource">The source of the image.</param>
        /// <returns>The image as a Texture2D.</returns>
        public Texture2D GetTilesetImage(string imageSource)
        {
            return pathForImage[imageSource]?.image;
        }
    }
}
