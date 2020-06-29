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
        public event ComponentAdded ComponentAdded;
        public int Layers { get; private set; }

        private LinkedList<Actor> actors;
        private LinkedListNode<Actor>[] layerNodes;

        private List<Actor> toAddActors;
        private List<Actor> toDeleteActors;

        private Pool<Actor> actorPool;

        /// <summary>
        /// Creates a new stage.
        /// </summary>
        /// <param name="layers">How many different layers are on the stage.</param>
        /// <param name="actorPool">The pool of actors the stage draws from when creating actors.</param>
        public Stage(int layers, Pool<Actor> actorPool)
        {
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
        /// Creates an actor on this stage.
        /// </summary>
        /// <param name="layer">The layer that the actor is on.</param>
        /// <returns>The created actor.</returns>
        public Actor CreateActor(int layer)
        {
            // Check if the layer is correct
            if (layer < 0 || layer >= Layers)
            {
                Log.Warn("Actor has wrong layer assigned. Setting it to 0! (" + layer + "/" + layer + ")");
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
