﻿using MonoNet.LevelManager;
using MonoNet.Util;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MonoNet.Network.UDP
{
    public class Client
    {
        private UdpClient connection;
        private Thread listenThread;
        private IPEndPoint endPoint;

        private bool exitRequested;
        public bool Connected { get; private set; }
        private string name;
        public readonly byte[] welcomeMessage = Encoding.ASCII.GetBytes("Hello!");

        public Client(IPEndPoint endPoint, string name)
        {
            connection = new UdpClient(AddressFamily.InterNetwork);
            this.endPoint = endPoint;
            this.name = name;
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
        /// Interrupts the listing thread and closes the UdpClient.
        /// </summary>
        public void Stop()
        {
            Connected = false;
            exitRequested = true;
            listenThread.Interrupt();
            connection.Close();
        }

        /// <summary>
        /// Tries to connect to the server and listens for any messages.
        /// </summary>
        private void Listen()
        {
            Console.WriteLine("Connecting");

            // ------------------------------------------------------------------------------------------------------------
            // Taken from https://stackoverflow.com/questions/10332630/connection-reset-on-receiving-packet-in-udp-server
            // ------------------------------------------------------------------------------------------------------------
            // This ignores connection resets as errors. If this is not set and a client crashes then this throws exception
            // on the server. This is why the code beneath is there. I am not sure if could have just catched the error and
            // ignored it, but I feel like this is the most efficient way of doing it. Eventhough I don't really know why
            // the socket needs to be configured like that.
            const int SIO_UDP_CONNRESET = -1744830452;
            byte[] inValue = new byte[] { 0 };
            byte[] outValue = new byte[] { 0 };
            connection.Client.IOControl(SIO_UDP_CONNRESET, inValue, outValue);
            // ------------------------------------------------------------------------------------------------------------
            connection.Connect(endPoint);

            // Wait to see if we get something back. If we do not get anything back
            // then we can assume that the ip adress was wrong or the server is not running
            for (int i = 0; i < 10; i++) // total of 5 seconds = 500ms * 10
            {
                // Send a ping to the server
                Send(Encoding.ASCII.GetBytes("Hello?" + name));

                // if no data is available sleep for 1 second
                if (connection.Available == 0)
                {
                    Thread.Sleep(500);
                    continue;
                }

                // We recieved something. Check to see if it was Hello!. If so then we are connected.
                Connected = Recieve(out byte[] message, true) && Encoding.ASCII.GetString(message).Equals("Hello!");
                break;
            }

            // if connected is false then something went wrong
            if (Connected == false)
            {
                Console.WriteLine("Connection could not be established!");
                ((ClientLevelScreen)LevelScreen.Instance).OnDisconnect("Could not connect to server!\nPress escape to go back to the main menu!");
                return;
            }
        }

        /// <summary>
        /// Help method for recieving messages from the UdpClient.
        /// </summary>
        /// <param name="buffer">The message as out parameter.</param>
        /// <returns>True if the message was read and false if some error occured.</returns>
        public bool Recieve(out byte[] buffer, bool overrideNotConnected = false)
        {
            buffer = null;

            if (Connected == false && overrideNotConnected == false)
                return false;

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                if (connection.Available <= 0)
                    return false;

                // read the mesage
                buffer = connection.Receive(ref endPoint);
            }
            catch (Exception ex) // catch any other exception that might occur
            {
                Log.Error(ex.ToString());
                buffer = null;
                return false;
            }

            return buffer.Length != 0;
        }

        /// <summary>
        /// Sends a string to the server.
        /// </summary>
        /// <param name="data">The message to be send.</param>
        public void Send(byte[] data)
        {
            connection.Send(data, data.Length);
        }

        public bool IsWelcomeMessage(byte[] data)
        {
            if (data.Length < welcomeMessage.Length)
                return false;

            for (int i = 0; i < welcomeMessage.Length; i++)
                if (data[i] != welcomeMessage[i])
                    return false;

            return true;
        }

    }
}
