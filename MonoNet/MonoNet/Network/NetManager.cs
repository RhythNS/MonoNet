using MonoNet.Util;

namespace MonoNet.Network
{
    public abstract class NetManager
    {
        public static NetManager Instance { get; private set; }

        public abstract bool IsServer { get; }

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

        public static void OnIDChanged(NetSyncComponent syncComponent, byte id)
            => Instance.ChangeId(syncComponent, id);

        protected virtual void ChangeId(NetSyncComponent syncComponent, byte id)
        {
            if (netSyncComponents[id] != null)
                Log.Info("Overwriting a netComponent " + netSyncComponents[id].GetType());
            netSyncComponents[id] = syncComponent;
        }

        protected byte GetNewerPackageNumber(byte oldNumber, byte newNumber)
            => (oldNumber - newNumber < 128 && newNumber > oldNumber) || (oldNumber - newNumber > 128 && newNumber < oldNumber)
                ? newNumber : oldNumber;
    }
}
