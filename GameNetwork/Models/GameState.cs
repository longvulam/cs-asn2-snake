using System.Collections;
public class GameState
{
    private ArrayList playerStates;
    private Coordinate foodPos;

    public GameState()
    {
        playerStates = new ArrayList();
    }

    public ArrayList getPlayerStates()
    {
        return playerStates;
    }

    public void setPlayerStates(ArrayList playerStates)
    {
        this.playerStates = playerStates;
    }

    public void addPlayerState(PlayerState player)
    {
        playerStates.Add(player);
    }

    public Coordinate getFoodPos()
    {
        return foodPos;
    }

    public void setFoodPos(Coordinate foodPos)
    {
        this.foodPos = foodPos;
    }
}