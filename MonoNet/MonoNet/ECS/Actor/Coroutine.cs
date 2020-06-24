using MonoNet.GameSystems;
using System;
using System.Collections;

namespace MonoNet.ECS
{
    public class Coroutine
    {
        public bool Finished { get; private set; }
        private bool exitRequested;

        private Action onFinished;
        private IEnumerator enumerator;

        public Coroutine(IEnumerator enumerator, Action onFinished = null)
        {
            this.enumerator = enumerator;
            this.onFinished = onFinished;
            Finished = false;
            exitRequested = false;
        }

        /// <summary>
        /// Exits the Coroutine.
        /// </summary>
        public void RequestExit()
        {
            exitRequested = true;
        }

        /// <summary>
        /// Called when the Coroutine finishes.
        /// </summary>
        public void OnFinished()
        {
            Finished = true;
            if (onFinished != null)
                onFinished.Invoke();
        }

        /// <summary>
        /// Updates the Coroutine. Returns wheter it should be deleted or not.
        /// </summary>
        public bool Update()
        {
            if (exitRequested == false && enumerator.MoveNext() == true)
                return false;

            OnFinished();
            return true;
        }

        /// <summary>
        /// Basic IEnumerator which waits unitl a given time has elapsed.
        /// </summary>
        /// <param name="seconds">The time to wait in seconds.</param>
        public static IEnumerator WaitForSeconds(float seconds)
        {
            float timer = 0;
            while (timer < seconds)
            {
                timer += Time.Delta;
                yield return null;
            }
        }
    }
}
