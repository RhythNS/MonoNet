using MonoNet.ECS;
using MonoNet.Interfaces;

namespace MonoNet.LevelManager.Client
{
    /// <summary>
    /// Helper component when a client disconnects for whatever reason.
    /// </summary>
    public class ClientOnDisconnectComponent : Component, IUpdateable
    {
        public string message;

        public void Update()
        {
            LevelUI.DisplayString(message);
        }
    }
}
