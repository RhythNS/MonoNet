using MonoNet.ECS;
using System.Collections.Generic;

namespace MonoNet.Network
{
    public class NetSyncComponent : Component
    {
        private byte id;

        public bool playerControlled;

        private List<ISyncable> syncables = new List<ISyncable>();

        public byte Id
        {
            get => id;
            set
            {
                NetManager.OnIDChanged(this, value);
                id = value;
            }
        }

        protected override void OnInitialize()
        {
            Actor.OnComponentAdded += OnComponentAdded;

            if (Actor.GetAllComponents(out ISyncable[] allComponents))
                syncables.AddRange(allComponents);
        }

        public void OnComponentAdded(Component addedComponent)
        {
            if (addedComponent is ISyncable syncable)
                syncables.Add(syncable);
        }

        public void Sync(byte[] data, ref int pointerAt)
        {
            for (int i = 0; i < syncables.Count; i++)
                syncables[i].Sync(data, ref pointerAt);
        }

        public void GetSync(List<byte> data)
        {
            for (int i = 0; i < syncables.Count; i++)
                syncables[i].GetSync(data);
        }
    }
}
