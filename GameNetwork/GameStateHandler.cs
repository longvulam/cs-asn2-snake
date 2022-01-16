using System;
using GameNetwork.Models;
using System.Collections;

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
        foreach (PlayerState player in gameState.getPlayerStates())
        {
            if (player.getId().Equals(playerState.getId()))
            {
                newPlayer = false;
                break;
            }
        }

        if (newPlayer)
        {
            if (gameState.getPlayerStates().Count < 4)
            {
                gameState.addPlayerState(playerState);
            }
        } else
        {
            // Move all players 1 square based on direction
            foreach (PlayerState player in gameState.getPlayerStates())
            {
                player.setCoordinates(insertNewCoord(player));
                player.getCoordinates().RemoveAt(player.getCoordinates().Count - 1);
            }

            // Copy players to new arraylist
            ArrayList remainingPlayers = new ArrayList();
            foreach (PlayerState player in gameState.getPlayerStates())
            {
                remainingPlayers.Add(player);
            }

            foreach (PlayerState player in gameState.getPlayerStates())
            {
                if (!remainingPlayers.Contains(player)) continue;
                int playerX = ((Coordinate) player.getCoordinates()[0]).getX();
                int playerY = ((Coordinate) player.getCoordinates()[0]).getY();

                // Check if player is hitting the border
                if (playerX == xMinBoundary || playerX == xMaxBoundary || playerY == yMinBoundary || playerY == yMaxBoundary) {
                    remainingPlayers.Remove(player);
                    continue;
                }

                // Check if player is hitting another player
                bool collideWithPlayer = false;
                foreach (PlayerState p in gameState.getPlayerStates())
                {
                    if (!remainingPlayers.Contains(player) || player.getId() == p.getId()) continue;
                    foreach (Coordinate coord in p.getCoordinates())
                    {
                        if (playerX == coord.getX() && playerY == coord.getY())
                        {
                            remainingPlayers.Remove(player);
                            // Check if head on head collision (if yes, remove other player as well)
                            if (playerX == ((Coordinate) p.getCoordinates()[0]).getX() && playerY == ((Coordinate) p.getCoordinates()[0]).getY())
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
                if (playerX == gameState.getFoodPos().getX() && playerY == gameState.getFoodPos().getY())
                {
                    ((PlayerState) remainingPlayers[remainingPlayers.IndexOf(player)]).setCoordinates(insertNewCoord(player));
                }
            }

            newFoodPos(remainingPlayers);
            gameState.setPlayerStates(remainingPlayers);

            // Reset clean game state if 1 or less players
            if (gameState.getPlayerStates().Count < 2)
            {
                gameState = new GameState();
            }
        }
    }
    public void startGame()
    {
        // Sets spawn points in order of TL, TR, BR, BL based on player #
        int playerNum = 1;
        foreach (PlayerState player in gameState.getPlayerStates())
        {
            player.generateInitialPos(playerNum);
            ++playerNum;
        }
        // Initialized foodPos
        newFoodPos(gameState.getPlayerStates());
    }

    private void newFoodPos(ArrayList playerStates)
    {
        int x;
        int y;
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
                foreach (Coordinate coord in player.getCoordinates())
                {
                    if (coord.getX() == x && coord.getY() == y) conflict = true;
                }
            }
        } while (conflict);
        gameState.setFoodPos(new Coordinate(x, y));
    }

    private ArrayList insertNewCoord(PlayerState player)
    {
        ArrayList coordCopy = new ArrayList();
        Coordinate newCoord = nextCoord(player);
        coordCopy.Add(newCoord);
        foreach (Coordinate oldCoord in player.getCoordinates())
        {
            coordCopy.Add(oldCoord);
        }
        return coordCopy;
    }

    private Coordinate nextCoord(PlayerState player)
    {
        int playerX = ((Coordinate)player.getCoordinates()[0]).getX();
        int playerY = ((Coordinate)player.getCoordinates()[0]).getY();

        switch (player.getDirection())
        {
            case "Up":
                return new Coordinate(playerX, playerY + 1);
            case "Right":
                return new Coordinate(playerX + 1, playerY);
            case "Down":
                return new Coordinate(playerX, playerY - 1);
            case "Left":
                return new Coordinate(playerX - 1, playerY);
            default:
                Console.WriteLine("No player direction matched!");
                return new Coordinate(playerX, playerY);
        }
    }
}