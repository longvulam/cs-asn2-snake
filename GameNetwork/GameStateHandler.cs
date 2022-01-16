internal class GameStateHandler
{
    private GameState gameState;

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
        // All game logic

    }
}