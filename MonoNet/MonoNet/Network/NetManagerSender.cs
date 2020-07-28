using MonoNet.Network.UDP;
using System.Collections.Generic;

namespace MonoNet.Network
{
    public class NetManagerSender : NetManager
    {
        public override bool IsServer => true;

        private byte currentState;

        private NetState zeroState = new NetState();
        private NetState[] netStates = new NetState[256];

        private List<byte> tempList = new List<byte>();
        private readonly List<ConnectedClient> connectedAdresses = new List<ConnectedClient>();

        private Server server;

        public NetManagerSender(int port) : base()
        {
            for (int i = 0; i < netStates.Length; i++)
                netStates[i] = new NetState();

            server = new Server(port, connectedAdresses, Recieve);
            server.StartListen();
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

        public void SendToAll()
        {
            for (int i = connectedAdresses.Count - 1; i > -1; i--)
                Send(connectedAdresses[i]);
        }

        public void Send(ConnectedClient connectedClient)
        {
            tempList.Clear();
            tempList.Add(currentState);

            if (connectedClient.requestResync == false)
                netStates[currentState].GetDif(netStates[connectedClient.lastRecievedPackage], tempList);
            else
                netStates[currentState].GetDif(zeroState, tempList);

            server.Send(connectedClient, tempList.ToArray());
        }

        public void Recieve(ConnectedClient connectedClient, byte[] data)
        {
            connectedClient.lastRecievedPackage = GetNewerPackageNumber(connectedClient.lastRecievedPackage, data[0]);

        }

    }
}
