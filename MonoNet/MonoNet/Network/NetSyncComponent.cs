using MonoNet.ECS;
using MonoNet.Network.Commands;
using MonoNet.Util;
using System.Collections.Generic;
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
                id = value;
                NetManager.Instance.SetNetSyncComponent(this, id);
            }
        }

        protected override void OnInitialize()
        {
            // Listen to added components to find out if they are ISyncable
            Actor.OnComponentAdded += OnComponentAdded;
            // Look for already added components which have ISyncable implemented
            if (Actor.GetAllComponents(out ISyncable[] allComponents))
                syncables.AddRange(allComponents);

            if (NetManager.Instance.IsServer == true)
            {
                if (NetManager.Instance.TryGetNextAvailableID(out byte gottenId) == false)
                {
                    Log.Error("Could not get id for NetSynccomponent! CRITICAL ERROR!");
                    return;
                }
                Id = gottenId;
            }
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
        public static void TriggerServerEvent(string eventName, params object[] args)
        {
            if (NetManager.Instance.IsServer) return;

            List<byte> data = EventDataToByteArray(eventName, args);

            ((NetManagerReciever)NetManager.Instance).AddRPC(data);
        }

        /// <summary>
        /// Trigger an event on a specified connected client that should be executed by that client. NEEDS TO BE REGISTERED ON THE CLIENT!
        /// </summary>
        /// <param name="player">The player to sent the RPC to.</param>
        /// <param name="eventName">The name of the event to trigger.</param>
        /// <param name="args">The arguments the event expects.</param>
        public static void TriggerClientEvent(ConnectedClient player, string eventName, params object[] args)
        {
            if (!NetManager.Instance.IsServer) return;

            List<byte> data = EventDataToByteArray(eventName, args);

            player.AddRPC(data);
        }

        /// <summary>
        /// Trigger an event on all connected clients that should be executed by those clients. NEEDS TO BE REGISTERED ON THE CLIENT!
        /// </summary>
        /// <param name="eventName">The name of the event to trigger.</param>
        /// <param name="args">The arguments the event expects.</param>
        public static void TriggerClientEvent(string eventName, params object[] args)
        {
            if (!NetManager.Instance.IsServer) return;

            List<byte> data = EventDataToByteArray(eventName, args);

            for (int i = 0; i < NetManager.Instance.ConnectedAdresses.Count; i++)
                NetManager.Instance.ConnectedAdresses[i].AddRPC(data);
        }

        /// <summary>
        /// Encodes event data into a Byte Array.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="args">The arguments for the event.</param>
        /// <returns>A byte array containing all data.</returns>
        private static List<byte> EventDataToByteArray(string eventName, params object[] args)
        {
            List<byte> data = new List<byte>();

            // encode event name into 32 byte
            /*
            if (eventName.Length > NetConstants.MAX_PREF_STRING_LENGTH)
                Log.Info("Event name is longer than preferred! (" + eventName.Length + "/" + NetConstants.MAX_PREF_STRING_LENGTH + ")");

            byte[] nameBytes = Encoding.Unicode.GetBytes(eventName);
            if (nameBytes.Length > byte.MaxValue + 1)
                Log.Error("Event name is too long!");

            data.Add((byte)nameBytes.Length);
            data.AddRange(nameBytes);
            */
            NetUtils.AddStringToList(eventName, data);

            // add all parameters into the byte array
            foreach (object arg in args)
            {
                if (NetUtils.TryAddValueToList(arg, data) == false)
                {
                    // Error: Could not convert data to byte array
                    Log.Error("Could not contert data to byte array!");
                }
            }

            return data;
        }

        /// <summary>
        /// Executes an event from receiving a byte array.
        /// </summary>
        /// <param name="data">The byte array for the event.</param>
        public static bool ExecuteEventFromByteArray(byte[] data, ref int pointer)
        {
            // get event name from byte array
            /*
            int byteLength = data[pointer++];
            string eventName = Encoding.Unicode.GetString(data.SubArray(pointer, byteLength));
            pointer += byteLength;
            */
            string eventName = NetUtils.GetNextString(data, ref pointer);

            // get method info from any registered callback
            MethodInfo method = EventHandlerDictionary.Instance[eventName].callbacks[0].GetMethodInfo();

            // create array with parameters from byte array
            ParameterInfo[] paras = method.GetParameters();
            object[] args = new object[paras.Length];

            for (int i = 0; i < paras.Length; i++)
            {
                if (NetUtils.TryGetNextValue(data, ref pointer, paras[i].ParameterType, out object parsed) == true)
                {
                    args[i] = parsed;
                }
                else
                {
                    // Error: Could not convert from byte array to a specified type
                    Log.Error("Could not parse event. The whole package should be discarded!");
                    return false;
                }
            }

            // execute event with its arguments
            EventHandlerDictionary.Instance[eventName].Invoke(args);
            return true;
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
