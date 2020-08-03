using MonoNet.GameSystems;
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

        public CommandPackageManager commandPackageManager;

        public TimeSpan lastHeardFrom;
        public bool requestResync = false;
        public string name;
        public byte id;

        public List<NetSyncComponent> controlledComponents = new List<NetSyncComponent>();
        public List<byte[]> toSendCommands = new List<byte[]>();
        public List<byte> recievedCommands = new List<byte>();

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

        public override string ToString() => name + " " + id;
    }
}
