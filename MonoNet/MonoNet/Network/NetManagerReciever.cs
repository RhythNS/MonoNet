using MonoNet.GameSystems;
using MonoNet.Network.Commands;
using MonoNet.Network.UDP;
using MonoNet.Util;
using System.Collections.Generic;
using System.Net;

namespace MonoNet.Network
{
    /// <summary>
    /// Manages the synchronization between the server and the local game.
    /// </summary>
    public class NetManagerReciever : NetManager
    {
        private float timer = 0;

        public override bool IsServer => false;

        private Client client;

        private readonly List<byte> recievedCommands = new List<byte>();
        private readonly List<byte[]> toSendCommands = new List<byte[]>();
        private readonly CommandPackageManager commandPackageManager = new CommandPackageManager();

        private List<byte> tempData = new List<byte>();
        private byte lastRecievedPackage = 0;

        public NetManagerReciever(IPEndPoint ip, string name) : base()
        {
            client = new Client(ip, name);
            client.StartListen();
        }

        /// <summary>
        /// Check if new data is available and update the current game state with the given information.
        /// </summary>
        public void Recieve()
        {
            // Repeat until every package is recieved and get the newest package
            byte[] data = null;
            bool isNewer = false;
            while (client.Recieve(out byte[] newData) == true)
            {
                if (IsNewerPackage(lastRecievedPackage, newData[0]) == true)
                {
                    lastRecievedPackage = newData[0];
                    isNewer = true;
                    data = newData;
                }
            }

            if (data == null || isNewer == false)
                return;

            // Iterate through the package. The first byte is the package number which we already processed.
            int pointer = 1;

            RecieveRPC(data, ref pointer, recievedCommands, toSendCommands, commandPackageManager);

            while (pointer < data.Length)
            {
                byte address = data[pointer]; // The first byte is the id of the NetSyncComponent.
                if (netSyncComponents[address] == null) // If the component is null then we are missing some information.
                {
                    Log.Error("NetSync tried to access a component with an invalid id (" + address + "). Requesting total sync!");
                    // request total sync;
                    break;
                }
                pointer++; // increase the index of the byte array to show to the data rather then the id of the component.
                netSyncComponents[address].Sync(data, ref pointer); // Let the NetSyncComponent handle the synchronization.
            }
        }

        /// <summary>
        /// Sends information about every component which the user controlls.
        /// </summary>
        public void Send()
        {
            // Check if we sent a package not too long ago.
            timer -= Time.Delta;
            if (timer > 0)
                return;
            timer = NetConstants.CLIENT_SEND_RATE_PER_SECOND;

            tempData.Clear();
            tempData.Add(lastRecievedPackage); // Save the number of the last recieved package first.

            // Handle rpcs
            AppendRPCSend(tempData, recievedCommands, toSendCommands);

            // Get the byte[] of each component.
            for (int i = 0; i < netSyncComponents.Length; i++)
            {
                if (netSyncComponents[i] != null && netSyncComponents[i].playerControlled)
                {
                    netSyncComponents[i].GetSync(tempData);
                }
            }

            // Send the package to the server.
            client.Send(tempData.ToArray());
        }

    }
}
