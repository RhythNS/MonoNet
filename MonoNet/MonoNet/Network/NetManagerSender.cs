using MonoNet.GameSystems;
using MonoNet.Network.UDP;
using MonoNet.Util;
using System;
using System.Collections.Generic;

namespace MonoNet.Network
{
    /// <summary>
    /// Manages the synchronization between each player.
    /// </summary>
    public class NetManagerSender : NetManager
    {
        private float timer = 0;

        public override bool IsServer => true;

        private byte currentState;

        private NetState zeroState = new NetState();
        private NetState[] netStates = new NetState[256];

        private List<byte> tempList = new List<byte>();

        private Server server;

        public NetManagerSender(int port) : base()
        {
            for (int i = 0; i < netStates.Length; i++)
                netStates[i] = new NetState();

            server = new Server(port, ConnectedAdresses, Recieve);
            server.StartListen();
        }

        /// <summary>
        /// Updates the current state of the game with local calculations and package recieved from players.
        /// </summary>
        public void UpdateCurrentState()
        {
            currentState++;

            // Create the newest NetState.
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

            // Look if any player has timed out and remove them if necessary.
            TimeSpan currentTime = Time.TotalGameTime;
            for (int i = ConnectedAdresses.Count; i > -1; i--)
            {
                if (currentTime.Subtract(ConnectedAdresses[i].lastHeardFrom).CompareTo(NetConstants.TIMEOUT_TIME) > 0)
                {
                    InvokePlayerDisconnected(ConnectedAdresses[i]);
                    ConnectedAdresses.RemoveAt(i);
                }
            }

            // I am reading and writing to these connectedAdresses in multiple threads.
            // This might cause some weird behaviour or it might be fine. If any
            // weird sync bugs happen then try to synchronize the writing and reading for
            // the lastRecievedData.
            for (int i = 0; i < ConnectedAdresses.Count; i++)
            {
                if (ConnectedAdresses[i].lastHandledPackage == ConnectedAdresses[i].lastRecievedPackage)
                    continue;
                ConnectedAdresses[i].lastHandledPackage = ConnectedAdresses[i].lastRecievedPackage;

                byte[] data = ConnectedAdresses[i].lastRecievedData;
                int pointerAt = 1;

                // Handle Rpcs
                if (RecieveRPC(data, ref pointerAt, ConnectedAdresses[i].recievedCommands, ConnectedAdresses[i].toSendCommands, ConnectedAdresses[i].commandPackageManager) == false)
                {
                    Log.Error("Discarding package...");
                    return;
                }

                while (pointerAt < data.Length)
                {
                    byte address = data[pointerAt];
                    if (netSyncComponents[address] == null)
                    {
                        Log.Warn("Player " + ConnectedAdresses[i].ToString() + " tried to access an already deleted adress.");
                        // maybe full sync?
                        break;
                    }
                    if (ConnectedAdresses[i].controlledComponents.Contains(netSyncComponents[address]) == false)
                    {
                        Log.Warn("Player " + ConnectedAdresses[i].ToString() + " tried to modify an component he is not controlling.");
                        // maybe full sync?
                        break;
                    }

                    pointerAt++;

                    netSyncComponents[address].Sync(data, ref pointerAt);
                }
            }
        }

        /// <summary>
        /// Send updates to each connected client.
        /// </summary>
        public void SendToAll()
        {
            // Check to see if we sent out updates recently.
            timer -= Time.Delta;
            if (timer > 0)
                return;

            timer = NetConstants.SERVER_SEND_RATE_PER_SECOND;

            // Send each client an update package.
            for (int i = ConnectedAdresses.Count - 1; i > -1; i--)
                Send(ConnectedAdresses[i]);
        }

        /// <summary>
        /// Sends an update to a specified client.
        /// </summary>
        /// <param name="connectedClient">The client which is updated.</param>
        public void Send(ConnectedClient connectedClient)
        {
            tempList.Clear();
            tempList.Add(currentState); // save the number of the current package

            // Handle rpcs
            AppendRPCSend(tempList, connectedClient.recievedCommands, connectedClient.toSendCommands);

            // Either send them an entire gamestate or a delta game state depending on wheter they want a complete resync.
            if (connectedClient.requestResync == false)
                netStates[currentState].GetDif(netStates[connectedClient.lastRecievedPackage], tempList);
            else
                netStates[currentState].GetDif(zeroState, tempList);

            // Send the prepared package.
            server.Send(connectedClient, tempList.ToArray());
        }

        /// <summary>
        /// Callback function to recieve a package.
        /// </summary>
        /// <param name="connectedClient">The client who sent the package.</param>
        /// <param name="data">The data of the package.</param>
        public void Recieve(ConnectedClient connectedClient, byte[] data)
        {
            // If the package is older then what the client send previously then ignore it.
            if (IsNewerPackage(connectedClient.lastRecievedPackage, data[0]) == false)
                return;

            // Save the package to the connectedClient and let the main thread handle the synchronization.
            connectedClient.lastRecievedPackage = data[0];
            connectedClient.lastHeardFrom = Time.TotalGameTime;
            connectedClient.lastRecievedData = data;
        }

    }
}
