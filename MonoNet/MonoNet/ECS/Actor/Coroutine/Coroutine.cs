using System;
using System.Collections;
using System.Collections.Generic;

namespace MonoNet.ECS
{
    /// <summary>
    /// Uses an IEnumerator to execute code in "parallel" with the actor it is on.
    /// The coroutine executes every frame after each component was updated. It stops
    /// execution after the IEnumerator yields something. The yield can be another IEnumerator
    /// or a yield instruction. Anything else is ignored and the execution stops until next frame.
    /// </summary>
    public class Coroutine
    {
        public bool Finished { get; private set; }
        private bool exitRequested;

        /// <summary>
        /// The stack of IEnumerator. This is so a yield can be another IEnumerator. The most recent given IEnumerator is
        /// executed until it finishes. If there are none on the stack then the coroutine is finished.
        /// </summary>
        private Stack<IEnumerator> enumsStack;
        private Action onFinished;

        private YieldInstruction executingYield;

        /// <summary>
        /// Creates a new Coroutine.
        /// </summary>
        /// <param name="enumerator">The enumerator which is being run in this coroutine.</param>
        /// <param name="onFinished">A function to call when the coroutine finishes.</param>
        public Coroutine(IEnumerator enumerator, Action onFinished = null)
        {
            enumsStack = new Stack<IEnumerator>();
            enumsStack.Push(enumerator);

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

            // If there is a yield instruction ongoing then do that first.
            if (executingYield != null)
            {
                if (executingYield.IsDone() == true)
                    executingYield = null;
                return false;
            }

            IEnumerator currentEnum = enumsStack.Peek();

            // If the enumerator is finished then finish the coroutine aswell. 
            if (currentEnum.MoveNext() == false)
            {
                enumsStack.Pop();
                // If there are still IEnumerators on the stack then don't stop the coroutine.
                if (enumsStack.Count > 0)
                    return false;

                // Everything is finished so end the coroutine.
                OnFinished();
                return true;
            }

            object yield = currentEnum.Current;

            // Check to see if we can do anything with the current yield. If not then simply continue.
            if (yield == null)
                return false;

            // If the current yield is an YieldInstruction then save it and start executing it next frame.
            if (yield is YieldInstruction instruction)
                executingYield = instruction;

            // If the yield is an IEnumerator then add it to the stack
            if (yield is IEnumerator newEnum)
                enumsStack.Push(newEnum);

            return false;
        }
    }
}
