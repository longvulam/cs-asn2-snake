using UnityEngine.SceneManagement;
using UnityEngine;

public class Lobby : MonoBehaviour
{
    // Start is called before the first frame update
    public void ExitButton()
    {
        Application.Quit();
        Debug.Log("Game Closed");
    }

    public void SinglePlayer()
    {
        SceneManager.LoadScene("Snake");
        Debug.Log("Game Started");
    }

    public void MultiPlayer()
    {
        //SceneManager.LoadScene("WaitingLobby");
        SceneManager.LoadScene("WebsocketWaitingLobby"); 
        Debug.Log("Game Started");
    }


}
