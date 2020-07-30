using MonoNet.GameSystems;
using MonoNet.Network.UDP;
using System;
using System.Collections.Generic;

namespace MonoNet.Network
{
    public class NetManagerSender : NetManager
    {
        public static readonly float UPDATES_PER_SECOND = 1 / 30f;
        public static readonly TimeSpan TIMEOUT_TIME = new TimeSpan(0, 0, 8);
        private float timer = 0;

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

            TimeSpan currentTime = Time.TotalGameTime;
            for (int i = connectedAdresses.Count; i > -1; i--)
            {
                if (currentTime.Subtract(connectedAdresses[i].lastHeardFrom).CompareTo(TIMEOUT_TIME) > 0)
                {
                    InvokePlayerDisconnected(connectedAdresses[i]);
                    connectedAdresses.RemoveAt(i);
                }
            }
        }

        public void SendToAll()
        {
            for (int i = connectedAdresses.Count - 1; i > -1; i--)
                Send(connectedAdresses[i]);
        }

        public void Send(ConnectedClient connectedClient)
        {
            timer -= Time.Delta;
            if (timer > 0)
                return;

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
            if (IsNewerPackage(connectedClient.lastRecievedPackage, data[0]) == false)
                return;

            connectedClient.lastRecievedPackage = data[0];
            connectedClient.lastHeardFrom = Time.TotalGameTime;
        }

    }
}
