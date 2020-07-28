using MonoNet.Network.UDP;
using MonoNet.Util;
using System.Collections.Generic;
using System.Net;

namespace MonoNet.Network
{
    public class NetManagerReciever : NetManager
    {
        public override bool IsServer => false;

        private List<byte> tempData = new List<byte>();
        private byte lastRecievedPackage = 0;
        private Client client;

        public NetManagerReciever(IPEndPoint ip) : base()
        {
            client = new Client(ip);
            client.StartListen();
        }

        public void Recieve()
        {
            if (client.Recieve(out byte[] data) == false)
                return;

            lastRecievedPackage = GetNewerPackageNumber(lastRecievedPackage, data[0]);

            int pointer = 1;
            while (pointer < data.Length)
            {
                byte address = data[pointer];
                if (netSyncComponents[address] == null)
                {
                    Log.Error("NetSync tried to access a component with an invalid id (" + address + "). Requesting total sync!");
                    // request total sync;
                    break;
                }
                pointer++;
                netSyncComponents[address].Sync(data, ref pointer);
            }
        }

        public void Send()
        {
            tempData.Clear();
            tempData.Add(lastRecievedPackage);
            for (int i = 0; i < netSyncComponents.Length; i++)
            {
                if (netSyncComponents[i] != null && netSyncComponents[i].playerControlled)
                {
                    netSyncComponents[i].GetSync(tempData);
                }
            }
            client.Send(tempData.ToArray());
        }

    }
}
