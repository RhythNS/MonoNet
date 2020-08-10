using Microsoft.Xna.Framework;
using MonoNet.Network;
using MonoNet.Network.MasterServerConnection;

namespace MonoNet.LevelManager
{
    /// <summary>
    /// Server side implentation of the levelscreen.
    /// </summary>
    public class ServerLevelScreen : LevelScreen
    {
        private GunSpawnLocations gunSpawns;
        private PlayerSpawnLocations playerSpawns;

        private NetManagerSender sender;
        private ServerConnectionComponent serverConnection;
        private int port;

        public string Name { get; private set; }

        public ServerLevelScreen(MonoNet monoNet, string name, int port) : base(monoNet)
        {
            this.port = port;
            Name = name;
        }

        public override void Initialize()
        {
            base.Initialize();

            gunSpawns = new GunSpawnLocations();
            tiledBase.OnObjectLoaded += gunSpawns.OnObjectLoaded;

            playerSpawns = new PlayerSpawnLocations();
            tiledBase.OnObjectLoaded += playerSpawns.OnObjectLoaded;

            sender = new NetManagerSender(port);

            serverConnection = stage.CreateActor(0).AddComponent<ServerConnectionComponent>();
            serverConnection.Set(playerSpawns, Name, stage);

            byte levelId = GameManager.GetRandomLevelNumber();
            LoadLevel(levelId);
            GameManager.currentLevel = levelId;

            stage.CreateActor(0).AddComponent<HostGameEndComponent>();
            stage.CreateActor(0).AddComponent<HostGameStartComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            sender.UpdateCurrentState();
            sender.SendToAll();

            base.Update(gameTime);
        }

        protected override void LoadLevel(byte levelNumber)
        {
            GunSpawnLocations.Instance.ClearLocations();
            playerSpawns.ClearLocations();
            base.LoadLevel(levelNumber);
        }

        protected override void OnGameQuit()
        {
            serverConnection.Shutdown();

            sender.UpdateCurrentState();
            sender.SendToAll();
            sender.Stop();

            MasterServerConnector.Instance.StopListingServer();
        }
    }
}
