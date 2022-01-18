using System;
using GameNetwork.Models;
using System.Collections.Generic;

internal class GameStateHandler
{
    private GameState gameState;

    public static int xMinBoundary = -24;
    public static int xMaxBoundary = 24;
    public static int yMinBoundary = -12;
    public static int yMaxBoundary = 12;

    public GameStateHandler()
    {
        gameState = new GameState();
    }

    public GameState getGameState()
    {
        return gameState;
    }

    public void updateStates(PlayerState playerState)
    {
        // Check if new or existing player id
        bool newPlayer = true;
        foreach (PlayerState player in gameState.playerStates)
        {
            if (player.id.Equals(playerState.id))
            {
                newPlayer = false;
                break;
            }
        }

        if (newPlayer)
        {
            if (gameState.playerStates.Count < 4)
            {
                gameState.addPlayerState(playerState);
            }
        }
        else
        {
            // Move all players 1 square based on direction
            foreach (PlayerState player in gameState.playerStates)
            {
                player.coordinates = insertNewCoord(player);
                player.coordinates.RemoveAt(player.coordinates.Count - 1);
            }

            // Copy players to new arraylist
            List<PlayerState> remainingPlayers = new List<PlayerState>();
            foreach (PlayerState player in gameState.playerStates)
            {
                remainingPlayers.Add(player);
            }

            foreach (PlayerState player in gameState.playerStates)
            {
                if (!remainingPlayers.Contains(player)) continue;
                int playerX = ((Coordinate)player.coordinates[0]).x;
                int playerY = ((Coordinate)player.coordinates[0]).y;

                // Check if player is hitting the border
                if (playerX == xMinBoundary || playerX == xMaxBoundary || playerY == yMinBoundary || playerY == yMaxBoundary)
                {
                    remainingPlayers.Remove(player);
                    continue;
                }

                // Check if player is hitting another player
                bool collideWithPlayer = false;
                foreach (PlayerState p in gameState.playerStates)
                {
                    if (!remainingPlayers.Contains(player) || player.id == p.id) continue;
                    foreach (Coordinate coord in p.coordinates)
                    {
                        if (playerX == coord.x && playerY == coord.y)
                        {
                            remainingPlayers.Remove(player);
                            // Check if head on head collision (if yes, remove other player as well)
                            if (playerX == ((Coordinate)p.coordinates[0]).x && playerY == ((Coordinate)p.coordinates[0]).y)
                            {
                                remainingPlayers.Remove(p);
                            }
                            collideWithPlayer = true;
                            break;
                        }
                    }
                    if (collideWithPlayer) break;
                }
                if (collideWithPlayer) continue;

                // Check if player is hitting foodPos
                if (playerX == gameState.foodPos.x && playerY == gameState.foodPos.y)
                {
                    PlayerState playerThatAte = remainingPlayers[remainingPlayers.IndexOf(player)];
                    playerThatAte.coordinates = insertNewCoord(player);
                }
            }

            newFoodPos(remainingPlayers);
            gameState.setPlayerStates(remainingPlayers);

            // Reset clean game state if 1 or less players
            if (gameState.playerStates.Count < 2)
            {
                gameState = new GameState();
            }
        }
    }
    public void startGame()
    {
        // Sets spawn points in order of TL, TR, BR, BL based on player #
        int playerNum = 1;
        foreach (PlayerState player in gameState.playerStates)
        {
            player.generateInitialPos(playerNum);
            ++playerNum;
        }
        // Initialized foodPos
        newFoodPos(gameState.playerStates);
        gameState.isRunnning = true;
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
            x = rnd.Next(xMinBoundary + 1, xMaxBoundary);
            y = rnd.Next(yMinBoundary + 1, yMaxBoundary);
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