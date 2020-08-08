using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MononetMasterServer
{
    public class ConnectedServer
    {
        public TcpClient Client { get; private set; }

        public string Name { get; set; }

        private int playerCount = 0;
        public int PlayerCount {
            get {
                return playerCount;
            }
            set {
                if (value >= 0)
                    playerCount = value;
            }
        }
        public int MaxPlayers { get; set; }

        public int Ping { get; set; } = 0;

        public bool FirstMsg { get; set; } = true;

        public ConnectedServer(TcpClient client) {
            Client = client;
        }

        public void HandleConnection() {
            NetworkStream stream = Client.GetStream();

            byte[] message = new byte[1024];
            int byteRead = stream.Read(message, 0, 1024);
            stream.Flush();

            if (byteRead == 0) {
                Client.Close();

                
            }
        }
    }
}
