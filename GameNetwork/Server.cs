using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using GameNetwork.Models;


namespace GameNetwork
{
    internal class Server
    {
        private MessageReceiver receiver;
        private MessageSender sender;
        private GameStateHandler gameStateHandler;

        private Server() { }

        public Server(string ipAddress, int port)
        {
            sender = new MessageSender(ipAddress, port);
            receiver = new MessageReceiver(ipAddress, port, 1024);

            bool withBots = false;
            gameStateHandler = new GameStateHandler(withBots);
            gameStateHandler.AddBots();
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
            int GameSpeed = 500;
            while (true)
            {
                await Task.Delay(GameSpeed);
                try
                {
                    GameState gameState = gameStateHandler.getGameState();
                    if (gameState.isRunnning)
                    {
                        gameStateHandler.movePlayers();
                    }
                    BroadcastState();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private void BroadcastState()
        {
            GameState gameState = gameStateHandler.getGameState();

            var body = JsonConvert.SerializeObject(gameState);
            var messsage = new BroadcastMessage
            {
                dest = BroadcastMessageDestination.Player,
                body = body
            };
            string json = JsonConvert.SerializeObject(messsage);
            string jsonPretty = JsonConvert.SerializeObject(messsage, Formatting.Indented);

            Console.WriteLine("Current state: {0}", jsonPretty);
            sender.SendMessage(json);
        }

        private void OnReceived(string otherPlayerJson)
        {
            try
            {
                var message = JsonConvert.DeserializeObject<BroadcastMessage>(otherPlayerJson);

                bool isToServer = message.dest == BroadcastMessageDestination.Server;
                if (isToServer == false) return;

                if (message.body == Constants.StartGameCode)
                {
                    gameStateHandler.startGame();
                    return;
                }


                if (message.body == Constants.EndGameCode)
                {
                    gameStateHandler.endGame();
                    return;
                }

                // Parse json into PlayerState obj
                var playerState = JsonConvert.DeserializeObject<PlayerState>(message.body);
                gameStateHandler.updateStates(playerState);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
