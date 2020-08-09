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
using MonoNet.Network.MasterServerConnection;

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

        private Grid serverList = new Grid();

        public MainMenu(Game game) {
            this.game = game;
        }

        public void Draw(GameTime gameTime) {
            desktop.Render();
        }

        public void Initialize() {

        }

        public void LoadContent() {
            InitializeUI();

            MasterServerConnector.Instance.RequestServerList();

            MasterServerConnector.Instance.OnReceivedServerlist += ListServers;
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

            // make a new grid on the bottom right of the game window
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

            // add a start game button
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

            // add a quit game button
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

            // create confirmation dialog for quitting the game
            dialogQuitGame = Dialog.CreateMessageBox("Quit Game?", "Are you sure you want to quit?");
            dialogQuitGame.Closed += (s, a) => {
                if (!dialogQuitGame.Result) return;

                // Enter or "Ok"
                game.Exit();
            };

            // add grid to the main menu
            mainMenu.Widgets.Add(grid);
        }

        private void CreateServerBrowser() {
            // create a new window for the server browser
            serverBrowser = new Window {
                Title = "Server Browser"
            };

            // create a new panel with the whole width and height of the game window
            Panel panel = new Panel {
                Width = game.Window.ClientBounds.Width,
                Height = game.Window.ClientBounds.Height,
                Background = new SolidBrush(new Color(0f, 0f, 0f, 0.5f))
            };

            // new vertical stack panel
            VerticalStackPanel verticalStackPanel = new VerticalStackPanel {
                Spacing = 8
            };

            // new horizontal stack panel
            HorizontalStackPanel horizontalStackPanel = new HorizontalStackPanel {
                Spacing = 8
            };

            // create a button for creating a server
            TextButton buttonCreate = new TextButton {
                Text = "Create Server"
            };
            buttonCreate.Click += (s, a) => {
                serverCreation.ShowModal(desktop);
            };
            horizontalStackPanel.Widgets.Add(buttonCreate);

            // create a button for refreshing the server list
            TextButton buttonRefresh = new TextButton {
                Text = "Refresh List"
            };
            buttonRefresh.Click += (s, a) => {
                // get server list from master server
                MasterServerConnector.Instance.RequestServerList();
            };
            horizontalStackPanel.Widgets.Add(buttonRefresh);

            verticalStackPanel.Widgets.Add(horizontalStackPanel);


            // create the server list
            serverList = new Grid {
                RowSpacing = 8,
                ColumnSpacing = 8,
                ShowGridLines = true,
                Padding = new Thickness(4)
            };

            // add row for column names
            serverList.RowsProportions.Add(new Proportion(ProportionType.Auto));
            string[] columnNames = { "Name", "Players", "Ping" };
            int[] columnWidth = { (int)(game.Window.ClientBounds.Width * 0.77f), 0, 0 };
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

        private void ListServers(List<Server> servers) {
            while (serverList.Widgets.Count > 3) {
                serverList.Widgets.RemoveAt(serverList.Widgets.Count - 1);
            }
            while (serverList.RowsProportions.Count > 1) {
                serverList.RowsProportions.RemoveAt(serverList.RowsProportions.Count - 1);
            }
            
            for (int i = 0; i < servers.Count; i++) {
                serverList.RowsProportions.Add(new Proportion(ProportionType.Auto));

                Label name = new Label {
                    Text = servers[i].Name,
                    GridColumn = 0,
                    GridRow = i + 1
                };
                serverList.Widgets.Add(name);

                Label players = new Label {
                    Text = $"{servers[i].CurrentPlayers}/{servers[i].MaxPlayers}",
                    GridColumn = 1,
                    GridRow = i + 1
                };
                serverList.Widgets.Add(players);

                Label ping = new Label {
                    Text = $"{servers[i].Ping}ms",
                    GridColumn = 2,
                    GridRow = i + 1
                };
                serverList.Widgets.Add(ping);

                servers[i].label = ping;
            }
        }
    }
}
