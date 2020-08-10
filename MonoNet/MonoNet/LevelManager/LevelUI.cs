using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;

namespace MonoNet.LevelManager
{
    /// <summary>
    /// For displaying messages inside the game view.
    /// </summary>
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

        /// <summary>
        /// Displays a string in the middle of the screen.
        /// </summary>
        /// <param name="toDisplay">The message. Empty if the message box should disapear.</param>
        public static void DisplayString(string toDisplay = "")
        {
            if (string.IsNullOrEmpty(toDisplay))
                toDisplay = "";
            Instance.label.Text = toDisplay;
            Instance.label.Visible = toDisplay.Length != 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            desktop.Render();
        }
    }
}
