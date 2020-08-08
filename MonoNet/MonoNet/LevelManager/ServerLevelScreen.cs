using MonoNet.Network;

namespace MonoNet.LevelManager
{
    public class ServerLevelScreen : LevelScreen
    {
        private GunSpawnLocations gunSpawns;
        private PlayerSpawnLocations playerSpawns;

        private NetManagerSender sender;
        private ServerConnectionComponent serverConnection;
        private int port;

        public ServerLevelScreen(MonoNet monoNet, int port) : base(monoNet)
        {
            this.port = port;
        }

        public override void Initialize()
        {
            base.Initialize();

            gunSpawns = new GunSpawnLocations();
            tiledBase.OnObjectLoaded += gunSpawns.OnObjectLoaded;

            playerSpawns = new PlayerSpawnLocations();
            tiledBase.OnObjectLoaded += playerSpawns.OnObjectLoaded;

            serverConnection = stage.CreateActor(0).AddComponent<ServerConnectionComponent>();
            serverConnection.Set(playerSpawns, stage);
        }

        public override void LoadContent()
        {
            sender = new NetManagerSender(port);
        }

    }
}
