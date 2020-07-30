using MonoNet.GameSystems;
using MonoNet.Network.UDP;
using MonoNet.Util;
using System.Collections.Generic;
using System.Net;

namespace MonoNet.Network
{
    public class NetManagerReciever : NetManager
    {
        public static readonly float UPDATES_PER_SECOND = 1 / 60f;
        private float timer = 0;

        public override bool IsServer => false;

        private List<byte> tempData = new List<byte>();
        private byte lastRecievedPackage = 0;
        private Client client;

        public NetManagerReciever(IPEndPoint ip, string name) : base()
        {
            client = new Client(ip, name);
            client.StartListen();
        }

        public void Recieve()
        {
            if (client.Recieve(out byte[] data) == false)
                return;

            if (IsNewerPackage(lastRecievedPackage, data[0]) == false)
                return;

            lastRecievedPackage = data[0];

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
            timer -= Time.Delta;
            if (timer > 0)
                return;

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
