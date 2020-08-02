using MonoNet.Util;

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
    }
}
