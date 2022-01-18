using System.Collections.Generic;

public class GameState
{
    public List<PlayerState> playerStates; // [{x, y}, ...]
    public Coordinate foodPos;

    public bool isRunnning { get; set; }

    public GameState()
    {
        playerStates = new List<PlayerState>();
    }

    public void setPlayerStates(List<PlayerState> playerStates)
    {
        this.playerStates = playerStates;
    }

    public void addPlayerState(PlayerState player)
    {
        playerStates.Add(player);
    }

    public void setFoodPos(Coordinate foodPos)
    {
        this.foodPos = foodPos;
    }
}