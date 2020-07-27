using System;
using System.Net;

namespace MonoNet.Network.UDP
{
    class OtherProgram
    {
        public static void Test(string[] args)
        {
            string adress = "0:0:0:0:0:0:0:1";
            Console.Write("Enter IP adress (nothing for default): ");
            string input = Console.ReadLine();
            if (string.IsNullOrEmpty(input) == false)
                adress = input;

            int port = 25565;
            Console.Write("Enter port (nothing for default): ");
            input = Console.ReadLine();
            if (string.IsNullOrEmpty(input) == false)
                port = int.Parse(input);

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(adress), port);

            Console.Write("1:Server\n2:Client\nEnterType: ");
            int sel = int.Parse(Console.ReadLine());

            switch (sel)
            {
                case 1:
                    Server server = new Server(port);
                    server.StartListen();

                    Console.WriteLine("Server started. Press enter to stop.");
                    Console.ReadLine();

                    server.Stop();
                    break;
                case 2:
                    Client client = new Client(endPoint);
                    client.StartListen();

                    Console.WriteLine("Connected. Enter stop to exit");
                    while (true)
                    {
                        input = Console.ReadLine();
                        if (input.Equals("exit", StringComparison.CurrentCultureIgnoreCase))
                            break;

                        client.Send(input);
                    }

                    client.Stop();
                    break;
            }

            Console.WriteLine("Exiting");
        }
    }
}
