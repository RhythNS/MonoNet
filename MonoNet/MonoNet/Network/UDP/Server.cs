using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MonoNet.Network.UDP
{
    public class Server
    {
        private UdpClient connection;
        private Thread listenThread;
        private int port;

        private bool exitRequested;

        private readonly List<IPEndPoint> connectedAdresses = new List<IPEndPoint>();

        public Server(int port)
        {
            connection = new UdpClient(AddressFamily.InterNetworkV6);
            this.port = port;
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
            Console.WriteLine("Now listing");

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

                // decode the message
                string message = Encoding.ASCII.GetString(buffer);

                // If the connection is a new one, save it to the end points and send a welcome message.
                if (connectedAdresses.Contains(endPoint) == false)
                {
                    connectedAdresses.Add(endPoint);
                    Send(endPoint, "Hello!");
                    continue;
                }

                // Iterate through each connectedAdress and send them the message.
                for (int i = 0; i < connectedAdresses.Count; i++)
                {
                    // if the adress is not the sender
                    if (connectedAdresses[i].Equals(endPoint) == false)
                        Send(connectedAdresses[i], message);
                }
            }
        }

        /// <summary>
        /// Sends a message to all connected clients.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public void SendAll(string message)
        {
            for (int i = 0; i < connectedAdresses.Count; i++)
                Send(connectedAdresses[i], message);
        }

        /// <summary>
        /// Sends a message to a single endPoint.
        /// </summary>
        /// <param name="endPoint">The endpoint recipient.</param>
        /// <param name="message">The message to be sent.</param>
        public void Send(IPEndPoint endPoint, string message)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            connection.Send(buffer, buffer.Length, endPoint);
        }
    }
}
