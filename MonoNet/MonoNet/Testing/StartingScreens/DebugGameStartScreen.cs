using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoNet.GameSystems;
using MonoNet.LevelManager;
using MonoNet.Screen;
using System.Net;

namespace MonoNet.Testing.StartingScreens
{
    class DebugGameStartScreen : ScreenAdapter
    {
        private MonoNet monoNet;

        public DebugGameStartScreen(MonoNet monoNet)
        {
            this.monoNet = monoNet;
        }

        public override void Draw(SpriteBatch gameTime)
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (Input.KeyDown(Keys.F1))
            {
                monoNet.ScreenManager.SetScreen(new ServerLevelScreen(monoNet, "Unknown", 25565));

            }
            else if (Input.KeyDown(Keys.F2))
            {
                monoNet.ScreenManager.SetScreen(new ClientLevelScreen(monoNet, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 25565), "Unknown"));
            }
        }
    }
}
