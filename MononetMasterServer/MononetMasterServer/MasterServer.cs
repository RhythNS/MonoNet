using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MononetMasterServer
{
    public class MasterServer
    {
        private static Dictionary<TcpClient, ConnectedServer> connectedServers;
     
        delegate void WriteToLogCallback(string text);

        private bool isRunning = false;

        private TcpListener tcpListener;
        private Thread listenThread;

        public void Start(int port) {
            if (isRunning) {
                RefreshConsole("[INFO] Instance of MasterServer is already running!");
                return;
            }

            isRunning = true;

            connectedServers = new Dictionary<TcpClient, ConnectedServer>();

            tcpListener = new TcpListener(IPAddress.Any, port);
            listenThread = new Thread(ListenForServerConnection);
            listenThread.Start();

            RefreshConsole();
        }

        public void Stop() {
            if (!isRunning) {
                RefreshConsole("[INFO] Instance of MasterServer is already stopped!");
                return;
            }

            isRunning = false;

            connectedServers = new Dictionary<TcpClient, ConnectedServer>();

            if (tcpListener != null)
                tcpListener.Stop();
            if (listenThread != null)
                listenThread.Interrupt();

            RefreshConsole();
        }

        public string Status() {
            string status = "";

            foreach (KeyValuePair<TcpClient, ConnectedServer> server in connectedServers) {
                status += $"{server.Value.Name}, Players: {server.Value.PlayerCount}, Ping: {server.Value.Ping}ms" + Environment.NewLine;
            }

            return status;
        }

        private void ListenForServerConnection() {
            tcpListener.Start();

            TcpClient client = new TcpClient();

            while (true) {
                try {
                    client = tcpListener.AcceptTcpClient();

                    ConnectedServer server = new ConnectedServer(client);

                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientConnection));
                    clientThread.Start(server);
                } catch (SocketException) {
                    // can be ignored (thread stopped and no socket connection)
                    break;
                } catch (Exception ex) {
                    RefreshConsole("[ERROR] " + ex.ToString());
                    break;
                }
            }

            tcpListener.Stop();
        }

        private enum PacketType : byte
        {
            ListServer = 0,
            RefreshServer = 1,
            RequestList = 2
        }

        private void HandleClientConnection(Object serverData) {
            ConnectedServer server = (ConnectedServer)serverData;

            connectedServers.Add(server.Client, server);

            NetworkStream stream = server.Client.GetStream();

            int byteRead;
            while (true) {
                try {
                    byte[] message = new byte[1024];
                    byteRead = stream.Read(message, 0, 1024);
                    stream.Flush();

                    if (byteRead == 0) {
                        break;
                    }

                    PacketType type = (PacketType)message[0];

                    switch (type) {
                        case PacketType.ListServer:
                            int nameLength = BitConverter.ToInt32(message.SubArray(1, 4), 0);
                            string serverName = Encoding.ASCII.GetString(message.SubArray(5, nameLength));
                            int maxPlayers = BitConverter.ToInt32(message.SubArray(5 + nameLength, 4), 0);

                            server.Name = serverName;
                            server.MaxPlayers = maxPlayers;

                            RefreshConsole($"New Server: {server.Name}");
                            break;

                        case PacketType.RefreshServer:
                            int currentPlayers = BitConverter.ToInt32(message.SubArray(1, 4), 0);

                            server.PlayerCount = currentPlayers;

                            RefreshConsole($"Update PlayerCount: {server.PlayerCount}");
                            break;

                        case PacketType.RequestList:

                            RefreshConsole($"Request server list from");
                            break;
                    }
                } catch {
                    // server no longer connected
                    break;
                }
            }

            server.Client.Close();

            connectedServers.Remove(server.Client);
            RefreshConsole();
        }

        public void RefreshConsole(string text = "") {
            Console.Clear();

            Console.WriteLine("[COMMANDS] start - starts the server; stop - stops the server; exit - stops the server and quits the program");
            Console.WriteLine();

            if (isRunning) {
                Console.WriteLine("[SERVER ONLINE]");
                Console.WriteLine();
                Console.WriteLine("Serverlist:");
                foreach (KeyValuePair<TcpClient, ConnectedServer> server in connectedServers) {
                    Console.WriteLine($"{server.Value.Name}, Players: {server.Value.PlayerCount}/{server.Value.MaxPlayers}, Ping: {server.Value.Ping}ms");
                }
            } else {
                Console.WriteLine("[SERVER OFFLINE]");
            }

            Console.WriteLine();
            Console.WriteLine(text);
        }
    }

    public static class Extensions
    {
        /// <summary>
        /// Returns just specified part of a byte array.
        /// </summary>
        /// <param name="data">The original byte array.</param>
        /// <param name="index">The index to start at.</param>
        /// <param name="length">How many elements should be copied into the sub array.</param>
        /// <returns>The specified part of the array.</returns>
        public static byte[] SubArray(this byte[] data, int index, int length) {
            byte[] result = new byte[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        /// <summary>
        /// Expands (or shortens) a string to the specified length.
        /// </summary>
        /// <param name="original">The string to expand / shorten.</param>
        /// <param name="length">The length the new string should be.</param>
        /// <returns>A new string at the exact length specified.</returns>
        public static string ExpandTo(this string original, int length) {
            if (length <= original.Length) return original.Substring(0, length);

            while (original.Length <= length) {
                original += " ";
            }

            return original;
        }
    }
}
