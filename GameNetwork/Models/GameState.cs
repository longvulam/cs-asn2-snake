using System.Collections.Generic;

namespace GameNetwork.Models
{
    public class GameState
    {
        public List<PlayerState> playerStates; // [{x, y}, ...]
        public Coordinate foodPos;

        public bool isRunnning { get; set; }

        public GameState()
        {
            playerStates = new List<PlayerState>();
            isRunnning = false;
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
}