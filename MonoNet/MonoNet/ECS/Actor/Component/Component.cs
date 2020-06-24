using System;
using System.Collections;

namespace MonoNet.ECS
{
    public abstract class Component
    {
        public Actor Actor { get; private set; }

        public virtual void Initialize(Actor actor)
        {
            Actor = actor;
            OnInitialize();
        }

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
