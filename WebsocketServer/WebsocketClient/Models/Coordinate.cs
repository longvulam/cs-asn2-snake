namespace WebsocketClient.Models
{
    public class Coordinate
    {
        public static int xMinBoundary = -24;
        public static int xMaxBoundary = 24;
        public static int yMinBoundary = -12;
        public static int yMaxBoundary = 12;

        public int x;
        public int y;

        public Coordinate()
        {

        }
        public Coordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
