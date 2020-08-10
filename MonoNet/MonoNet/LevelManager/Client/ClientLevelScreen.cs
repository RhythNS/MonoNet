using Microsoft.Xna.Framework;
using MonoNet.GameSystems;
using MonoNet.LevelManager.Client;
using MonoNet.Network;
using System.Net;

namespace MonoNet.LevelManager
{
    /// <summary>
    /// Client implentation of the main game screen.
    /// </summary>
    public class ClientLevelScreen : LevelScreen
    {
        private IPEndPoint ip;
        private string playerName;

        private NetManagerReciever reciever;
        private ClientConnectionComponent clientConnection;

        public ClientLevelScreen(MonoNet monoNet, IPEndPoint ip, string playerName) : base(monoNet)
        {
            this.ip = ip;
            this.playerName = playerName;
        }

        public override void Initialize()
        {
            base.Initialize();

            LevelUI.DisplayString("Connecting...");

            reciever = new NetManagerReciever(ip, playerName);

            clientConnection = stage.CreateActor(0).AddComponent<ClientConnectionComponent>();
            clientConnection.Set(this);
        }

        public override void Update(GameTime gameTime)
        {
            reciever.Recieve();

            if (reciever.Connected == true)
            {
                reciever.Send();
                if (Time.TotalGameTime.Subtract(reciever.lastHeardFrom).CompareTo(NetConstants.TIMEOUT_TIME) > 0)
                {
                    OnDisconnect("Connection to server lost!\nPress escape to return to the main menu!");
                    reciever.Stop();
                }
            }


            base.Update(gameTime);
        }

        protected override void LoadLevel(byte levelNumber)
        {
            base.LoadLevel(levelNumber);

            LevelUI.DisplayString("");

            clientConnection.LevelLoaded();
            LevelUI.DisplayString("Level loaded\nWaiting for host to start!");
        }

        public void GameEnd(bool draw, string name)
        {
            if (draw == false)
                LevelUI.DisplayString("Player " + name + " won!\nWaiting for game to restart!");
            else
                LevelUI.DisplayString("Nobody wins!\nWaiting for game to restart");
        }

        /// <summary>
        /// Called when the client disconnects for whatever reason.
        /// </summary>
        /// <param name="message">The reason why the client disconnected.</param>
        public void OnDisconnect(string message)
        {
            stage.CreateActor(0).AddComponent<ClientOnDisconnectComponent>().message = message;
        }

        protected override void OnGameQuit()
        {
            NetSyncComponent.TriggerServerEvent("D", false);
            if (reciever.Connected == true)
            {
                reciever.Recieve();
                reciever.Send(true);
                reciever.Stop();
            }
        }
    }
}
