using System;
using System.Threading;

namespace GameNetwork
{
    class Program
    {
        private const string IpAddress = "230.0.0.1";
        private const int Port = 11000;

        static void handleMessage (string message)
        {
            Console.WriteLine(message);
        }

        static void Main(string[] args)
        {
            MessageReceiver receiver = new MessageReceiver(IpAddress, Port);
            receiver.OnReceivedEvent += handleMessage;

            Thread thread = new Thread(new ThreadStart(receiver.StartListen));
            thread.Start();

            while(!receiver.IsListening) { };

            MessageSender sender = new MessageSender(IpAddress, Port);
            string message = Console.ReadLine();
            sender.SendMessage(message);

            receiver.StopListen();
            Console.ReadKey();
        }
    }
}
