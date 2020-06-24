using Microsoft.Xna.Framework.Graphics;
using MonoNet.Util.Pools;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MonoNet.ECS
{
    public sealed class Actor : IDisposable, IUpdateable, IDrawable, IPoolable
    {
        public Stage Stage { get; private set; }
        public int Layer { get; private set; }

        private List<Coroutine> coroutines;

        private readonly List<Component> components;

        private readonly List<IUpdateable> updateables;
        private readonly List<IDrawable> drawables;
        private readonly List<IDisposable> disposables;

        public Actor()
        {
            components = new List<Component>();

            updateables = new List<IUpdateable>();
            drawables = new List<IDrawable>();
            disposables = new List<IDisposable>();
            coroutines = new List<Coroutine>();
        }

        /// <summary>
        /// Initializes the Actor.
        /// </summary>
        /// <param name="layer">Which layer the actor is on.</param>
        public void Initialize(Stage stage, int layer)
        {
            Stage = stage;
            Layer = layer;
        }

        /// <summary>
        /// Adds a component to the actor.
        /// </summary>
        /// <typeparam name="T">The type of the added component</typeparam>
        public Component AddComponent<T>() where T : Component, new()
        {
            T component = new T();
            component.Initialize(this);

            // Check if the component implements any of these interfaces, if so then add them to their specific list
            if (component is IUpdateable updateable)
                updateables.Add(updateable);
            if (component is IDrawable drawable)
                drawables.Add(drawable);
            if (component is IDisposable disposable)
                disposables.Add(disposable);

            components.Add(component);
            Stage.OnComponentAdded(component);
            return component;
        }

        /// <summary>
        /// Gets the first component of given type.
        /// </summary>
        /// <typeparam name="T">The type of component to look for.</typeparam>
        /// <param name="component">The returning component.</param>
        /// <returns>Wheter it successded.</returns>
        public bool GetComponent<T>(out T component) where T : Component
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].GetType() == typeof(T))
                {
                    component = (T)components[i];
                    return true;
                }
            }

            component = default;
            return false;
        }

        /// <summary>
        /// Gets all components of the given type.
        /// </summary>
        /// <typeparam name="T">The type of component to look for.</typeparam>
        /// <param name="allComponents">The returning components.</param>
        /// <returns>Wheter one component was found.</returns>
        public bool GetAllComponents<T>(T[] allComponents) where T : Component
        {
            List<T> returnList = new List<T>();
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].GetType() == typeof(T))
                {
                    returnList.Add((T)components[i]);
                }
            }

            allComponents = returnList.ToArray();
            return allComponents.Length > 0;
        }

        /// <summary>
        /// Removes the first component of the given type.
        /// </summary>
        /// <typeparam name="T">The type to remove.</typeparam>
        /// <returns>Wheter one component was found and deleted.</returns>
        public bool RemoveComponent<T>() where T : Component
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].GetType() == typeof(T))
                {
                    InnerRemoveComponent(components[i]);
                    components.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes all components of a given type.
        /// </summary>
        /// <typeparam name="T">The type to remove.</typeparam>
        /// <returns>Wheter it found and deleted one.</returns>
        public bool RemoveAllComponents<T>() where T : Component
        {
            bool deletedOne = false;
            for (int i = components.Count - 1; i > -1; i--)
            {
                if (components[i].GetType() == typeof(T))
                {
                    InnerRemoveComponent(components[i]);
                    components.RemoveAt(i);
                    deletedOne = true;
                }
            }

            return deletedOne;
        }

        /// <summary>
        /// Removes the component from the interface lists.
        /// </summary>
        /// <param name="component">The component that whichs references should be deleted.</param>
        private void InnerRemoveComponent(Component component)
        {
            if (component is IUpdateable updateable)
                updateables.Remove(updateable);
            if (component is IDrawable drawable)
                drawables.Remove(drawable);
            if (component is IDisposable disposable)
                disposables.Remove(disposable);
        }

        /// <summary>
        /// Starts a Coroutine with the given IEnumerator.
        /// </summary>
        /// <param name="enumerator">What the Coroutine is supposed to be doing.</param>
        /// <param name="onFinshed">Called when the Coroutine finishes.</param>
        /// <returns>A reference to the started Coroutine.</returns>
        public Coroutine StartCoroutine(IEnumerator enumerator, Action onFinshed = null)
        {
            Coroutine coroutine = new Coroutine(enumerator, onFinshed);
            coroutines.Add(coroutine);
            return coroutine;
        }

        /// <summary>
        /// Calls Draw on each component which implements IDrawable.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < drawables.Count; i++)
                drawables[i].Draw(spriteBatch);
        }

        /// <summary>
        /// Calls Update on each component which implements IUpdable.
        /// </summary>
        public void Update()
        {
            for (int i = 0; i < updateables.Count; i++)
                updateables[i].Update();

            // Update coroutines and remove if one finishes.
            for (int i = coroutines.Count - 1; i > -1; i--)
            {
                if (coroutines[i].Update() == true)
                    coroutines.RemoveAt(i);
            }
        }

        /// <summary>
        /// Calls Dispose on each component which implements Disposable.
        /// </summary>
        public void Dispose()
        {
            for (int i = 0; i < disposables.Count; i++)
                disposables[i].Dispose();
        }

        /// <summary>
        /// Resets the actor to be freed into a pool again.
        /// </summary>
        public void Reset()
        {
            components.Clear();
            components.TrimExcess();

            updateables.Clear();
            updateables.TrimExcess();

            drawables.Clear();
            drawables.TrimExcess();

            disposables.Clear();
            disposables.TrimExcess();

            coroutines.Clear();
            coroutines.TrimExcess();
        }
    }
}
