﻿using MonoNet.Network.Commands;
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

        public NetManager()
        {
            Instance = this;
        }

        public void InvokePlayerConnected(ConnectedClient client) => OnPlayerConnected?.Invoke(client);

        public void InvokePlayerDisconnected(ConnectedClient client) => OnPlayerDisconnected?.Invoke(client);

        protected bool IsNewerPackage(byte oldNumber, byte newNumber)
            => (oldNumber - newNumber < 128 && newNumber > oldNumber) || (oldNumber - newNumber > 128 && newNumber < oldNumber);

        public NetSyncComponent GetNetSyncComponent(byte id) => netSyncComponents[id];

        public void SetNetSyncComponent(NetSyncComponent netSyncComponent, byte id) => netSyncComponents[id] = netSyncComponent;

        /// <summary>
        /// Resets all NetSyncComponents and ConnectedAdresses. Should be called after a level is reset.
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < netSyncComponents.Length; i++)
                netSyncComponents[i] = null;

            for (int i = 0; i < ConnectedAdresses.Count; i++)
                ConnectedAdresses[i].controlledComponents.Clear();
        }

        /// <summary>
        /// Gets a client based on a given id.
        /// </summary>
        /// <param name="id">The id of the client.</param>
        /// <returns>The client. If not found, this returns null.</returns>
        public ConnectedClient GetClient(byte id)
        {
            for (int i = 0; i < ConnectedAdresses.Count; i++)
            {
                if (ConnectedAdresses[i].id == id)
                    return ConnectedAdresses[i];
            }
            return null;
        }

        /// <summary>
        /// Gets an id for a newly connected Client.
        /// </summary>
        /// <param name="id">The new id.</param>
        /// <returns>Wheter it found one or not.</returns>
        public bool TryGetNextAvailableID(out byte id)
        {
            for (int i = 0; i < netSyncComponents.Length; i++)
            {
                if (netSyncComponents[i] == null)
                {
                    id = (byte)i;
                    return true;
                }
            }
            id = default;
            return false;
        }

        /// <summary>
        /// Manages the execution and sending of rpcs..
        /// </summary>
        /// <param name="data">The package as byte[].</param>
        /// <param name="pointerAt">The next byte that needs to be read.</param>
        /// <param name="recievedCommands">The id of the executed rpc is added to this list.</param>
        /// <param name="toSendCommands">Each rpc that the other side needs to execute is saved into this list.</param>
        /// <param name="commandPackageManager">The package manager for knowing which rpcs to execute and which to ignore.</param>
        /// <param name="connectedClient">The connected client, if this is called server side.</param>
        /// <returns>Wheter the package should continue to be read from.</returns>
        protected bool RecieveRPC(byte[] data, ref int pointerAt, List<byte> recievedCommands, List<byte[]> toSendCommands, CommandPackageManager commandPackageManager, ConnectedClient connectedClient = null)
        {
            // First get all rpcs which we do not need to send again
            int numberOfAckRpcs = data[pointerAt++];
            for (int j = 0; j < numberOfAckRpcs; j++)
            {
                if (IsServer == true && connectedClient.hasExited == true)
                    return false;

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

            // If the client has not loaded the level already, then set the string to the eventname for loading a level
            string requireLevelChange = null;
            if (IsServer == true && connectedClient.hasChangedLevel == false)
                requireLevelChange = "LL";

            // Get Rpcs which client wants to execute
            int numberOfOwnRpcs = data[pointerAt++];
            for (int j = 0; j < numberOfOwnRpcs; j++)
            {
                byte idOfRpc = data[pointerAt++];
                recievedCommands.Add(idOfRpc);

                bool shouldExecute = commandPackageManager.RecievedShouldExecute(idOfRpc);

                if (NetSyncComponent.ExecuteEventFromByteArray(data, ref pointerAt, shouldExecute, connectedClient, requireLevelChange) == false)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Adds rpcs to a package that can be send to the server or client.
        /// </summary>
        /// <param name="packageList">The package as byte list.</param>
        /// <param name="recievedCommands">Which rpcs need to be acknowledged.</param>
        /// <param name="toSendCommands">Which rpcs need to be sent.</param>
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
                packageList.AddRange(toSendCommands[i]);
        }
    }
}
