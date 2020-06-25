using System.Collections;
using System.Collections.Generic;

namespace MonoNet.Util.Datatypes
{
    /// <summary>
    /// A simulated 2D array on a simple 1D array to reduce access time for elements in the arary.
    /// Can throw OutOfBoundsExceptions!
    /// </summary>
    /// <typeparam name="T">Type of elements</typeparam>
    public class Fast2DArray<T> : IEnumerable<T>
    {
        private readonly T[] array;

        public int XSize { private set; get; }
        public int YSize { private set; get; }

        public Fast2DArray(int xSize, int ySize)
        {
            XSize = xSize;
            YSize = ySize;
            array = new T[xSize * ySize];
        }

        /// <summary>
        /// Returns the element at the specified position of the array.
        /// </summary>
        public T Get(int x, int y) => array[x * XSize + y];

        /// <summary>
        /// Sets the element at the specified position of the array.
        /// </summary>
        public void Set(T element, int x, int y) => array[x * XSize + y] = element;

        /// <summary>
        /// Util method to check if given ints are in the bounds of the array.
        /// </summary>
        public bool InBounds(int x, int y) => x > -1 && x < XSize && y > -1 && y < YSize;

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < array.Length; i++)
                yield return array[i];
        }

        /// <summary>
        /// Get the enumerator for a single array specified by the int atX
        /// </summary>
        public IEnumerator<T> GetEnumerator(int atX)
        {
            int to = atX * XSize + YSize;
            for (int i = atX * XSize; i < to; i++)
                yield return array[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}