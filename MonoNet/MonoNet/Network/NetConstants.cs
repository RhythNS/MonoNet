using System;

namespace MonoNet.Network
{
    /// <summary>
    /// Holds constants for the networking.
    /// </summary>
    public class NetConstants
    {
        /// <summary>
        /// Max players that can connect to the server.
        /// </summary>
        public readonly static byte MAX_PLAYERS = 8;
        /// <summary>
        /// Max number of chars a name can have.
        /// </summary>
        public readonly static int MAX_NAME_LENGTH = 15;
        /// <summary>
        /// Prefered string length that a string that is sent should have.
        /// </summary>
        public readonly static int MAX_PREF_STRING_LENGTH = 6;

        /// <summary>
        /// How many packages a server sends per second.
        /// </summary>
        public static readonly float SERVER_SEND_RATE_PER_SECOND = 1 / 30f;
        /// <summary>
        /// How many packages a client sends per second.
        /// </summary>
        public static readonly float CLIENT_SEND_RATE_PER_SECOND = 1 / 60f;
        /// <summary>
        /// How long until a connection is considered lost.
        /// </summary>
        public static readonly TimeSpan TIMEOUT_TIME = new TimeSpan(0, 0, 8);
        /// <summary>
        /// The ip of the master server.
        /// </summary>
        public static readonly string MASTER_SERVER_IP = "176.57.171.145";
    }
}
