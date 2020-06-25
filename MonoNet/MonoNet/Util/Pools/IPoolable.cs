namespace MonoNet.Util.Pools
{
    /// <summary>
    /// Interface for objects that can be poolable.
    /// </summary>
    public interface IPoolable
    {
        /// <summary>
        /// Used for resetting and disposing any references and made
        /// ready to be reused.
        /// </summary>
        void Reset();
    }
}
