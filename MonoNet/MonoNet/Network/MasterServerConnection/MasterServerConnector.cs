using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MonoNet.Network.MasterServerConnection
{
    /// <summary>
    /// Provides access to connect to the master server.
    /// </summary>
    public class MasterServerConnector
    {
        public static MasterServerConnector Instance;


        public delegate void ReceivedServerlist(List<Server> servers);

        public event ReceivedServerlist OnReceivedServerlist;

        // if this instance of the game is a server
        private static bool isServer = false;

        // IPAddress of Master Server
        private static string masterServerIpAddress = "176.57.171.145"; // REPLACE THIS
        // Port of the Master Server
        private static int masterServerPort = 1337; // REPLACE THIS

        // tcpClient for connecting to the master server
        private TcpClient client;
        private NetworkStream stream;

        /// <summary>
        /// Thread for listening to the master server
        /// </summary>
        private Thread listenThread;

        /// <summary>
        /// Thread to send a keep alive packet
        /// </summary>
        private Thread keepAliveThread;

        /// <summary>
        /// Initializes the Instance of the master server.
        /// </summary>
        public MasterServerConnector() {
            Instance = this;
        }

        /// <summary>
        /// Initializes the connection to the master server and starts the listen and keepAlive threads.
        /// </summary>
        public void Start() {
            // get the ip;
            if (IPAddress.TryParse(masterServerIpAddress, out IPAddress ip)) {
                // create new endpoint to server
                IPEndPoint remoteEndPoint = new IPEndPoint(ip, masterServerPort);
                // connect client to server endpoint
                client = new TcpClient();
                client.Connect(remoteEndPoint);

                // start thread to listen to the server
                listenThread = new Thread(ListenForMasterServer);
                listenThread.Start();

                // start a thread to keep the connection alive
                keepAliveThread = new Thread(KeepAlive);
                keepAliveThread.Start();
            } else {
                WriteToLog("[ERROR] MasterServer IP Address could not be parsed!");
            }
        }

        /// <summary>
        /// Starts listing the server on the master server list.
        /// </summary>
        /// <param name="serverName">The name of the server.</param>
        /// <param name="maxPlayers">The maximum amount of players in the server.</param>
        public void StartListingServer(int port, string serverName, int maxPlayers) {
            if (isServer) {
                WriteToLog("Server is already started");
                return;
            }

            try {
                // get stream from and to server
                stream = client.GetStream();

                // send a message containing server name length, name and max player count
                List<byte> byteList = new List<byte>();
                byteList.Add((byte)PacketType.ListServer);
                byteList.AddRange(BitConverter.GetBytes(port));
                byteList.AddRange(BitConverter.GetBytes(serverName.Length));
                byteList.AddRange(Encoding.ASCII.GetBytes(serverName));
                byteList.AddRange(BitConverter.GetBytes(maxPlayers));

                byte[] outStream = byteList.ToArray();
                stream.Write(outStream, 0, outStream.Length);
                stream.Flush();

                isServer = true;
            } catch (IOException ioex) {
                WriteToLog("[ERROR] KeepAlive could not be sent: " + ioex.ToString());
            } catch (SocketException sex) {
                WriteToLog("[ERROR] Connection could not be established." + Environment.NewLine + sex.ToString() + Environment.NewLine + sex.ErrorCode);
            } catch (Exception ex) {
                WriteToLog("[ERROR] " + ex.ToString());
            }
        }

        /// <summary>
        /// Stops listing the server on the master server list.
        /// </summary>
        public void StopListingServer() {
            if (!isServer) {
                WriteToLog("Server is already stopped");
                return;
            }

            try {
                // get stream from and to server
                stream = client.GetStream();

                // send a message containing server name length, name and max player count
                List<byte> byteList = new List<byte>();
                byteList.Add((byte)PacketType.StopListingServer);

                byte[] outStream = byteList.ToArray();
                stream.Write(outStream, 0, outStream.Length);
                stream.Flush();

                isServer = false;
            } catch (SocketException sex) {
                WriteToLog(">> Error: Connection could not be established." + Environment.NewLine + sex.ToString() + Environment.NewLine + sex.ErrorCode);
            } catch (Exception ex) {
                WriteToLog(">> Exception: " + ex.ToString());
            }
        }

        /// <summary>
        /// Updates the current player count on the master server list.
        /// </summary>
        /// <param name="count">The new player count.</param>
        public void UpdatePlayerCount(int count) {
            // if this is no server return
            if (!isServer) return;

            try {
                // create byte array
                List<byte> byteList = new List<byte>();
                byteList.Add((byte)PacketType.UpdateServer);
                byteList.AddRange(BitConverter.GetBytes(count));

                byte[] outStream = byteList.ToArray();
                stream = client.GetStream();
                // send to server
                stream.Write(outStream, 0, outStream.Length);
                stream.Flush();
            } catch (Exception ex) {
                WriteToLog("Exception: " + ex.ToString());
            }
        }

        /// <summary>
        /// Requests the server list from the master server
        /// </summary>
        public void RequestServerList() {
            if (isServer) return;

            try {
                // create byte array
                List<byte> byteList = new List<byte>();
                byteList.Add((byte)PacketType.RequestList);

                byte[] outStream = byteList.ToArray();
                stream = client.GetStream();
                // send to server
                stream.Write(outStream, 0, outStream.Length);

                stream.Flush();
            } catch (Exception ex) {
                WriteToLog("Exception: " + ex.ToString());
            }
        }

        /// <summary>
        /// Sends a KeepAlive Packet to the server to not get a TimeOut
        /// </summary>
        private void KeepAlive() {
            while (true) {
                // wait 10s
                Thread.Sleep(10000);

                try {
                    // get stream from and to server
                    stream = client.GetStream();

                    // send a keepAlive to the server
                    byte[] outStream = { (byte)PacketType.KeepAlive };
                    stream.Write(outStream, 0, outStream.Length);
                    stream.Flush();
                } catch (IOException ioex) {
                    WriteToLog("[ERROR] KeepAlive could not be sent: " + ioex.ToString());
                } catch (Exception ex) {
                    WriteToLog("[ERROR] " + ex.ToString());
                }
            }
        }

        /// <summary>
        /// Listening for any data from the master server.
        /// </summary>
        private void ListenForMasterServer() {
            while (true) {
                try {
                    // read from server stream
                    stream = client.GetStream();
                    byte[] inStream = new byte[4096];
                    int bytesRead = stream.Read(inStream, 0, 4096);
                    stream.Flush();
                    if (bytesRead == 0) {
                        // connection broken
                        break;
                    }

                    PacketType type = (PacketType)inStream[0];

                    switch (type) {
                        case PacketType.RequestList:
                            // get server count
                            int serverCount = BitConverter.ToInt32(inStream.SubArray(1, 4), 0);
                            List<Server> servers = new List<Server>();
                            int pointer = 5;
                            for (int i = 0; i < serverCount; i++) {
                                // get server name length
                                int nameLength = BitConverter.ToInt32(inStream.SubArray(pointer, 4), 0);
                                pointer += 4;

                                // get server name
                                string serverName = Encoding.ASCII.GetString(inStream.SubArray(pointer, nameLength));
                                pointer += nameLength;

                                // get ip length
                                int ipLength = BitConverter.ToInt32(inStream.SubArray(pointer, 4), 0);
                                pointer += 4;

                                // get ip address
                                IPAddress address = IPAddress.Parse(Encoding.ASCII.GetString(inStream.SubArray(pointer, ipLength)));
                                pointer += ipLength;

                                // get server port
                                int port = BitConverter.ToInt32(inStream.SubArray(pointer, 4), 0);
                                pointer += 4;

                                // get current players
                                int currPlayers = BitConverter.ToInt32(inStream.SubArray(pointer, 4), 0);
                                pointer += 4;

                                // get max players
                                int maxPlayers = BitConverter.ToInt32(inStream.SubArray(pointer, 4), 0);
                                pointer += 4;

                                Server sv = new Server(serverName, address, port, currPlayers, maxPlayers);
                                servers.Add(sv);
                            }

                            this.OnReceivedServerlist?.Invoke(servers);
                            break;

                        case PacketType.KeepAlive:
                            // do nothing
                            break;
                    }
                } catch (ThreadAbortException taex) {
                    // thread was closed
                    break;
                } catch (SocketException sex) {
                    // socket no longer available
                    break;
                } catch (Exception ex) {
                    // print any other exception
                    WriteToLog("Exception: " + ex.ToString());
                    break;
                }
            }
        }

        /// <summary>
        /// Outputs text to the log.
        /// </summary>
        /// <param name="text">The text to output.</param>
        private void WriteToLog(string text) {
            Console.WriteLine(text + Environment.NewLine);
        }

        /// <summary>
        /// Collection of all different packettypes.
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
    }
}
