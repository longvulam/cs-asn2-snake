using System.Collections;
using System;

public class GameState
{
    private ArrayList playerStates;
    private Coordinate foodPos;

    public static int xMinBoundary = -24;
    public static int xMaxBoundary = 24;
    public static int yMinBoundary = -12;
    public static int yMaxBoundary = 12;

    public GameState()
    {
        playerStates = new ArrayList();

        Random rnd = new Random();
        int x = rnd.Next(xMinBoundary, xMaxBoundary);
        int y = rnd.Next(yMinBoundary, yMaxBoundary);
        foodPos = new Coordinate(x, y);
    }

    public ArrayList getPlayerStates()
    {
        return playerStates;
    }

    public void addPlayerState(PlayerState player)
    {
        playerStates.Add(player);
    }

    /*
    public void updatePlayerState(string id)
    {
        foreach (PlayerState player in playerStates)
        {
            if (player.getId().Equals(id))
            {
                
                break;
            }
        }
    }
    */

    public Coordinate getFoodPos()
    {
        return foodPos;
    }

    public void setFoodPos(Coordinate foodPos)
    {
        this.foodPos = foodPos;
    }
}