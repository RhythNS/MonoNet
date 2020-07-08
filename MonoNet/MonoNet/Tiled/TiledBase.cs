using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using System.Collections.Generic;
using TiledSharp;

namespace MonoNet.Tiled
{
    public delegate void ObjectLoaded(TiledMapComponent onMap, TmxObject loadedObject);

    /// <summary>
    /// Used to manage Tiled resources.
    /// </summary>
    public class TiledBase : Component
    {
        public bool unloadUnusedImages;
        public event ObjectLoaded OnObjectLoaded;

        private ContentManager content;
        private Dictionary<string, TextureWithReferenceCount> imageForPath;
        private List<TiledMapComponent> loadedMaps = new List<TiledMapComponent>();

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
            imageForPath = new Dictionary<string, TextureWithReferenceCount>();
        }

        /// <summary>
        /// Adds a map.
        /// </summary>
        /// <param name="actor">The actor to place the map onto.</param>
        /// <param name="path">The path of the map inside content.</param>
        /// <param name="addTransform">If a new transform component should be added to the actir,</param>
        /// <returns>The map in form of a TiledMapComponent.</returns>
        public TiledMapComponent AddMap(Actor actor, string path, bool addTransform = false)
        {
            TmxMap map = content.Load<TmxMap>(path);

            // Load every tileset
            for (int i = 0; i < map.Tilesets.Count; i++)
            {
                // Unedited image source
                string source = map.Tilesets[i].Image.Source;

                // If the image is already loaded then ignore it
                if (imageForPath.ContainsKey(source) == false)
                {
                    // Get the acctual location for the image from the tileset
                    string imageLocation = GetPath(map.Tilesets[i].Image.Source);
                    Texture2D image = content.Load<Texture2D>(imageLocation);

                    imageForPath.Add(source, new TextureWithReferenceCount(image));
                }

                // increase the reference count
                imageForPath[source].referenceCount++;
            }
            
            // Create the TiledMapComponent
            if (addTransform == true)
                actor.AddComponent<Transform2>();
            TiledMapComponent tiledMapComponent = actor.AddComponent<TiledMapComponent>();
            tiledMapComponent.Set(map, this);
            loadedMaps.Add(tiledMapComponent);

            // Send messages for every loaded object
            if (OnObjectLoaded != null)
            {
                for (int i = 0; i < map.ObjectGroups.Count; i++)
                {
                    for (int j = 0; j < map.ObjectGroups[i].Objects.Count; j++)
                    {
                        OnObjectLoaded.Invoke(tiledMapComponent, map.ObjectGroups[i].Objects[j]);
                    }
                }
            }

            return tiledMapComponent;
        }

        /// <summary>
        /// Removes a map.
        /// </summary>
        /// <param name="component">The map to be removed.</param>
        public void RemoveMap(TiledMapComponent component)
        {
            // If the map was already unloaded then do nothing.
            if (loadedMaps.Remove(component) == false)
                return;

            TmxMap map = component.Map;

            for (int i = 0; i < map.Tilesets.Count; i++)
            {
                string imageLocation = GetPath(map.Tilesets[i].Image.Source);

                // Try and get the reference with the string location
                if (imageForPath.TryGetValue(imageLocation, out TextureWithReferenceCount reference) == false)
                    continue;

                // decrement the reference count and check if it 0 or lower
                if (--reference.referenceCount <= 0)
                {
                    // If images should be disposed then delete the image
                    if (unloadUnusedImages == true)
                        reference.image.Dispose();

                    // delete the TexutreWithReferenceCount from pathForImage
                    imageForPath.Remove(imageLocation);
                }
            }
        }

        /// <summary>
        /// Removes content and the .tmx ending from string.
        /// </summary>
        /// <param name="imageSource">The original string.</param>
        /// <returns>The formatted path string.</returns>
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
            return imageForPath[imageSource]?.image;
        }
    }
}
