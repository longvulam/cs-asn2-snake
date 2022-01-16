using System.Collections;
using System.Collections.Generic;
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
        //TODO: load the multiplayer waiting lobby
        SceneManager.LoadScene("WaitingLobby");
        Debug.Log("Game Started");
    }


}
