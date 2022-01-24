using System.Collections.Generic;

namespace WebsocketServer.Models
{
    public class PlayerState
    {
        public string id;
        public Direction direction;
        public List<Coordinate> coordinates = new List<Coordinate>();

        public PlayerState(string id) : this(id, Direction.None)
        {
        }

        public PlayerState(string id, Direction direction)
        {
            this.id = id;
            this.direction = direction;
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
                        Coordinate c = new Coordinate(GameStateHandler.xMinBoundary + coordinateCounter - i, GameStateHandler.yMaxBoundary - 1);
                        coordinates.Add(c);
                    }
                    direction = Direction.Right;
                    break;
                case 2:
                    // TR corner
                    for (int i = 0; i < 5; ++i)
                    {
                        Coordinate c = new Coordinate(GameStateHandler.xMaxBoundary - coordinateCounter - i, GameStateHandler.yMaxBoundary - 1);
                        coordinates.Add(c);
                    }
                    direction = Direction.Left;
                    break;
                case 3:
                    // BR corner
                    for (int i = 0; i < 5; ++i)
                    {
                        Coordinate c = new Coordinate(GameStateHandler.xMaxBoundary - coordinateCounter - i, GameStateHandler.yMinBoundary + 1);
                        coordinates.Add(c);
                    }
                    direction = Direction.Left;
                    break;
                case 4:
                    // BL corner
                    for (int i = 0; i < 5; ++i)
                    {
                        Coordinate c = new Coordinate(GameStateHandler.xMinBoundary + coordinateCounter - i, GameStateHandler.yMinBoundary + 1);
                        coordinates.Add(c);
                    }
                    direction = Direction.Right;
                    break;
            }
        }
    }
}