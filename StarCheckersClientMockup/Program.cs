using System;
using System.Net;

namespace StarCheckersClientMockup
{
    class MainClass
    {
        public static void Main()
        {
            Console.WriteLine("Welcome to StarCheckers Turn IRC!");

            Console.WriteLine("Waiting for server connection...");

            try
            {
//                var client = new SyncClient(IPAddress.Parse("192.168.0.11"), 8888);
                var client = new SyncClient(IPAddress.Parse("25.113.123.152"), 8888); // ja windows
//                var client = new SyncClient(IPAddress.Parse("25.122.152.24"), 8888); // dominik
//                var client = new SyncClient(IPAddress.Parse("25.141.205.209"), 8888); // ja linux

                client.StartClient();

                Console.WriteLine("Waiting for pair founding...");
                string response;
                response = client.ReceiveMessage();
                Console.WriteLine("We have found for you!");
                Console.WriteLine("Your color is: " + response + "\n\n");

                if(response == "white")
                {

                }
                else if(response == "black")
                {
                    response = client.ReceiveMessage();
                    Console.WriteLine("Stranger: " + response + "\n");
                }
                else if(response == "any")
                {
                    client.StopClient();
                    return;
                }

                while(response != "end" && response != "player_disconected")
                {
                    Console.Write("You: ");
                    string message = Console.ReadLine();
                    Console.WriteLine();
                    response = client.SendAndReceiveMessage(message);
                    Console.WriteLine("Stranger: " + response + "\n");
                }

                client.StopClient();
                Console.WriteLine("Conversation ended!");
            }
            catch(Exception e)
            {
                Console.WriteLine("Server connection error...");
                Console.WriteLine(e);
            }

//            Console.ReadKey();
        }
    }
}
