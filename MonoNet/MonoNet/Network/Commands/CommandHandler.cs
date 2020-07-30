using System.Collections.Generic;

namespace MonoNet.Network.Commands
{
    public class CommandHandler
    {
        public static CommandHandler Instance { get; private set; }

        public List<Command> commands;
        public GameVariables gameVariables;


        public CommandHandler(GameVariables gameVariables)
        {
            this.gameVariables = gameVariables;
            Instance = this;
        }

        public static void AddCommand(Command command)
        {
            Instance.commands.Add(command);
        }

        public void HandleAll(byte[] data, ref int pointerAt)
        {
            int length = data[pointerAt];
            pointerAt++;
            for (int i = 0; i < length; i++)
            {
                Command command = Command.Parse(data, ref pointerAt);
                command.Execute(gameVariables);
            }
        }
    }
}
