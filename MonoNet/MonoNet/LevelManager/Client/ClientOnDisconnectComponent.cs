using MonoNet.ECS;
using MonoNet.Interfaces;

namespace MonoNet.LevelManager.Client
{
    public class ClientOnDisconnectComponent : Component, IUpdateable
    {
        public string message;

        public void Update()
        {
            LevelUI.DisplayString(message);
        }
    }
}
