namespace MonoNet.Network.Commands
{
    /// <summary>
    /// Helper class for managing which rpcs have already been executed and which are supposed
    /// to be executed.
    /// </summary>
    public class CommandPackageManager
    {
        private readonly bool[] recievedPackaged = new bool[256];

        private static readonly ResetRange[] resets =
        {
            new ResetRange(0, 64, 128),
            new ResetRange(64, 128, 192),
            new ResetRange(128, 192, 256),
            new ResetRange(192, 0, 64)
        };

        private struct ResetRange
        {
            public int id;
            public int clearFrom, clearTo;

            public ResetRange(int id, int clearFrom, int clearTo)
            {
                this.id = id;
                this.clearFrom = clearFrom;
                this.clearTo = clearTo;
            }
        }

        /// <summary>
        /// Returns wheter the rpc should be executed or not.
        /// </summary>
        /// <param name="id">The id of the rpc.</param>
        /// <returns>Wheter it should be executed or not.</returns>
        public bool RecievedShouldExecute(byte id)
        {
            for (int i = 0; i < resets.Length; i++)
            {
                if (id == resets[i].id)
                {
                    for (int j = resets[i].clearFrom; j < resets[i].clearTo; j++)
                    {
                        recievedPackaged[j] = false;
                    }
                    break;
                }
            }

            if (recievedPackaged[id] == true)
                return false;

            recievedPackaged[id] = true;
            return true;
        }
    }
}
