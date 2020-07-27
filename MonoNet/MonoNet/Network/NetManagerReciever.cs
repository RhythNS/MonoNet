using MonoNet.Util;
using System.Collections.Generic;

namespace MonoNet.Network
{
    public class NetManagerReciever : NetManager
    {
        private List<byte> tempData = new List<byte>();

        private byte lastRecievedPackage = 0;

        public override bool IsServer => false;

        public NetManagerReciever() : base()
        {
        }


        public void Recieve(byte[] data)
        {
            lastRecievedPackage = GetNewerPackageNumber(lastRecievedPackage, data[0]);

            int pointer = 1;
            while (pointer < data.Length)
            {
                byte address = data[pointer];
                if (netSyncComponents[pointer] == null)
                {
                    Log.Error("NetSync tried to access a component with an invalid id. Requesting total sync!");
                    // request total sync;
                    break;
                }
                netSyncComponents[pointer].Sync(data, ref pointer);
            }
        }

        public byte[] Send()
        {
            tempData.Clear();
            tempData.Add(lastRecievedPackage);
            for (int i = 0; i < netSyncComponents.Length; i++)
            {
                if (netSyncComponents[i].playerControlled)
                {
                    netSyncComponents[i].GetSync(tempData);
                }
            }
            return tempData.ToArray();
        }

    }
}
