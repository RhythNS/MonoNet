using MonoNet.ECS;
using System.Collections.Generic;

namespace MonoNet.Network
{
    /// <summary>
    /// Marks the actor to be synchronized with the server/ client. This component
    /// looks for any component which have ISyncable implemented.
    /// </summary>
    public class NetSyncComponent : Component
    {
        public bool playerControlled;

        private List<ISyncable> syncables = new List<ISyncable>();
        private byte id;

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
            // Listen to added components to find out if they are ISyncable
            Actor.OnComponentAdded += OnComponentAdded;

            NetManager.OnNetComponentCreated(this);

            // Look for already added components which have ISyncable implemented
            if (Actor.GetAllComponents(out ISyncable[] allComponents))
                syncables.AddRange(allComponents);
        }

        /// <summary>
        /// Callback function for when a new component was added. Looks for any
        /// component that implements ISyncable and adds it to an internal list.
        /// </summary>
        /// <param name="addedComponent">The component that was added to this actor.</param>
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
