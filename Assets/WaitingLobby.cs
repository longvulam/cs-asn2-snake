using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class WaitingLobby : MonoBehaviour
{
    Text player;

    private void Start()
    {
        player = GameObject.Find("PlayerName").GetComponent<Text>();

    }

    // Start is called before the first frame update
    public void BackButton()
    {
        SceneManager.LoadScene("Lobby");
        Debug.Log("Back Button");
    }

    public void StartButton()
    {
        //TODO: Connect with backend server so the scene can be loaded with all the snakes
        //TODO: Along with the snakes the name and the score of each player must be displayed on the top
        SceneManager.LoadScene("Snake");
        Debug.Log("Game Started");
    }

    public void LoadPlayers()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => DisplayText());
    }

    public void DisplayText ()
    {
        for(int i = 0; i < 5; i++)
        {
            string playerName = "Testing";
            player.text += string.Format("Player {0}: {1}\n", i, playerName);
        }
    }

}
