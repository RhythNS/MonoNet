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
        /// <param name="pointer">Where the next byte should be read from the array.</param>
        /// <param name="shouldExecute">If set to false the method is only parsed and the pointer is advanced.
        /// If set to true, then the parsed method will also be executed.</param>
        public static bool ExecuteEventFromByteArray(byte[] data, ref int pointer, bool shouldExecute)
        {
            // get event name from byte array
<<<<<<< Updated upstream
            /*
            int byteLength = data[pointer++];
            string eventName = Encoding.Unicode.GetString(data.SubArray(pointer, byteLength));
            pointer += byteLength;
            */
            string eventName = NetUtils.GetNextString(data, ref pointer);
=======
            string eventName = Encoding.ASCII.GetString(data.SubArray(pointer, 32));
            eventName = eventName.Trim();
            pointer += 32;
>>>>>>> Stashed changes

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
            if (shouldExecute == true)
                EventHandlerDictionary.Instance[eventName].Invoke(args);
            return true;
        }
    }
}
