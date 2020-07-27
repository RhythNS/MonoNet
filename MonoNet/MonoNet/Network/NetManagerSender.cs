using System.Collections.Generic;

namespace MonoNet.Network
{
    public class NetManagerSender : NetManager
    {
        public override bool IsServer => true; 

        private byte currentState;
        private NetState[] netStates = new NetState[256];
        private List<byte> tempList = new List<byte>();

        public NetManagerSender() : base()
        {
            for (int i = 0; i < netStates.Length; i++)
                netStates[i] = new NetState();
        }

        public void UpdateCurrentState()
        {
            currentState++;
            for (int i = 0; i < netSyncComponents.Length; i++)
            {
                byte[] data = null;
                if (netSyncComponents[i] != null)
                {
                    tempList.Clear();
                    netSyncComponents[i].GetSync(tempList);
                    data = tempList.ToArray();
                }
                netStates[currentState].Set(i, data);
            }
        }

        public byte[] Send(ConnectedClient connectedClient, byte newestState)
        {
            tempList.Clear();
            netStates[newestState].GetDif(netStates[connectedClient.lastRecievedPackage], tempList);
            return tempList.ToArray();
        }

        public void Recieve(ConnectedClient connectedClient, byte[] data)
        {
            connectedClient.lastRecievedPackage = GetNewerPackageNumber(connectedClient.lastRecievedPackage, data[0]);

        }

    }
}
