using WebsocketClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebsocketServer
{
    internal class GameStateHandler
    {
        private GameState gameState;
        private bool withBots;

        public GameStateHandler(bool withBots = false)
        {
            gameState = new GameState();
            this.withBots = withBots;
        }

        public GameState getGameState()
        {
            return gameState;
        }

        public void startGame()
        {
            // Sets spawn points in order of TL, TR, BR, BL based on player #
            int playerNum = 1;
            AddBots();
            foreach (PlayerState player in gameState.playerStates)
            {
                player.generateInitialPos(playerNum);
                ++playerNum;
            }
            // Initialized foodPos
            newFoodPos(gameState.playerStates);
            gameState.isRunnning = true;
        }

        internal void addPlayerToGame(string playerId)
        {
            bool canAddPlayer = gameState.isRunnning == false && gameState.playerStates.Count < 4;
            if (canAddPlayer == false) return;

            PlayerState playerState = gameState.playerStates.FirstOrDefault(ps => ps.id == playerId);
            bool isUnknownPlayer = playerState == null;
            if (isUnknownPlayer == false) return;

            gameState.addPlayerState(new PlayerState(playerId));
        }

        internal void removePlayerFromGame(string playerId)
        {
            List<PlayerState> playerStates = gameState.playerStates;
            PlayerState player = playerStates.Find(ps => ps.id == playerId);
            if (player == null) return;

            playerStates.Remove(player);
        }

        public void endGame()
        {
            ResetGame();
        }

        public void iterateGame()
        {
            // Move all players 1 square based on direction
            foreach (PlayerState player in gameState.playerStates)
            {
                player.coordinates = insertNewCoord(player);
                player.coordinates.RemoveAt(player.coordinates.Count - 1);
            }

            // Copy players to new arraylist
            List<PlayerState> remainingPlayers = gameState.playerStates.ToList();

            bool AteFood = false;
            foreach (PlayerState player in gameState.playerStates)
            {
                if (!remainingPlayers.Contains(player)) continue;
                Coordinate head = player.coordinates.First();
                int playerX = head.x;
                int playerY = head.y;

                // Check if player is hitting the border
                if (playerX == Coordinate.xMinBoundary || 
                    playerX == Coordinate.xMaxBoundary || 
                    playerY == Coordinate.yMinBoundary || 
                    playerY == Coordinate.yMaxBoundary)
                {
                    remainingPlayers.Remove(player);
                    continue;
                }

                // Check if player is hitting another player
                //bool collideWithPlayer = false;
                //foreach (PlayerState p in gameState.playerStates)
                //{
                //    if (!remainingPlayers.Contains(player) || player.id == p.id) continue;
                //    foreach (Coordinate coord in p.coordinates)
                //    {
                //        if (playerX == coord.x && playerY == coord.y)
                //        {
                //            remainingPlayers.Remove(player);
                //            // Check if head on head collision (if yes, remove other player as well)
                //            Coordinate first = p.coordinates.First();
                //            if (playerX == first.x && playerY == first.y)
                //            {
                //                remainingPlayers.Remove(p);
                //            }
                //            collideWithPlayer = true;
                //            break;
                //        }
                //    }
                //    if (collideWithPlayer) break;
                //}
                //if (collideWithPlayer) continue;

                // Check if player is hitting foodPos
                if (playerX == gameState.foodPos.x && playerY == gameState.foodPos.y)
                {
                    player.coordinates = insertNewCoord(player);
                    AteFood = true;
                }
            }

            if (AteFood)
            {
                newFoodPos(remainingPlayers);
            }

            gameState.playerStates = remainingPlayers;
        }

        public void ResetGame()
        {
            gameState = new GameState();
            gameState.isRunnning = false;
        }

        public void AddBots()
        {
            if (withBots == false) return;

            addPlayerToGame("CPU1");
            addPlayerToGame("CPU2");
            addPlayerToGame("CPU3");
        }

        public void updatePlayer(PlayerState newPlayerState)
        {
            // don't update if game is not running
            if (gameState.isRunnning == false) return;

            // Check if new or existing player id
            PlayerState playerState = gameState.playerStates.FirstOrDefault(ps => ps.id == newPlayerState.id);
            bool isUnknownPlayer = playerState == null;
            if (isUnknownPlayer) return;

            // update the player from here
            playerState.direction = newPlayerState.direction;
        }

        private void newFoodPos(List<PlayerState> playerStates)
        {
            int x; int y;
            Random rnd = new Random();
            // Continues to generate new xy until not on conflicting coordinate
            bool conflict;
            do
            {
                conflict = false;
                x = rnd.Next(Coordinate.xMinBoundary + 1, Coordinate.xMaxBoundary);
                y = rnd.Next(Coordinate.yMinBoundary + 1, Coordinate.yMaxBoundary);
                foreach (PlayerState player in playerStates)
                {
                    foreach (Coordinate coord in player.coordinates)
                    {
                        if (coord.x == x && coord.y == y) conflict = true;
                    }
                }
            } while (conflict);
            gameState.setFoodPos(new Coordinate(x, y));
        }

        private List<Coordinate> insertNewCoord(PlayerState player)
        {
            List<Coordinate> coordCopy = new List<Coordinate>();
            Coordinate newCoord = nextCoord(player);
            coordCopy.Add(newCoord);
            foreach (Coordinate oldCoord in player.coordinates)
            {
                coordCopy.Add(oldCoord);
            }
            return coordCopy;
        }

        private Coordinate nextCoord(PlayerState player)
        {
            Coordinate first = player.coordinates[0];

            int playerX = first.x;
            int playerY = first.y;

            switch (player.direction)
            {
                case Direction.Up:
                    return new Coordinate(playerX, playerY + 1);
                case Direction.Right:
                    return new Coordinate(playerX + 1, playerY);
                case Direction.Down:
                    return new Coordinate(playerX, playerY - 1);
                case Direction.Left:
                    return new Coordinate(playerX - 1, playerY);
                default:
                    Console.WriteLine("No player direction matched!");
                    return new Coordinate(playerX, playerY);
            }
        }
    }
}