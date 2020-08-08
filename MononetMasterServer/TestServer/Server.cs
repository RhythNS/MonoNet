using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestServer
{
    class Server
    {
        private string name = "TestServer";
        private int maxPlayers = 8;
        private int currentPlayers = 0;
        private string masterServerIpAddress = "127.0.0.1";
        private int masterServerPort = 1337;
        
        private TcpClient client;
        private NetworkStream stream;

        private Thread listenThread;

        private bool isConnected = false;

        private enum PacketType : byte
        {
            ListServer = 0,
            RefreshServer = 1,
            RequestList = 2
        }

        public void StartServer() {
            if (isConnected) {
                WriteToLog("Server is already started");
                return;
            }

            IPAddress ip;
            if (IPAddress.TryParse(masterServerIpAddress, out ip)) {
                try {
                    // create new endpoint to server
                    IPEndPoint remoteEndPoint = new IPEndPoint(ip, masterServerPort);
                    // connect client to server endpoint
                    client = new TcpClient();
                    client.Connect(remoteEndPoint);
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

                    // start thread to listen to the server
                    listenThread = new Thread(ListenForMasterServer);
                    listenThread.Start();

                    isConnected = true;
                } catch (SocketException sex) {
                    WriteToLog(">> Error: Connection could not be established." + Environment.NewLine + sex.ToString() + Environment.NewLine + sex.ErrorCode);
                } catch (Exception ex) {
                    WriteToLog(">> Exception: " + ex.ToString());
                }
            } else {
                WriteToLog(">> Connection failed. IP Address could not be parsed.");
            }
        }

        public void StopServer() {
            if (!isConnected) {
                WriteToLog("Server is already stopped");
                return;
            }

            StopListening();
        }

        private void ListenForMasterServer() {
            while (true) {
                try {
                    // read from server stream
                    stream = client.GetStream();
                    byte[] inStream = new byte[1024];
                    int bytesRead = stream.Read(inStream, 0, 1024);
                    stream.Flush();
                    if (bytesRead == 0) {
                        break;
                    }
                    // get string from stream (MAKE SURE TO USE GetString(byte[] bytes, int index, int count) TO GET A PROPER LENGTH FOR STRING)
                    string returnData = Encoding.ASCII.GetString(inStream, 0, bytesRead);
                    WriteToLog(returnData);
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

            if (client.Connected)
                client.Close();
        }

        public void UpdatePlayerCount(int count) {
            // if client is not connected to the server, don't send the message
            if (!isConnected) return;

            try {
                // create byte array
                List<byte> byteList = new List<byte>();
                byteList.Add((byte)PacketType.RefreshServer);
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
            // if client is not connected to the server, don't send the message
            if (!isConnected) return;

            try {
                // create byte array
                List<byte> byteList = new List<byte>();
                byteList.Add((byte)PacketType.RequestList);
                byteList.AddRange(BitConverter.GetBytes(name.Length));
                byteList.AddRange(Encoding.ASCII.GetBytes(name));
                byteList.AddRange(BitConverter.GetBytes(maxPlayers));

                byte[] outStream = byteList.ToArray();
                stream = client.GetStream();
                // send to server
                stream.Write(outStream, 0, outStream.Length);
                stream.Flush();
            } catch (Exception ex) {
                WriteToLog("Exception: " + ex.ToString());
            }
        }

        private void StopListening() {
            isConnected = false;

            WriteToLog(">> Disconnected.");

            // close the connection to the server if it is open
            if (client.Connected)
                client.Close();

            // end listening Thread if it is running
            if (listenThread != null && listenThread.ThreadState == ThreadState.Running)
                listenThread.Abort();
        }

        private void WriteToLog(string text) {
            Console.WriteLine(text + Environment.NewLine);
        }
    }
}
