using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoNet.GameSystems;
using MonoNet.Screen;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;

namespace MonoNet.Testing.UI
{
    class UITestScreen : IScreen
    {
        private Desktop desktop;
        private Panel healthBar;

        public void Draw(SpriteBatch gameTime) {
            desktop.Render();
        }

        public void Initialize() {

        }

        public void LoadContent() {
            InitializeUI();
        }

        public void UnloadContent() {

        }

        public void Update(GameTime gameTime) {
            // ui test
            if (Input.KeyDown(Keys.Left))
                healthBar.Width += 5;
            if (Input.KeyDown(Keys.Right))
                healthBar.Width -= 5;
        }

        private void InitializeUI() {
            Panel container = new Panel();

            healthBar = new Panel {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                BorderThickness = new Thickness(5, 5, 5, 5),
                Width = 100,
                Height = 20,
                Background = new SolidBrush(Color.Red)
            };

            container.Widgets.Add(healthBar);

            var grid = new Grid {
                RowSpacing = 8,
                ColumnSpacing = 8
            };

            grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

            var helloWorld = new Label {
                Id = "label",
                Text = "Hello, World!"
            };
            grid.Widgets.Add(helloWorld);


            // ComboBox
            var combo = new ComboBox {
                GridColumn = 1,
                GridRow = 0
            };

            combo.Items.Add(new ListItem("Red", Color.Red));
            combo.Items.Add(new ListItem("Green", Color.Green));
            combo.Items.Add(new ListItem("Blue", Color.Blue));
            grid.Widgets.Add(combo);

            // Button
            var button = new TextButton {
                GridColumn = 0,
                GridRow = 1,
                Text = "Show"
            };

            button.Click += (s, a) => {
                var messageBox = Dialog.CreateMessageBox("Message", "Some message!");
                messageBox.ShowModal(desktop);
            };

            grid.Widgets.Add(button);

            // Spin button
            var spinButton = new SpinButton {
                GridColumn = 1,
                GridRow = 1,
                Width = 100,
                Nullable = true
            };
            grid.Widgets.Add(spinButton);

            container.Widgets.Add(grid);

            // Add it to the desktop
            desktop = new Desktop();
            desktop.Root = container;
        }
    }
}
