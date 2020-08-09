using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MononetMasterServer
{
    /// <summary>
    /// Provides access to a master server for listing individual connected servers and overall online clients.
    /// </summary>
    public class MasterServer
    {
        /// <summary>
        /// Contains a list with all current online servers and their TcpClients.
        /// </summary>
        private static Dictionary<TcpClient, Server> servers;
     
        /// <summary>
        /// If the server is running.
        /// </summary>
        private bool isRunning = false;

        /// <summary>
        /// Listener for new clients.
        /// </summary>
        private TcpListener tcpListener;

        /// <summary>
        /// Thread for listening for new clients.
        /// </summary>
        private Thread listenThread;

        /// <summary>
        /// Counts all online clients.
        /// </summary>
        private int onlineClients = 0;

        /// <summary>
        /// Starts the master server.
        /// </summary>
        /// <param name="port">The port to connect to the masters server.</param>
        public void Start(int port) {
            // if the server is already running, return
            if (isRunning) {
                RefreshConsole("[INFO] Instance of MasterServer is already running!");
                return;
            }

            isRunning = true;

            // Initialize server list
            servers = new Dictionary<TcpClient, Server>();

            // Initialize the listener and start the listen thread
            tcpListener = new TcpListener(IPAddress.Any, port);
            listenThread = new Thread(ListenForClientConnection);
            listenThread.Start();

            RefreshConsole();
        }

        /// <summary>
        /// Stops the master server.
        /// </summary>
        public void Stop() {
            // if the server is already stopped, return
            if (!isRunning) {
                RefreshConsole("[INFO] Instance of MasterServer is already stopped!");
                return;
            }

            isRunning = false;

            // stop the listener and the thread
            if (tcpListener != null)
                tcpListener.Stop();
            if (listenThread != null)
                listenThread.Interrupt();

            RefreshConsole();
        }

        /// <summary>
        /// Listens for new client connection.
        /// </summary>
        private void ListenForClientConnection() {
            // start the listener
            tcpListener.Start();

            TcpClient client = new TcpClient();

            while (true) {
                try {
                    // accept any new client
                    client = tcpListener.AcceptTcpClient();

                    // add new client to client count
                    onlineClients++;

                    RefreshConsole();

                    // start a new thread for listening to the client
                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientConnection));
                    clientThread.Start(client);
                } catch (SocketException) {
                    // can be ignored (thread stopped and no socket connection)
                    break;
                } catch (Exception ex) {
                    // print any other exception
                    RefreshConsole("[ERROR] " + ex.ToString());
                    break;
                }
            }

            // connection was broken
            // remove client from counter
            onlineClients--;

            // stop listener
            tcpListener.Stop();

            RefreshConsole();
        }

        /// <summary>
        /// Handles the data transfer between client and master server.
        /// </summary>
        /// <param name="cl">The client for the data transfer</param>
        private void HandleClientConnection(Object cl) {
            // get the client from thread parameter
            TcpClient client = (TcpClient)cl;

            // get the stream from the client
            NetworkStream stream = client.GetStream();

            int byteRead;
            while (true) {
                try {
                    // read from the stream
                    byte[] message = new byte[1024];
                    byteRead = stream.Read(message, 0, 1024);
                    stream.Flush();

                    // if nothing is read, break the connection
                    if (byteRead == 0) {
                        break;
                    }

                    // get the packettype from the message
                    PacketType type = (PacketType)message[0];

                    switch (type) {
                        case PacketType.ListServer:
                            // start listing the server in the server list
                            Server server = new Server(client);

                            servers.Add(server.Client, server);

                            int port = BitConverter.ToInt32(message.SubArray(1, 4), 0);
                            int nameLength = BitConverter.ToInt32(message.SubArray(5, 4), 0);
                            string serverName = Encoding.ASCII.GetString(message.SubArray(9, nameLength));
                            int maxPlayers = BitConverter.ToInt32(message.SubArray(9 + nameLength, 4), 0);

                            server.Port = port;
                            server.Name = serverName;
                            server.MaxPlayers = maxPlayers;

                            RefreshConsole($"New Server: {server.Name}");
                            break;

                        case PacketType.UpdateServer:
                            // update the server in the list
                            int currentPlayers = BitConverter.ToInt32(message.SubArray(1, 4), 0);

                            servers[client].PlayerCount = currentPlayers;

                            RefreshConsole($"Update PlayerCount: {servers[client].PlayerCount}");
                            break;

                        case PacketType.RequestList:
                            // get the playerlist from a client request and send it back
                            List<byte> data = new List<byte>();

                            // add packettype
                            data.Add((byte)PacketType.RequestList);

                            // add how many servers there are
                            data.AddRange(BitConverter.GetBytes(servers.Count));

                            foreach (KeyValuePair<TcpClient, Server> sv in servers) {
                                // add servername length
                                data.AddRange(BitConverter.GetBytes(sv.Value.Name.Length));

                                // add servername
                                data.AddRange(Encoding.ASCII.GetBytes(sv.Value.Name.ToCharArray(), 0, sv.Value.Name.Length));

                                // add ip length
                                string ip = ((IPEndPoint)sv.Key.Client.RemoteEndPoint).Address.ToString();
                                data.AddRange(BitConverter.GetBytes(ip.Length));

                                // add ip
                                data.AddRange(Encoding.ASCII.GetBytes(ip.ToCharArray(), 0, ip.Length));

                                // add port
                                data.AddRange(BitConverter.GetBytes(sv.Value.Port));

                                // add current players
                                data.AddRange(BitConverter.GetBytes(sv.Value.PlayerCount));

                                // add max players
                                data.AddRange(BitConverter.GetBytes(sv.Value.MaxPlayers));
                            }

                            // send it
                            byte[] outStream = data.ToArray();
                            stream.Write(outStream, 0, outStream.Length);
                            stream.Flush();
                            break;

                        case PacketType.StopListingServer:
                            // stop listing the server
                            string name = servers[client].Name;

                            servers.Remove(client);

                            RefreshConsole($"Server removed: {name}");
                            break;

                        case PacketType.KeepAlive:
                            // if received a keepAlive packet, send one back
                            List<byte> byteList = new List<byte>();

                            // add packettype
                            byteList.Add((byte)PacketType.KeepAlive);

                            byte[] ka = byteList.ToArray();
                            stream.Write(ka, 0, ka.Length);
                            stream.Flush();
                            break;
                    }
                } catch {
                    // server no longer connected
                    break;
                }
            }

            // remove server from list
            servers.Remove(client);

            // close connection
            client.Close();

            // remove from online clients
            onlineClients--;

            RefreshConsole();
        }

        /// <summary>
        /// Refresh the console.
        /// </summary>
        /// <param name="text">Additional text to put in the refresh.</param>
        public void RefreshConsole(string text = "") {
            Console.Clear();

            Console.WriteLine("[COMMANDS]");
            Console.WriteLine("start - starts the server");
            Console.WriteLine("stop - stops the server");
            Console.WriteLine("exit - stops the server and quits the program");
            Console.WriteLine();

            if (isRunning) {
                Console.WriteLine("[SERVER ONLINE]");
                Console.WriteLine();
                Console.WriteLine($"Players online: {onlineClients}");
                Console.WriteLine();
                Console.WriteLine($"Servers online: {servers.Count}");
                foreach (KeyValuePair<TcpClient, Server> server in servers) {
                    IPAddress addr = ((IPEndPoint)server.Value.Client.Client.RemoteEndPoint).Address;
                    Console.WriteLine($"{server.Value.Name}, Players: {server.Value.PlayerCount}/{server.Value.MaxPlayers}, Address: {addr}");
                }
            } else {
                Console.WriteLine("[SERVER OFFLINE]");
            }

            Console.WriteLine();
            Console.WriteLine("Last Operation: " + text);
            Console.WriteLine();
            Console.Write("Enter Command: ");
        }

        /// <summary>
        /// Collection of all different PacketTypes.
        /// </summary>
        private enum PacketType : byte
        {
            /// <summary>
            /// List the server in the server list
            /// </summary>
            ListServer = 0,

            /// <summary>
            /// Update a server in the server list
            /// </summary>
            UpdateServer = 1,

            /// <summary>
            /// Request the server list
            /// </summary>
            RequestList = 2,

            /// <summary>
            /// Remove the server from the server list
            /// </summary>
            StopListingServer = 3,

            /// <summary>
            /// KeepAlive packet
            /// </summary>
            KeepAlive = 4
        }
    }

    /// <summary>
    /// Contains few useful extensions.
    /// </summary>
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
