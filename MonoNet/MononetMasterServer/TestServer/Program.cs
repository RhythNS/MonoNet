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
            MasterServerConnector connector = new MasterServerConnector();
            connector.Start();


            while (true) {
                string input = Console.ReadLine();

                switch (input) {
                    case "start":
                        connector.StartServer();
                        break;

                    case "stop":
                        connector.StopServer();
                        break;

                    case "playercount":
                        connector.UpdatePlayerCount(Convert.ToInt32(Console.ReadLine()));
                        break;

                    case "list":
                        connector.RequestServerList();
                        break;
                }
            }

        }
    }
}
