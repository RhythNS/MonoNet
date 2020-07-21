namespace MonoNet.Util.Datatypes
{
    /// <summary>
    /// Helper struct to get a MultiKey for a Dictionary.
    /// </summary>
    public struct MultiKey<T>
    {
        public T object1;
        public T object2;

        public MultiKey(T object1, T object2)
        {
            // To have a proper ordering, the first object is the one with a higher hashValue.
            if (object1.GetHashCode() > object2.GetHashCode())
            {
                this.object1 = object1;
                this.object2 = object2;
            }
            else
            {
                this.object1 = object2;
                this.object2 = object1;
            }
        }
    }
}
