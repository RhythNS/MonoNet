using Microsoft.Xna.Framework;
using MonoNet.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using MonoNet.GameSystems;
using Microsoft.Xna.Framework.Graphics;

namespace MonoNet.Testing.UI
{
    class MainMenu : IScreen
    {
        private Game game;
        private Desktop desktop;

        private Panel mainMenu;

        private Window serverBrowser;
        private Window serverCreation;

        private Dialog dialogQuitGame;

        public MainMenu(Game game) {
            this.game = game;
        }

        public void Draw(SpriteBatch batch) {
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

        }

        private void InitializeUI() {
            // create a new dektop
            desktop = new Desktop();

            // create all menu screens
            CreateMainMenu();
            CreateServerBrowser();
            CreateServerCreation();

            // add mainMenu
            desktop.Root = mainMenu;
        }

        private void CreateMainMenu() {
            mainMenu = new Panel();

            Grid grid = new Grid {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                RowSpacing = 8,
                ColumnSpacing = 8,
                Padding = new Thickness(4)
            };
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

            TextButton startGameBtn = new TextButton {
                Text = "Start Game",
                Width = 128,
                Height = 32,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Right,
                GridColumn = 0,
                GridRow = 0
            };
            startGameBtn.Click += (s, a) => {
                serverBrowser.ShowModal(desktop);
            };
            grid.Widgets.Add(startGameBtn);

            TextButton quitGameBtn = new TextButton {
                Text = "Quit Game",
                Width = 128,
                Height = 32,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Right,
                GridColumn = 0,
                GridRow = 2
            };
            quitGameBtn.Click += (s, a) => {
                dialogQuitGame.ShowModal(desktop);
            };
            grid.Widgets.Add(quitGameBtn);

            dialogQuitGame = Dialog.CreateMessageBox("Quit Game?", "Are you sure you want to quit?");

            dialogQuitGame.Closed += (s, a) =>
            {
                if (!dialogQuitGame.Result) return;

                // Enter or "Ok"
                game.Exit();
            };

            mainMenu.Widgets.Add(grid);
        }

        private void CreateServerBrowser() {
            serverBrowser = new Window {
                Title = "Server Browser"
            };

            Panel panel = new Panel {
                Width = game.Window.ClientBounds.Width,
                Height = game.Window.ClientBounds.Height,
                Background = new SolidBrush(new Color(0f, 0f, 0f, 0.5f))
            };

            VerticalStackPanel verticalStackPanel = new VerticalStackPanel {
                Spacing = 8
            };

            TextButton buttonCreate = new TextButton {
                Text = "Create Server"
            };
            verticalStackPanel.Widgets.Add(buttonCreate);

            buttonCreate.Click += (s, a) => {
                serverCreation.ShowModal(desktop);
            };


            Grid serverList = new Grid {
                RowSpacing = 8,
                ColumnSpacing = 8,
                ShowGridLines = true,
                Padding = new Thickness(4)
            };

            // add row for column names
            serverList.RowsProportions.Add(new Proportion(ProportionType.Auto));
            string[] columnNames = { "Name", "Players", "Private", "Ping" };
            int[] columnWidth = { (int)(game.Window.ClientBounds.Width * 0.77f), 0, 0, 0 };
            for (int i = 0; i < columnNames.Length; i++) {
                serverList.ColumnsProportions.Add(new Proportion(columnWidth[i] != 0 ? ProportionType.Pixels : ProportionType.Auto, columnWidth[i]));
                Label field = new Label {
                    Text = columnNames[i],
                    Background = new SolidBrush(Color.White),
                    TextColor = Color.Black,
                    GridColumn = i,
                    GridRow = 0
                };
                serverList.Widgets.Add(field);
            }

            // test rows
            string[,] testServers = {
                {"Test1", "10/16", "X", "25" },
                {"This one has a really long name attached... does it even fit in this window?????????????????????????????????????????????", "10/16", "X", "25" },
                {"Test3", "2/16", "", "576" },
                {"Test3", "2/16", "", "576" },
                {"Test3", "2/16", "", "576" },
                {"Test3", "2/16", "", "576" },
                {"Test3", "2/16", "", "576" },
                {"Test3", "2/16", "", "576" },
                {"Test3", "2/16", "", "576" },
                {"Test3", "2/16", "", "576" },
                {"Test3", "2/16", "", "576" },
                {"Test3", "2/16", "", "576" },
                {"Test3", "2/16", "", "576" },
                {"Test3", "2/16", "", "576" },
                {"Test3", "2/16", "", "576" },
                {"sdjkgfjhsdgfjhsd", "2/16", "", "576" }
            };
            for (int x = 0; x < testServers.GetLength(0); x++) {
                serverList.RowsProportions.Add(new Proportion(ProportionType.Auto));
                for (int y = 0; y < testServers.GetLength(1); y++) {
                    Label field = new Label {
                        Text = testServers[x, y],
                        GridColumn = y,
                        GridRow = x + 1
                    };
                    serverList.Widgets.Add(field);
                }
            }
            // end test rows

            var scrollView = new ScrollViewer();
            scrollView.Content = serverList;

            verticalStackPanel.Widgets.Add(scrollView);

            panel.Widgets.Add(verticalStackPanel);

            serverBrowser.Content = panel;
        }

        private void CreateServerCreation() {
            serverCreation = new Window {
                Title = "Create Server"
            };

            Panel panel = new Panel {
                Width = game.Window.ClientBounds.Width,
                Height = game.Window.ClientBounds.Height,
                Background = new SolidBrush(new Color(0f, 0f, 0f, 0.5f))
            };

            VerticalStackPanel verticalStackPanel = new VerticalStackPanel {
                Spacing = 8
            };

            Grid serverSettings = new Grid {
                RowSpacing = 8,
                ColumnSpacing = 8,
                ShowGridLines = true,
                Padding = new Thickness(4)
            };
            serverSettings.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            serverSettings.ColumnsProportions.Add(new Proportion(ProportionType.Auto));

            serverSettings.RowsProportions.Add(new Proportion(ProportionType.Auto));
            Label maxPlayersLabel = new Label {
                Text = "Max Players:",
                TextAlign = TextAlign.Center,
                GridColumn = 0,
                GridRow = 0
            };
            serverSettings.Widgets.Add(maxPlayersLabel);
            TextBox maxPlayersInput = new TextBox {
                Text = "4",
                Width = 50,
                GridColumn = 1,
                GridRow = 0
            };
            serverSettings.Widgets.Add(maxPlayersInput);

            serverSettings.RowsProportions.Add(new Proportion(ProportionType.Auto));
            Label passwordLabel = new Label {
                Text = "Password:",
                TextAlign = TextAlign.Center,
                GridColumn = 0,
                GridRow = 1
            };
            serverSettings.Widgets.Add(passwordLabel);
            TextBox passwordTextBox = new TextBox {
                HintText = "Leave empty for no password.",
                GridColumn = 1,
                GridRow = 1
            };
            serverSettings.Widgets.Add(passwordTextBox);

            verticalStackPanel.Widgets.Add(serverSettings);

            HorizontalStackPanel horizontalStackPanel = new HorizontalStackPanel {
                Spacing = 8
            };
            TextButton buttonCreate = new TextButton {
                Text = "Create Server",
                HorizontalAlignment = HorizontalAlignment.Right
            };
            buttonCreate.Click += (s, a) => {
                // start server here
            };
            horizontalStackPanel.Widgets.Add(buttonCreate);
            TextButton buttonCancel = new TextButton {
                Text = "Cancel",
                HorizontalAlignment = HorizontalAlignment.Right
            };
            buttonCancel.Click += (s, a) => {
                serverCreation.Close();
            };
            horizontalStackPanel.Widgets.Add(buttonCancel);

            verticalStackPanel.Widgets.Add(horizontalStackPanel);

            panel.Widgets.Add(verticalStackPanel);

            serverCreation.Content = panel;
        }
    }
}
