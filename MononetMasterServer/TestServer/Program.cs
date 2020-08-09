using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{
    class Program
    {
        static void Main(string[] args) {
            Server server = new Server();

            while (true) {
                string input = Console.ReadLine();

                switch (input) {
                    case "start":
                        server.StartServer();
                        break;

                    case "stop":
                        server.StopServer();
                        break;

                    case "playercount":
                        server.UpdatePlayerCount(Convert.ToInt32(Console.ReadLine()));
                        break;

                    case "list":
                        server.RequestServerList();
                        break;
                }
            }

        }
    }
}
