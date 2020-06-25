using System;
using System.Collections;

namespace MonoNet.ECS
{
    /// <summary>
    /// Component in an entity component system. Component needs to implement IUpdateable,
    /// IDrawable or IDisposable in order to be called from the actor. If none are implemented
    /// then the component can still be accessed via GetComponent in actor.
    /// </summary>
    public abstract class Component
    {
        /// <summary>
        /// Reference to the Actor on which the component is on.
        /// </summary>
        public Actor Actor { get; private set; }

        /// <summary>
        /// Called as soon as the Component is created. Can be treated as an constructor.
        /// </summary>
        /// <param name="actor">The actor on which the component is on.</param>
        public void Initialize(Actor actor)
        {
            Actor = actor;
            OnInitialize();
        }

        /// <summary>
        /// Called as soon as the Component is created. Can be treated as an constructor.
        /// </summary>
        protected virtual void OnInitialize() { }

        /// <summary>
        /// Starts a Coroutine with the given IEnumerator.
        /// </summary>
        /// <param name="enumerator">What the Coroutine is supposed to be doing.</param>
        /// <param name="onFinshed">Called when the Coroutine finishes.</param>
        /// <returns>A reference to the started Coroutine.</returns>
        public Coroutine StartCoroutine(IEnumerator enumerator, Action onFinish = null)
        {
            return Actor.StartCoroutine(enumerator, onFinish);
        }
    }
}
