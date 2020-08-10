using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoNet.Interfaces;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using System;

namespace MonoNet.LevelManager
{
    public class LevelUI : Interfaces.IDrawable
    {
        public static LevelUI Instance { get; private set; }

        private Desktop desktop;
        private Label label;

        public LevelUI()
        {
            Instance = this;

            desktop = new Desktop();
            label = new Label {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlign = TextAlign.Center,
                TextColor = Color.White,
                Padding = new Thickness(8),
                Background = new SolidBrush(new Color(30, 30, 30, 150))
            };

            desktop.Widgets.Add(label);
        }

        public static void DisplayString(string toDisplay = "")
        {
            Instance.label.Text = toDisplay;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            desktop.Render();
        }
    }
}
