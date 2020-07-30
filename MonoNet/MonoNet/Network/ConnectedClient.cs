using MonoNet.GameSystems;
using MonoNet.Network.Commands;
using System;
using System.Collections.Generic;
using System.Net;

namespace MonoNet.Network
{
    public class ConnectedClient
    {
        public IPEndPoint ip;

        public TimeSpan lastHeardFrom;
        public byte lastRecievedPackage;
        public bool requestResync = false;
        public string name;
        public byte id;

        public List<NetSyncComponent> controlledComponents = new List<NetSyncComponent>();
        public List<Command> toSendCommands = new List<Command>();

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
    }
}
