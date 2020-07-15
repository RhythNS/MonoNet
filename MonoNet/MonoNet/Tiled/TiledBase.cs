using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.Util;
using System.Collections.Generic;
using TiledSharp;

namespace MonoNet.Tiled
{
    public delegate void ObjectLoaded(List<TiledMapComponent> allMapComponents, TmxObject loadedObject);
    public delegate void CollisionHitboxLoaded(TiledMapComponent onComponent, Transform2 componentTrans, TmxObject hitbox, float localX, float localY);

    /// <summary>
    /// Used to manage Tiled resources.
    /// </summary>
    public class TiledBase : Component
    {
        public bool unloadUnusedImages;
        public event ObjectLoaded OnObjectLoaded;
        public event CollisionHitboxLoaded OnCollisionHitboxLoaded;

        private ContentManager content;
        private Dictionary<string, TextureWithReferenceCount> imageForPath;
        private List<TmxMap> loadedMaps = new List<TmxMap>();

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
        /// Adds a map to a single actor.
        /// </summary>
        /// <param name="actor">The actor to place the map onto.</param>
        /// <param name="path">The path of the map inside content.</param>
        /// <param name="addTransform">If a new transform component should be added to the actir,</param>
        /// <returns>The map in form of a TiledMapComponent.</returns>
        public TiledMapComponent AddMap(Actor actor, string path, bool addTransform = false)
        {
            TmxMap map = InnerLoadMap(path);

            // Create the TiledMapComponent
            if (addTransform == true)
                actor.AddComponent<Transform2>();

            TiledMultiLayerComponent tiledMapComponent = actor.AddComponent<TiledMultiLayerComponent>();
            tiledMapComponent.Set(map, this);

            // Set the shared maps. In this case it is only the single multilayercomponent.
            List<TiledMapComponent> sharedMaps = new List<TiledMapComponent>
            {
                tiledMapComponent
            };
            tiledMapComponent.SetConnectedMaps(sharedMaps);
            loadedMaps.Add(map);

            // Load all objectss.
            NotifyLoadObjects(map, sharedMaps);

            return tiledMapComponent;
        }

        /// <summary>
        /// Adds a map to a stage spread to a number of different actors.
        /// </summary>
        /// <param name="stage">The stage where the map should be created.</param>
        /// <param name="path">The path to the map inside the content folder.</param>
        /// <param name="growStage">If the stage should grow its layers to support each layer of the loaded map.</param>
        /// <param name="useSharedPositionTransform">Wheter the transform should be a SharedPositionTransform or a normal one.</param>
        /// <returns>Each TiledMapComponent that is needed to display the map.</returns>
        public TiledMapComponent[] AddMap(Stage stage, string path, bool growStage = false, bool useSharedPositionTransform = false)
        {
            TmxMap map = InnerLoadMap(path);

            // In the following code we go through each tileLayer in map and look where it wants to be
            // placed on the stage. This is done via the "layer" custom property on each tile layer.
            // If this property is not set in the tmx file then we simply set the layer to the previously
            // layer + 1. Layers can also be skipped. For example the layer property could go from 1 on the first
            // layer to 3 on the second layer. One might do this so that sprites can be put on layer 2 which
            // ensures total control of what is supposed to be drawn when.

            // Create all variables that are needed for the loop
            bool warnedNotAbleToGrow = false;
            int atLayer = -1;
            List<int> layersToPlaceOn = new List<int>(map.TileLayers.Count);

            for (int i = 0; i < map.TileLayers.Count; i++)
            {
                // first set the default layer to be placed on as the lastlayer + 1
                int layerToPlaceOn = atLayer + 1;

                // Try to see if a layer property is on the tilelayer.
                if (map.TileLayers[i].Properties.TryGetValue("layer", out string value) == true)
                {
                    if (int.TryParse(value, out int newLayer) == false) // if we can not parse it print an error.
                        Log.Warn("TileLayer " + map.TileLayers[i].Name + " layer could not be parsed to int: " + value);
                    else // otherwise set the layerToPlace on to the value we just read and parsed.
                        layerToPlaceOn = newLayer;
                }

                // If the newLayer is smaller than the previous layer print an error and set it to previous layer + 1.
                if (layerToPlaceOn < atLayer)
                {
                    Log.Warn("TileLayer " + map.TileLayers[i].Name + " layer order should increment with each layer or stay the same value!");
                    layerToPlaceOn = atLayer + 1;
                }

                // Check to see if the layer is bigger than the stage has layers and if the stage should not grow.
                if (layerToPlaceOn >= stage.Layers && growStage == false)
                {
                    // Print an error about this once.
                    if (warnedNotAbleToGrow == false)
                    {
                        Log.Warn("Stage can not grow but layers on TiledMap are above the layer count. This might result in wrong rendering of the map");
                        warnedNotAbleToGrow = true;
                    }
                    // Set the layer to the maximum amount the stage can have layers.
                    layerToPlaceOn = stage.Layers - 1;
                }

                // Lastly put it in the layerToPlaceOn list and set the new layer number to the atLayer for the next loop.
                layersToPlaceOn.Add(layerToPlaceOn);
                atLayer = layerToPlaceOn;
            }

            // If we should grow the stage then do this now.
            int growToLayers = layersToPlaceOn[layersToPlaceOn.Count - 1] + 1;
            if (growStage == true && growToLayers > stage.Layers)
                stage.GrowToLayers(growToLayers);

            // Now that every index is set, we have to create each TiledMapComponent. For this we go through the
            // previously set indexes and create a batch when the layer is different.
            int lastIndex = 0;
            List<TiledMapComponent> mapComponents = new List<TiledMapComponent>();
            for (int i = 1; i < layersToPlaceOn.Count; i++)
            {
                if (layersToPlaceOn[lastIndex] != layersToPlaceOn[i])
                {
                    mapComponents.Add(InnerAddMapWithActor(map, stage, layersToPlaceOn[lastIndex], useSharedPositionTransform, lastIndex, i - 1));
                    lastIndex = i;
                }
            }
            // There now should still be one batch of maps that we should create.
            mapComponents.Add(InnerAddMapWithActor(map, stage, layersToPlaceOn[lastIndex], useSharedPositionTransform, lastIndex, layersToPlaceOn.Count - 1));

            // If shared transforms are used then set their group.
            if (useSharedPositionTransform == true)
            {
                SharedPositionTransform2[] sharedTrans = new SharedPositionTransform2[mapComponents.Count];
                for (int i = 0; i < sharedTrans.Length; i++)
                    sharedTrans[i] = mapComponents[i].Actor.GetComponent<SharedPositionTransform2>();
                sharedTrans[0].SetGroup(sharedTrans);
            }

            // Go through each map and set the connected maps which are all the maps we just created.
            for (int i = 0; i < mapComponents.Count; i++)
                mapComponents[i].SetConnectedMaps(mapComponents);
            loadedMaps.Add(map);

            // load all objects
            NotifyLoadObjects(map, mapComponents);

            // Lastly return the mapcomponents.
            return mapComponents.ToArray();
        }

        /// <summary>
        /// Helper method for AddMap with stage. Creates either a TiledSingleLayer or TiledMultiLayer depending
        /// on how many layers should be created.
        /// </summary>
        /// <param name="map">A reference to the TiledMap of which the layer should be made from.</param>
        /// <param name="stage">A refernce to the stage where the actor comes from which has the new map component.</param>
        /// <param name="actorLayer">The layer of where the created actor should be placed upon.</param>
        /// <param name="from">First index of layer to be created.</param>
        /// <param name="to">Last index of layer to be created.</param>
        /// <returns>The new TiledMapComponent of the specified layers.</returns>
        private TiledMapComponent InnerAddMapWithActor(TmxMap map, Stage stage, int actorLayer, bool useSharedTrans, int from, int to)
        {
            // First create and actor on the specified layer and add a transform to it.
            Actor actor = stage.CreateActor(actorLayer);
            if (useSharedTrans == true)
                actor.AddComponent<SharedPositionTransform2>();
            else
                actor.AddComponent<Transform2>();

            int count = to - from;
            if (count == 0) // If only one layer should be made
            {
                TiledSingleLayerComponent singleLayer = actor.AddComponent<TiledSingleLayerComponent>();
                singleLayer.Set(map, this, from);
                return singleLayer;
            }
            else // If more than one layer should be made
            {
                TiledMultiLayerComponent multiLayer = actor.AddComponent<TiledMultiLayerComponent>();
                multiLayer.Set(map, this, from, count + 1);
                return multiLayer;
            }
        }

        /// <summary>
        /// Loads all Tileset images into the tiledBase.
        /// </summary>
        /// <param name="path">The path to the map inside content.</param>
        /// <returns>The map with all tilesets loaded. </returns>
        private TmxMap InnerLoadMap(string path)
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
            }

            return map;
        }

        /// <summary>
        /// Triggeres event for every loaded object.
        /// </summary>
        /// <param name="map"></param>
        private void NotifyLoadObjects(TmxMap map, List<TiledMapComponent> allMapComponents)
        {
            if (OnObjectLoaded == null)
                return;

            for (int i = 0; i < map.ObjectGroups.Count; i++)
            {
                for (int j = 0; j < map.ObjectGroups[i].Objects.Count; j++)
                {
                    OnObjectLoaded.Invoke(allMapComponents, map.ObjectGroups[i].Objects[j]);
                }
            }
        }

        /// <summary>
        /// Triggers the event for a loaded Hitbox.
        /// </summary>
        /// <param name="onComponent">The component from where the hitbox was loaded.</param>
        /// <param name="componentTrans">The transform of the component.</param>
        /// <param name="hitbox">The hitbox which should be created.</param>
        public void NotifyHitboxLoaded(TiledMapComponent onComponent, Transform2 componentTrans, TmxObject hitbox, float localX, float localY)
            => OnCollisionHitboxLoaded?.Invoke(onComponent, componentTrans, hitbox, localX, localY);

        /// <summary>
        /// Removes a map.
        /// </summary>
        /// <param name="map">The map to be removed.</param>
        public void RemoveMap(TmxMap map)
        {
            // If the map was already unloaded then do nothing.
            if (loadedMaps.Remove(map) == false)
                return;

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
