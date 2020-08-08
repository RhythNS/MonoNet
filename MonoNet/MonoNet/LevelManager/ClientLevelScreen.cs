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

            clientConnection = stage.CreateActor(0).AddComponent<ClientConnectionComponent>();
            clientConnection.Set(this);
        }

        public override void LoadContent()
        {
            reciever = new NetManagerReciever(ip, playerName);
        }

        public override void LoadLevel(byte levelNumber)
        {
            base.LoadLevel(levelNumber);
            clientConnection.LevelLoaded();
            UI.DisplayString("Level loaded\nWaiting for host to start!");
        }
    }
}
