using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MonoNet.Network
{
    public class ConnectedClient
    {
        public IPEndPoint ip;
        public byte lastRecievedPackage;

        public bool requestResync = false;

        public List<NetSyncComponent> controlledComponents = new List<NetSyncComponent>();

        public ConnectedClient(IPEndPoint ip)
        {
            this.ip = ip;
            requestResync = true;
        }
    }
}
