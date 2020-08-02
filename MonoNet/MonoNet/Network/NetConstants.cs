using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoNet.Network
{
    /// <summary>
    /// Holds constants for the networking.
    /// </summary>
    public class NetConstants
    {
        public readonly static byte MAX_PLAYERS = 8;
        public readonly static int MAX_NAME_LENGTH = 15;

        public static readonly float SERVER_SEND_RATE_PER_SECOND = 1 / 30f;
        public static readonly float CLIENT_SEND_RATE_PER_SECOND = 1 / 60f;
        public static readonly TimeSpan TIMEOUT_TIME = new TimeSpan(0, 0, 8);
    }
}
