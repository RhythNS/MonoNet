using Microsoft.Xna.Framework;
using MonoNet.Network;
using System.Net;

namespace MonoNet.LevelManager
{
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

            reciever = new NetManagerReciever(ip, playerName);

            clientConnection = stage.CreateActor(0).AddComponent<ClientConnectionComponent>();
            clientConnection.Set(this);
        }

        public override void Update(GameTime gameTime)
        {
            reciever.Recieve();
            reciever.Send();

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
    }
}
