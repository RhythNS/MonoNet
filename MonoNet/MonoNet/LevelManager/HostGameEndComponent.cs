using MonoNet.ECS;
using MonoNet.GameSystems;
using MonoNet.Interfaces;
using MonoNet.Network;


namespace MonoNet.LevelManager
{
    class HostGameEndComponent : Component, IUpdateable
    { // TODO: Hook up with gamemanager
        protected override void OnInitialize()
        {
            LevelScreen.Instance.UI.DisplayString("Player <REPLACE> won!\nPress F6 to restart game!"); // TODO: replace
        }

        public void Update()
        {
            if (Input.IsKeyDownThisFrame(Microsoft.Xna.Framework.Input.Keys.F6))
            {
                ServerConnectionComponent.Instance.ChangeLevel(0); // TODO: replace with acctual level id
            }
        }
    }
}
