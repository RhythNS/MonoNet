﻿using MonoNet.GameSystems;
using MonoNet.Network.Commands;
using System;
using System.Collections.Generic;
using System.Net;

namespace MonoNet.Network
{
    /// <summary>
    /// Representation of a client connected to the server.
    /// </summary>
    public class ConnectedClient
    {
        public IPEndPoint ip;

        public byte[] lastRecievedData;
        public byte lastRecievedPackage;
        public byte lastHandledPackage = 255;

        public CommandPackageManager commandPackageManager = new CommandPackageManager();

        public TimeSpan lastHeardFrom;
        public bool requestResync = false;
        public string name;
        public byte id;

        /// <summary>
        /// A List of the NetSyncComponents that this client handles.
        /// </summary>
        public List<NetSyncComponent> controlledComponents = new List<NetSyncComponent>();

        public List<byte[]> toSendCommands = new List<byte[]>();
        private byte autoIncrementRPCSend = 255;
        public List<byte> recievedCommands = new List<byte>();

        /// <summary>
        /// Wheter the client has loaded the new level.
        /// </summary>
        public bool hasChangedLevel = false;
        /// <summary>
        /// Wheter the client is waiting on a new round.
        /// </summary>
        public bool waiting = false;
        /// <summary>
        /// Wheter the client has exited the game on his side.
        /// </summary>
        public bool hasExited = false;

        public ConnectedClient(IPEndPoint ip, string name, byte id)
        {
            this.ip = ip;
            this.name = name;
            this.id = id;
            requestResync = true;
            lastHeardFrom = Time.TotalGameTime;
        }

        public ConnectedClient(string name, byte id)
        {
            this.name = name;
            this.id = id;
        }

        public void AddRPC(List<byte> rpc)
        {
            rpc.Insert(0, ++autoIncrementRPCSend);
            toSendCommands.Add(rpc.ToArray());
        }

        public override string ToString() => name + " " + id;
    }
}
