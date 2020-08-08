using MonoNet.ECS;
using MonoNet.GameSystems;
using MonoNet.Interfaces;
using MonoNet.Network;

namespace MonoNet.LevelManager
{
    class HostGameStartComponent : Component, IUpdateable
    {
        private NetManager net;

        protected override void OnInitialize()
        {
            net = NetManager.Instance;
        }

        public void Update()
        {
            int playerNotReady = 0;
            for (int i = 0; i < net.ConnectedAdresses.Count; i++)
            {
                if (net.ConnectedAdresses[i].hasChangedLevel == false)
                {
                    playerNotReady++;
                }
            }

            if (playerNotReady <= 0)
            {
                LevelScreen.Instance.UI.DisplayString("All players ready!\nPress F5 to start the game!");

                if (Input.IsKeyDownThisFrame(Microsoft.Xna.Framework.Input.Keys.F5))
                {
                    ServerConnectionComponent.Instance.StartGame();
                    Actor.Stage.DeleteActor(Actor);
                }
            }
            else
            {
                LevelScreen.Instance.UI.DisplayString("Still waiting on " + playerNotReady + " players!");
            }
        }
    }
}
