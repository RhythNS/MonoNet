using Microsoft.Xna.Framework.Input;
using MonoNet.ECS;
using MonoNet.GameSystems;
using MonoNet.Util;

namespace MonoNet.Testing
{
    public class InputTesterComponent : Component, IUpdateable
    {
        public void Update()
        {
            if (Input.IsKeyDownThisFrame(Keys.E))
                    Log.Info("E was just pressed!");
            if (Input.IsKeyUpThisFrame(Keys.E))
                Log.Info("E was just released!");

            if (Input.KeyDown(Keys.F))
                Log.Info("F is pressed!");
        }
    }
}
