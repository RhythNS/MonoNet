using System.Net.Sockets;

namespace MononetMasterServer
{
    /// <summary>
    /// Provides data for a server in the master server list.
    /// </summary>
    public class Server
    {
        /// <summary>
        /// The TcpClient connection to the server.
        /// </summary>
        public TcpClient Client { get; private set; }

        /// <summary>
        /// The port of the server
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Name of the server.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The current amount of players on the server.
        /// </summary>
        public int PlayerCount { get; set; }

        /// <summary>
        /// The maximum allowed number of players.
        /// </summary>
        public int MaxPlayers { get; set; }

        /// <summary>
        /// Creates a new instance of type Server with the corresponding TcpClient.
        /// </summary>
        /// <param name="client"></param>
        public Server(TcpClient client) {
            Client = client;
        }
    }
}
