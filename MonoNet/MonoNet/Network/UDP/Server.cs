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

                // If the connection is a new one, save it to the end points and send a welcome message.
                ConnectedClient connectedClient = null;
                for (int i = 0; i < connectedAdresses.Count; i++)
                {
                    if (connectedAdresses[i].ip == endPoint)
                        connectedClient = connectedAdresses[i];
                }

                if (connectedClient == null)
                {
                    if (Encoding.ASCII.GetString(buffer).Equals("Hello?") == false)
                        continue;

                    connectedAdresses.Add(connectedClient = new ConnectedClient(endPoint));
                    Send(connectedClient, Encoding.ASCII.GetBytes("Hello!"));
                    continue;
                }

                OnMessageRecieved.Invoke(connectedClient, buffer);
            }
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
