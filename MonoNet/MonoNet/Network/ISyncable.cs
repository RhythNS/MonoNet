using System.Collections.Generic;

namespace MonoNet.Network
{
    /// <summary>
    /// Components should implement this to make them synchronizable between servers and clients.
    /// </summary>
    public interface ISyncable
    {
        /// <summary>
        /// Synchronizes a component with a given byte array. Increments the pointer based on
        /// data that was read from the array.
        /// </summary>
        /// <param name="data">The data as byte array.</param>
        /// <param name="pointerAt">A pointer to the current index of the array.</param>
        void Sync(byte[] data, ref int pointerAt);

        /// <summary>
        /// Serializes a component to a list of bytes.
        /// </summary>
        /// <param name="data">The list to where the bytes should be written to.</param>
        void GetSync(List<byte> data);
    }
}
