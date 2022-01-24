using System;
using System.Threading.Tasks;
using Microsoft.Web.WebSockets;
using Newtonsoft.Json;
using WebsocketClient.Models;
//https://itq.eu/net-4-5-websocket-client-without-a-browser/


namespace WebsocketServer
{
    public class SnakeGameWebSocketHandler : WebSocketHandler
    {
        private static WebSocketCollection clients = new WebSocketCollection();
        private static GameStateHandler gameLogic = new GameStateHandler();
        private static bool serverIsRunning = false;
        private static bool withBots = false;
        private static int GameSpeed = 500;
        private string playerId;

        internal SnakeGameWebSocketHandler()
        {
            if (serverIsRunning == false)
            {
                gameLogic = new GameStateHandler(withBots);
                var broadcastTask = Task.Run(() => BroadcastGameState());
            }
        }

        private async void BroadcastGameState()
        {
            serverIsRunning = true;
            while (true)
            {
                await Task.Delay(GameSpeed);
                try
                {
                    GameState gameState = gameLogic.getGameState();
                    if (gameState.isRunnning)
                    {
                        gameLogic.iterateGame();
                    }

                    var messsage = new BroadcastMessage
                    {
                        dest = BroadcastMessageDestination.Player,
                        body = JsonConvert.SerializeObject(gameState)
                    };

                    string jsonPretty = JsonConvert.SerializeObject(messsage, Formatting.Indented);
                    Console.WriteLine("Current state: {0}", jsonPretty);

                    string json = JsonConvert.SerializeObject(messsage);
                    clients.Broadcast(json);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                if (serverIsRunning == false)
                    break;
            }
        }

        public override void OnOpen()
        {
            // client <- hub -> client <- hub -> server
            playerId = WebSocketContext.QueryString["playerId"];
            clients.Add(this);

            gameLogic.addPlayerToGame(playerId);
        }

        public override void OnMessage(string otherPlayerJson)
        {
            try
            {
                var message = JsonConvert.DeserializeObject<BroadcastMessage>(otherPlayerJson);

                bool isToServer = message.dest == BroadcastMessageDestination.Server;
                if (isToServer == false) return;

                if (message.body == Constants.StartGameCode)
                {
                    gameLogic.startGame();
                    return;
                }

                if (message.body == Constants.EndGameCode)
                {
                    gameLogic.endGame();
                    return;
                }

                // Parse json into PlayerState obj
                var playerState = JsonConvert.DeserializeObject<PlayerState>(message.body);
                gameLogic.updatePlayer(playerState);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnClose()
        { 
            clients.Remove(this);
        }
    }
}