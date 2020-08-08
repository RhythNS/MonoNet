using Microsoft.Xna.Framework.Graphics;
using MonoNet.Interfaces;
using Myra.Graphics2D.UI;
using System;

namespace MonoNet.LevelManager
{
    public class LevelUI : IDrawable
    {
        private Desktop desktop;

        public LevelUI()
        {
            desktop = new Desktop();
        }

        public void DisplayString(string toDisplay)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            desktop.Render();
        }
    }
}
