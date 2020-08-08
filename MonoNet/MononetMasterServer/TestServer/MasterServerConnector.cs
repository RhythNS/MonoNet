using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TestServer
{
    /// <summary>
    /// 
    /// </summary>
    class MasterServerConnector
    {
        private string name = "TestServer";
        private int maxPlayers = 8;
        //private string masterServerIpAddress = "127.0.0.1";
        private string masterServerIpAddress = "176.57.171.145";
        private int masterServerPort = 1337;
        
        private TcpClient client;
        private NetworkStream stream;

        private Thread listenThread;
        private Thread keepAliveThread;

        private bool isServer = false;

        private enum PacketType : byte
        {
            ListServer = 0,
            UpdateServer = 1,
            RequestList = 2,
            StopListingServer = 3,
            KeepAlive = 4
        }

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

        public void StartServer() {
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
                byteList.AddRange(BitConverter.GetBytes(name.Length));
                byteList.AddRange(Encoding.ASCII.GetBytes(name));
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

        public void StopServer() {
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

                            //foreach (Server sv in servers) {
                            //    WriteToLog(sv.ToString());
                            //}
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

        public void RequestServerList() {
            // only requests from clients that are not connected,count
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

        private void WriteToLog(string text) {
            Console.WriteLine(text + Environment.NewLine);
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
    }
}
