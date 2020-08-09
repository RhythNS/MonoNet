using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MononetMasterServer
{
    class Program
    {
        private static bool exitRequested = false;

        static void Main(string[] args) {
            MasterServer master = new MasterServer();
            const int port = 1337;

            master.RefreshConsole();

            while (!exitRequested) {
                string input = Console.ReadLine();

                switch (input) {
                    case "start":
                        master.Start(port);
                        break;

                    case "stop":
                        master.Stop();
                        break;

                    case "exit":
                        exitRequested = true;
                        break;

                    default:
                        Console.WriteLine("Unknown Command: " + input);
                        break;
                }
            }
        }
    }
}
