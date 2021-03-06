﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestServer
{
    public class Server
    {
        public string Name { get; set; }
        public IPAddress Address { get; set; }
        public int Port { get; set; }
        public int CurrentPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public long Ping { get; set; }

        private Thread pingThread;

        public Server(string name, IPAddress address, int port, int currPlayers, int maxPlayers) {
            Name = name;
            Address = address;
            Port = port;
            CurrentPlayers = currPlayers;
            MaxPlayers = maxPlayers;

            pingThread = new Thread(PingServer);
            pingThread.Start();
        }

        private void PingServer() {
            Ping pingSender = new Ping();

            // Create a buffer of 32 bytes of data to be transmitted.
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 120;
            PingReply reply = pingSender.Send(Address, timeout, buffer);
            if (reply.Status == IPStatus.Success) {
                Ping = reply.RoundtripTime;

                Console.WriteLine(this.ToString() + Environment.NewLine);
            }
        }

        public override string ToString() {
            return $"{Name}, Players: {CurrentPlayers}/{MaxPlayers}, Ping: {Ping}ms, Address: {Address}";
        }
    }
}
