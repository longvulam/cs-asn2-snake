using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Web.WebSockets;
using Newtonsoft.Json;
using WebsocketServer.Models;
//https://itq.eu/net-4-5-websocket-client-without-a-browser/


namespace WebsocketServer
{
    public class SnakeGameWebSocketHandler : WebSocketHandler
    {
        private static WebSocketCollection clients = new WebSocketCollection();

        private string playerId;
        private GameStateHandler gameLogic;
        private int GameSpeed = 500;

        public SnakeGameWebSocketHandler()
        {
            gameLogic = new GameStateHandler();
            var broadcastTask = Task.Run(BroadcastGameState);
        }

        private async void BroadcastGameState()
        {
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

                    string json = JsonConvert.SerializeObject(messsage);
                    string jsonPretty = JsonConvert.SerializeObject(messsage, Formatting.Indented);
                    Console.WriteLine("Current state: {0}", jsonPretty);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
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
            gameLogic.removePlayerFromGame(playerId);
        }
    }
}