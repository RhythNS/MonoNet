using System.Collections.Generic;

namespace MonoNet.Util.Pools
{
    public class Pool<T> where T : IPoolable, new()
    {
        private LinkedList<T> availableElements;
        private readonly int maxCapacity;

        /// <summary>
        /// Creates a new Pool.
        /// </summary>
        /// <param name="maxCapacity">How many elements the pool can hold.</param>
        /// <param name="startingAmount">How many elements are created when the pool starts.</param>
        public Pool(int maxCapacity, int startingAmount)
        {
            if (startingAmount > maxCapacity)
                throw new System.ArgumentException("Starting amount can not be bigger than max capacity!");

            availableElements = new LinkedList<T>();
            this.maxCapacity = maxCapacity;

            for (int i = 0; i < startingAmount; i++)
                availableElements.AddFirst(new T());
        }

        /// <summary>
        /// Frees given element and returns it to the pool if the pool is not full already.
        /// </summary>
        /// <param name="element">The element to be freed.</param>
        public void Free(T element)
        {
            element.Reset();
            if (availableElements.Count < maxCapacity)
                availableElements.AddFirst(element);
        }

        /// <summary>
        /// Gets an element from the pool. Creates a new instance if there were none in the pool.
        /// </summary>
        /// <returns></returns>
        public T Get()
        {
            if (availableElements.Count == 0)
                return new T();
            T toReturn = availableElements.First.Value;
            availableElements.RemoveFirst();
            return toReturn;
        }

        /// <summary>
        /// Clears all elements from the pool.
        /// </summary>
        public void Clear()
        {
            availableElements.Clear();
        }
    }
}
