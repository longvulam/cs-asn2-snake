using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GameNetwork
{
    internal class Server
    {
        private MessageReceiver receiver;
        private MessageSender sender;

        private Server() { }

        public Server(string ipAddress, int port)
        {
            sender = new MessageSender(ipAddress, port);
            receiver = new MessageReceiver(ipAddress, port);
        }

        public void Start()
        {
            receiver.OnReceivedEvent += OnReceived;
            Thread listenThread = new Thread(new ThreadStart(receiver.StartListen));
            listenThread.Start();

            Thread senderThread = new Thread(new ThreadStart(OnSend));
            senderThread.Start();
        }

        private async void OnSend()
        { 
            int IntervalTime = 1000;
            while (true)
            {
                await Task.Delay(IntervalTime);
                BroadcastState();
            }
        }

        private void BroadcastState()
        {
            try
            {
                // ServerState.getState() -> broadcast to game clients
                var messsage = new BroadcastMessage
                {
                    dest = BroadcastMessageDestination.Player,
                    body = "From server"
                };
                string json = JsonConvert.SerializeObject(messsage);
                sender.SendMessage(json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void OnReceived(string otherPlayerJson)
        {
            try
            {
                var message = JsonConvert.DeserializeObject<BroadcastMessage>(otherPlayerJson);

                bool isToServer = message.dest == BroadcastMessageDestination.Server;
                if (isToServer == false) return;

                Console.WriteLine($"Received: {otherPlayerJson}");

                // ServerLogic.UpdatePlayerState(otherPlayerJson);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
