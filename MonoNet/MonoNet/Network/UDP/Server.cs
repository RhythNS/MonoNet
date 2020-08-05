using MonoNet.Util;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MonoNet.Network.UDP
{
    public delegate void OnMessageRecieved(ConnectedClient connectedClient, byte[] data);

    public class Server
    {
        private UdpClient connection;
        private Thread listenThread;
        private int port;
        private readonly byte[] welcomeMessage = Encoding.ASCII.GetBytes("Hello?");

        private bool exitRequested;

        private readonly List<ConnectedClient> connectedAdresses;

        public event OnMessageRecieved OnMessageRecieved;

        public Server(int port, List<ConnectedClient> connectedAdresses, OnMessageRecieved onMessageRecieved)
        {
            connection = new UdpClient(AddressFamily.InterNetworkV6);
            OnMessageRecieved += onMessageRecieved;
            this.port = port;
            this.connectedAdresses = connectedAdresses;
        }

        /// <summary>
        /// Starts the main listing thread.
        /// </summary>
        public void StartListen()
        {
            listenThread = new Thread(Listen);
            listenThread.Start();
        }

        /// <summary>
        /// Interrupts the listing thread and shuts down the server.
        /// </summary>
        public void Stop()
        {
            exitRequested = true;
            listenThread.Interrupt();
            connection.Close();
        }

        /// <summary>
        /// Listens to incoming messages and handles them.
        /// </summary>
        private void Listen()
        {
            Log.Info("Now listing");

            // Start listing for messages
            connection.Client.Bind(new IPEndPoint(IPAddress.IPv6Any, port));

            while (exitRequested == false)
            {
                // Listen for any messages.
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] buffer;
                try
                {
                    // Recieve the message.
                    buffer = connection.Receive(ref endPoint);
                }
                catch (SocketException se) // Anything went wrong with the socket.
                {
                    // Interrupts are okay. So if that did not occure print the error message
                    if (se.SocketErrorCode != SocketError.Interrupted)
                        Console.WriteLine(se.ToString());
                    break;
                }
                catch (Exception ex) // catch any other exception that might occur
                {
                    Console.WriteLine(ex.ToString());
                    break;
                }

                if (buffer == null || buffer.Length == 0)
                    continue;

                // If the connection is a new one, save it to the end points and send a welcome message.
                ConnectedClient connectedClient = null;
                for (int i = 0; i < connectedAdresses.Count; i++)
                {
                    if (connectedAdresses[i].ip == endPoint)
                        connectedClient = connectedAdresses[i];
                }

                bool isWelcomeMessage = IsWelcomeMessage(buffer);

                if (isWelcomeMessage == true && connectedClient != null)
                    continue;

                if (connectedClient == null)
                {
                    string response = Encoding.ASCII.GetString(buffer);
                    if (response.Length < 6 || connectedAdresses.Count >= NetConstants.MAX_PLAYERS)
                        continue;

                    string name = response.Substring(6, response.Length - NetConstants.MAX_NAME_LENGTH - 6 > 0
                        ? NetConstants.MAX_NAME_LENGTH : response.Length - 6);

                    byte id = NetUtils.GetLowestAvailableId(connectedAdresses);

                    connectedAdresses.Add(connectedClient = new ConnectedClient(endPoint, name, id));
                    Send(connectedClient, Encoding.ASCII.GetBytes("Hello!"));
                    NetManager.Instance.InvokePlayerConnected(connectedClient);
                    continue;
                }

                OnMessageRecieved.Invoke(connectedClient, buffer);
            }
        }

        /// <summary>
        /// Checks if a message is a welcome message.
        /// </summary>
        /// <param name="data">The recieved package as byte array.</param>
        /// <returns>Wheter it is a welcome message or not.</returns>
        private bool IsWelcomeMessage(byte[] data)
        {
            if (data.Length < welcomeMessage.Length)
                return false;

            for (int i = 0; i < welcomeMessage.Length; i++)
                if (data[i] != welcomeMessage[i])
                    return false;

            return true;
        }

        /// <summary>
        /// Sends a message to all connected clients.
        /// </summary>
        /// <param name="data">The message to be sent.</param>
        public void SendAll(byte[] data)
        {
            for (int i = 0; i < connectedAdresses.Count; i++)
                Send(connectedAdresses[i], data);
        }

        /// <summary>
        /// Sends a message to a single endPoint.
        /// </summary>
        /// <param name="endPoint">The endpoint recipient.</param>
        /// <param name="data">The message to be sent.</param>
        public void Send(ConnectedClient endPoint, byte[] data)
        {
            connection.Send(data, data.Length, endPoint.ip);
        }
    }
}
