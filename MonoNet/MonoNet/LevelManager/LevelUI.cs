using Microsoft.Xna.Framework.Graphics;
using MonoNet.Interfaces;
using Myra.Graphics2D.UI;
using System;

namespace MonoNet.LevelManager
{
    public class LevelUI : IDrawable
    {
        private Desktop desktop;
        private Label label;

        public LevelUI()
        {
            desktop = new Desktop();
            label = new Label {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlign = TextAlign.Center
            };

            desktop.Widgets.Add(label);
        }

        public void DisplayString(string toDisplay = "")
        {
            label.Text = toDisplay;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            desktop.Render();
        }
    }
}
