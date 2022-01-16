public class PlayerState
{
    public string id;
    public Coordinate playerPos;
    public string direction;

    public PlayerState(string id, Coordinate playerPos, string direction)
    {
        this.id = id;
        this.playerPos = playerPos;
        this.direction = direction;
    }

    public string getId()
    {
        return id;
    }

    public Coordinate getPlayerPos()
    {
        return playerPos;
    }

    public void setPlayerPos(Coordinate playerPos)
    {
        this.playerPos = playerPos;
    }
}