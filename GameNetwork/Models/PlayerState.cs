using System.Collections;
using GameNetwork.Models;

public class PlayerState
{
    public string id;
    public string direction;
    public ArrayList coordinates;

    public PlayerState(string id, string direction)
    {
        this.id = id;
        this.direction = direction;
        coordinates = new ArrayList();
    }

    public string getId()
    {
        return id;
    }

    public string getDirection()
    {
        return direction;
    }

    public void setDirection(Direction direction)
    {
        this.direction = direction.ToString();
    }

    public ArrayList getCoordinates()
    {
        return coordinates;
    }

    public void setCoordinates(ArrayList coordinates)
    {
        this.coordinates = coordinates;
    }

    public void generateInitialPos(int playerNum)
    {
        int coordinateCounter = 6;
        switch (playerNum)
        {
            case 1:
                // TL corner
                for (int i = 0; i < 5; ++i)
                {
                    Coordinate c = new Coordinate(GameStateHandler.xMinBoundary + coordinateCounter - i, GameStateHandler.yMinBoundary + 1);
                    coordinates.Add(c);
                }
                direction = Direction.Right.ToString();
                break;
            case 2:
                // TR corner
                for (int i = 0; i < 5; ++i)
                {
                    Coordinate c = new Coordinate(GameStateHandler.xMaxBoundary - coordinateCounter - i, GameStateHandler.yMinBoundary + 1);
                    coordinates.Add(c);
                }
                direction = Direction.Left.ToString();
                break;
            case 3:
                // BR corner
                for (int i = 0; i < 5; ++i)
                {
                    Coordinate c = new Coordinate(GameStateHandler.xMaxBoundary - coordinateCounter - i, GameStateHandler.yMaxBoundary - 1);
                    coordinates.Add(c);
                }
                direction = Direction.Left.ToString();
                break;
            case 4:
                // BL corner
                for (int i = 0; i < 5; ++i)
                {
                    Coordinate c = new Coordinate(GameStateHandler.xMinBoundary + coordinateCounter - i, GameStateHandler.yMaxBoundary - 1);
                    coordinates.Add(c);
                }
                direction = Direction.Right.ToString();
                break;
        }
    }
}