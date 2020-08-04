using MonoNet.ECS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using MonoNet.Network.Commands;
using System.Reflection;

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

        /// <summary>
        /// Trigger an event on the server that should be executed by the server. NEEDS TO BE REGISTERED ON THE SERVER!
        /// </summary>
        /// <param name="eventName">The name of the event to trigger.</param>
        /// <param name="args">The arguments the event expects.</param>
        public static void TriggerServerEvent(string eventName, params object[] args) {
            if (NetManager.Instance.IsServer) return;

            byte[] data = EventDataToByteArray(eventName, args);
            // TODO: send data to server
        }

        /// <summary>
        /// Trigger an event on a specified connected client that should be executed by that client. NEEDS TO BE REGISTERED ON THE CLIENT!
        /// </summary>
        /// <param name="playerId">The id of the player.</param>
        /// <param name="eventName">The name of the event to trigger.</param>
        /// <param name="args">The arguments the event expects.</param>
        public static void TriggerClientEvent(byte playerId, string eventName, params object[] args) {
            if (!NetManager.Instance.IsServer) return;

            byte[] data = EventDataToByteArray(eventName, args);
            // TODO: send data to client playerId
        }

        /// <summary>
        /// Trigger an event on all connected clients that should be executed by those clients. NEEDS TO BE REGISTERED ON THE CLIENT!
        /// </summary>
        /// <param name="eventName">The name of the event to trigger.</param>
        /// <param name="args">The arguments the event expects.</param>
        public static void TriggerClientEvent(string eventName, params object[] args) {
            if (!NetManager.Instance.IsServer) return;

            // TODO: trigger on all connected clients
            //foreach (ConnectedClient client in CONNECTEDCLIENTS)
            //    TriggerClientEvent(client.id, eventName, args);
        }

        /// <summary>
        /// Encodes event data into a Byte Array.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="args">The arguments for the event.</param>
        /// <returns>A byte array containing all data.</returns>
        private static byte[] EventDataToByteArray(string eventName, params object[] args) {
            List<byte> data = new List<byte>();

            // encode event name into 32 byte
            data.AddRange(Encoding.ASCII.GetBytes(eventName.ExpandTo(32).ToCharArray(), 0, 32));

            // add all parameters into the byte array
            foreach (object arg in args) {
                if (arg.GetType() == typeof(int)) {
                    data.AddRange(BitConverter.GetBytes((int)arg));
                } else if (arg.GetType() == typeof(float)) {
                    data.AddRange(BitConverter.GetBytes((float)arg));
                } else {
                    // Error: Could not convert data to byte array
                }
            }

            return data.ToArray();
        }

        /// <summary>
        /// Executes an event from receiving a byte array.
        /// </summary>
        /// <param name="data">The byte array for the event.</param>
        private static void ExecuteEventFromByteArray(byte[] data) {
            // get event name from byte array
            string eventName = Encoding.ASCII.GetString(data.SubArray(0, 32));

            byte[] parameters = data.SubArray(32, data.Length - 32);

            // get method info from any registered callback
            MethodInfo method = EventHandlerDictionary.Instance[eventName].callbacks[0].GetMethodInfo();

            // create array with parameters from byte array
            ParameterInfo[] paras = method.GetParameters();
            object[] args = new object[paras.Length];

            int pointer = 0;
            for (int i = 0; i < paras.Length; i++) {
                if (paras[i].ParameterType == typeof(int)) {
                    args[i] = BitConverter.ToInt32(parameters.SubArray(pointer, 4), 0);

                    pointer += 4;
                } else if (paras[i].ParameterType == typeof(float)) {
                    args[i] = BitConverter.ToSingle(parameters.SubArray(pointer, 4), 0);

                    pointer += 4;
                } else {
                    // Error: Could not convert from byte array to a specified type
                }
            }

            // execute event with its arguments
            EventHandlerDictionary.Instance[eventName].Invoke(args);
        }

        /// How an event can look
        //[EventHandler("AngleObject")]
        //private void Test(int objectId, float angle) {
        //    if (objectId != this.netId) return;
        //
        //    this.angle = angle;
        //}

        /// How to trigger an event
        // Triggers the event with name "TEST" on the client, passing in an int for the objectId and a float for an angle.
        //TriggerClientEvent("AngleObject", 5, 45.343f);
    }
}
