using Microsoft.Xna.Framework.Graphics;
using MonoNet.Interfaces;
using MonoNet.Util;
using MonoNet.Util.Pools;
using System;
using System.Collections.Generic;

namespace MonoNet.ECS
{
    public delegate void ComponentAdded(Component addedComponent);

    /// <summary>
    /// A stage holds and manages actors. The world in an entity component system.
    /// </summary>
    public class Stage : IUpdateable, IDrawable, IDisposable
    {
        public static readonly int MAXIMUM_LAYERS = 32; // integer bit length so one can perform bit operations for the layers.

        public event ComponentAdded ComponentAdded;
        public int Layers { get; private set; }

        private LinkedList<Actor> actors;
        private LinkedListNode<Actor>[] layerNodes;

        private List<Actor> toAddActors;
        private List<Actor> toDeleteActors;

        private Pool<Actor> actorPool;

        private int? deleteRequest = null;

        /// <summary>
        /// Creates a new stage.
        /// </summary>
        /// <param name="layers">How many different layers are on the stage.</param>
        /// <param name="actorPool">The pool of actors the stage draws from when creating actors.</param>
        public Stage(int layers, Pool<Actor> actorPool)
        {
            if (Layers > MAXIMUM_LAYERS)
                throw new ArgumentOutOfRangeException("layers", layers, "A maximum of " + MAXIMUM_LAYERS + " layers are allowed on a stage!");

            Layers = layers;

            actors = new LinkedList<Actor>();
            toAddActors = new List<Actor>();
            toDeleteActors = new List<Actor>();

            this.actorPool = actorPool;
            layerNodes = new LinkedListNode<Actor>[layers];

            // Create empty actors and set them to the layerNodes
            for (int i = 0; i < layers; i++)
            {
                Actor actor = new Actor();
                actor.Initialize(this, i);
                layerNodes[i] = actors.AddLast(actor);
            }
        }

        /// <summary>
        /// Grows the stage to the specefied layerCount.
        /// </summary>
        /// <param name="newLayerCount">The new number of layers of this stage.</param>
        public void GrowToLayers(int newLayerCount)
        {
            // Check argument and print error if something is wrong.
            if (newLayerCount > MAXIMUM_LAYERS || newLayerCount < Layers)
            {
                Log.Warn("Stage tried to grow to an illegal amount of layers! Previously:" + Layers + ", Tried to grow to:" + newLayerCount);
                return;
            }

            // Grow the layerNodeArray
            LinkedListNode<Actor>[] newLayerNodes = new LinkedListNode<Actor>[newLayerCount];
            Array.Copy(layerNodes, newLayerNodes, layerNodes.Length);

            // Add the new layerNodeElements
            for (int i = Layers; i < newLayerCount; i++)
            {
                Actor actor = new Actor();
                actor.Initialize(this, i);
                newLayerNodes[i] = actors.AddLast(actor);
            }

            // Lasty set all old references to the new ones.
            Layers = newLayerCount;
            layerNodes = newLayerNodes;
        }

        /// <summary>
        /// Creates an actor on this stage.
        /// </summary>
        /// <param name="layer">The layer that the actor is on.</param>
        /// <returns>The created actor.</returns>
        public Actor CreateActor(int layer)
        {
            // Check if the layer is correct
            if (layer < 0 || layer >= Layers)
            {
                Log.Warn("Actor has wrong layer assigned. Setting it to 0! (" + layer + "/" + (Layers - 1) + ")");
                layer = 0;
            }

            // Get the Actor from the Pool, initialize and add him to the toAdd list
            Actor actor = actorPool.Get();
            actor.Initialize(this, layer);
            toAddActors.Add(actor);
            return actor;
        }

        /// <summary>
        /// Removes an Actor from the stage.
        /// </summary>
        /// <param name="actor">The actor to be removed.</param>
        public void DeleteActor(Actor actor)
        {
            if (toDeleteActors.Contains(actor) == false)
                toDeleteActors.Add(actor);
        }

        /// <summary>
        /// Deletes all Actors from Stage at the end of the Update call.
        /// </summary>
        /// <param name="keepActorsFromLayer0">Wheter to keep all actors in layer 0.</param>
        public void DeleteAllActors(bool keepActorsFromLayer0)
        {
            int layerToStart = 0;
            if (keepActorsFromLayer0 == true)
            {
                if (Layers == 1)
                {
                    Log.Warn("Can not delete actors from layer other than 0 since there is only the layer 0.");
                    return;
                }
                layerToStart = 1;
            }
            deleteRequest = layerToStart;
        }

        /// <summary>
        /// Invokes component added event.
        /// </summary>
        /// <param name="component">The component that was added to an actor.</param>
        public void OnComponentAdded(Component component)
        {
            ComponentAdded?.Invoke(component);
        }

        /// <summary>
        /// Updates the stage and all its actors.
        /// </summary>
        public void Update()
        {
            foreach (Actor actor in actors)
                actor.Update();

            if (toDeleteActors.Count > 0)
            {
                // Go through all the actors that are supposed to be deleted.
                for (int i = 0; i < toDeleteActors.Count; i++)
                {
                    RemoveActorFromList(toDeleteActors[i]);

                    toDeleteActors[i].Dispose();
                    actorPool.Free(toDeleteActors[i]);
                }

                // Lastly clear the list
                toDeleteActors.Clear();
            }

            // If there is a delete Request, then delete all actors.
            if (deleteRequest.HasValue == true)
            {
                LinkedListNode<Actor> startingActor = layerNodes[deleteRequest.Value]; // Get the layerNode actor as a stop point
                LinkedListNode<Actor> currentActor = layerNodes[deleteRequest.Value].Next; // The currentActor to be deleted
                while (startingActor != currentActor && currentActor.Value.Layer != 0)
                {
                    if (currentActor.Value == layerNodes[currentActor.Value.Layer].Value)
                    {
                        currentActor = currentActor.Next;
                        continue;
                    }

                    Actor toDelete = currentActor.Value;
                    currentActor = currentActor.Next;
                    actors.Remove(currentActor.Previous);

                    toDelete.Dispose();
                    actorPool.Free(toDelete);
                }
            }

            // Go through all actors that should be added.
            if (toAddActors.Count > 0)
            {
                for (int i = 0; i < toAddActors.Count; i++)
                    actors.AddAfter(layerNodes[toAddActors[i].Layer], toAddActors[i]);

                toAddActors.Clear();
            }
        }

        private void RemoveActorFromList(Actor toRemove)
        {
            // Get the layer and the current and next layerNode.
            int layer = toRemove.Layer;
            LinkedListNode<Actor> layerNode = layerNodes[layer];
            LinkedListNode<Actor> nextLayerNode = layerNodes[layer + 1 == layerNodes.Length ? 0 : layer + 1];

            // Go through the actors after the layer node.
            LinkedListNode<Actor> currentNode = layerNode.Next;
            while (currentNode != nextLayerNode) // if the next layer node was reached break the loop.
            {
                // If it was found then simply remove the actor.
                if (currentNode.Value == toRemove)
                {
                    actors.Remove(currentNode);
                    return;
                }
                currentNode = currentNode.Next;
            }

            // Actor was not found.
            Log.Warn("Could not find actor to delete!");
        }

        /// <summary>
        /// Draws the stage and all its actors.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Actor actor in actors)
                actor.Draw(spriteBatch);
        }

        /// <summary>
        /// Disposes all actors and the stage.
        /// </summary>
        public void Dispose()
        {
            foreach (Actor actor in actors)
                actor.Dispose();
        }
    }
}
