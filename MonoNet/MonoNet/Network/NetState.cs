using System.Collections.Generic;

namespace MonoNet.Network
{
    /// <summary>
    /// Represents a tick of all network components in the game.
    /// </summary>
    public class NetState
    {
        private readonly byte[][] rawState = new byte[256][];

        public void Set(int index, byte[] array) => rawState[index] = array;

        /// <summary>
        /// Creates a delta NetState with the given NetState and writes these
        /// values into the given list.
        /// </summary>
        /// <param name="fromState">The previous state.</param>
        /// <param name="difList">A ready to send package with all changes.</param>
        /// <returns>The reference of the given list for chaining.</returns>
        public List<byte> GetDif(NetState fromState, List<byte> difList)
        {
            for (int i = 0; i < rawState.Length; i++)
            {
                // If this NetSyncComponent is not existent continue
                if (rawState[i] == null)
                    continue;

                // Check to see if the given state has the NetStateComponent. If it does
                // then check if it is the same. Otherwise we can directly append the array
                // to the outgoing list.
                if (fromState.rawState[i] != null)
                {
                    // If the length is not the same then we can add it. If it is the same 
                    // then invistigate furthor.
                    if (rawState[i].Length == fromState.rawState[i].Length)
                    {
                        bool shouldContinue = false;

                        // go through the entire array and see if each byte is the same
                        for (int j = 0; j < rawState[i].Length; j++)
                        {
                            if (rawState[i] != fromState.rawState[j])
                            {
                                shouldContinue = true;
                                break;
                            }
                        }

                        // Every byte was the same, so this component does not need to be synchronized.
                        if (shouldContinue == true)
                            continue;
                    }
                }

                difList.Add((byte)i); // Add the id of the NetSyncComponent
                difList.AddRange(rawState[i]); // Add the array of data from the NetSyncComponent
            } // end iterate through each NetSyncComponent

            return difList;
        }
    }
}
