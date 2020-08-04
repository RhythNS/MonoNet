using MonoNet.Network.Commands;
using MonoNet.Util;
using System.Collections.Generic;

namespace MonoNet.Network
{
    public delegate void OnPlayerConnected(ConnectedClient client);
    public delegate void OnPlayerDisconnected(ConnectedClient client);

    /// <summary>
    /// Holds information which is both used by the Sender and Reciever.
    /// </summary>
    public abstract class NetManager
    {
        public static NetManager Instance { get; private set; }
        public abstract bool IsServer { get; }
        public List<ConnectedClient> ConnectedAdresses { get; protected set; } = new List<ConnectedClient>();

        public event OnPlayerConnected OnPlayerConnected;
        public event OnPlayerDisconnected OnPlayerDisconnected;

        protected NetSyncComponent[] netSyncComponents = new NetSyncComponent[256]; // id in byte

        private static byte probRemoveCounter = 255;

        public NetManager()
        {
            Instance = this;
        }

        public static void OnNetComponentCreated(NetSyncComponent component)
        {
            Instance.ChangeId(component, ++probRemoveCounter);
        }

        protected void InvokePlayerConnected(ConnectedClient client) => OnPlayerConnected?.Invoke(client);

        protected void InvokePlayerDisconnected(ConnectedClient client) => OnPlayerDisconnected?.Invoke(client);

        public static void OnIDChanged(NetSyncComponent syncComponent, byte id)
            => Instance.ChangeId(syncComponent, id);

        protected virtual void ChangeId(NetSyncComponent syncComponent, byte id)
        {
            if (netSyncComponents[id] != null)
                Log.Info("Overwriting a netComponent " + netSyncComponents[id].GetType());
            netSyncComponents[id] = syncComponent;
        }

        protected bool IsNewerPackage(byte oldNumber, byte newNumber)
            => (oldNumber - newNumber < 128 && newNumber > oldNumber) || (oldNumber - newNumber > 128 && newNumber < oldNumber);

        protected bool RecieveRPC(byte[] data, ref int pointerAt, List<byte> recievedCommands, List<byte[]> toSendCommands, CommandPackageManager commandPackageManager)
        {
            // First get all rpcs which we do not need to send again
            int numberOfAckRpcs = data[pointerAt++];
            for (int j = 0; j < numberOfAckRpcs; j++)
            {
                byte handledRpc = data[pointerAt++];
                for (int k = 0; k < toSendCommands.Count; k++)
                {
                    if (toSendCommands[k][0] == handledRpc)
                    {
                        toSendCommands.RemoveAt(k);
                        break;
                    }
                }
            }

            // Get Rpcs which client wants to execute
            int numberOfOwnRpcs = data[pointerAt++];
            for (int j = 0; j < numberOfOwnRpcs; j++)
            {
                byte idOfRpc = data[pointerAt++];
                recievedCommands.Add(idOfRpc);
                if (commandPackageManager.RecievedShouldExecute(idOfRpc) == true)
                {
                    if (NetSyncComponent.ExecuteEventFromByteArray(data, ref pointerAt) == false)
                        return false;
                }
            }
            return true;
        }

        protected void AppendRPCSend(List<byte> packageList, List<byte> recievedCommands, List<byte[]> toSendCommands)
        {
            // Send which rpc command we have recieved and already managed
            packageList.Add((byte)recievedCommands.Count);
            for (int i = 0; i < recievedCommands.Count; i++)
                packageList.Add(recievedCommands[i]);
            recievedCommands.Clear();

            // Send all rpcs that client has not yet acknowledged
            packageList.Add((byte)toSendCommands.Count);
            for (int i = 0; i < toSendCommands.Count; i++)
                for (int j = 0; j < toSendCommands[i].Length; j++)
                    packageList.Add(toSendCommands[i][j]);

        }
    }
}
