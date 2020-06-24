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

        private YieldInstruction currentYield;

        /// <summary>
        /// Creates a new Coroutine.
        /// </summary>
        /// <param name="enumerator">The enumerator which is being run in this coroutine.</param>
        /// <param name="onFinished">A function to call when the coroutine finishes.</param>
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
            // If exit is requested simply stop and return true.
            if (exitRequested == true)
            {
                OnFinished();
                return true;
            }

            // If there is a yield instruction ongoing then first do that.
            if (currentYield != null)
            {
                if (currentYield.IsDone() == true)
                    currentYield = null;
                return false;
            }

            // If the enumerator is finished then finish the coroutine aswell. 
            if (enumerator.MoveNext() == false)
            {
                OnFinished();
                return true;
            }

            // Check to see if we can do anything with the current yield. If not then simply continue.
            if (enumerator.Current == null)
                return false;

            // If the current yield is an YieldInstruction then save it and start executing it next frame.
            if (enumerator.Current is YieldInstruction instruction)
                currentYield = instruction;
            return false;
        }
    }
}
