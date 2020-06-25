namespace MonoNet.Interfaces
{
    /// <summary>
    /// Interface for everything that can be updated in a frame.
    /// </summary>
    public interface IUpdateable
    {
        /// <summary>
        /// Called per frame.
        /// </summary>
        void Update();
    }
}
