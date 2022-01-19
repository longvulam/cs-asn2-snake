using System.Linq;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using GameNetwork;
using System.Threading;
using System;
using Newtonsoft.Json;
using GameNetwork.Models;

public class WaitingLobby : MonoBehaviour
{
    public static Guid PlayerGuid;
    Text player;
    MessageReceiver receiver;
    MessageSender sender;

    private void Start()
    {
        player = GameObject.Find("PlayerName").GetComponent<Text>();
        sender = new MessageSender(NetworkController.IpAddress, NetworkController.Port);
        SendJoinGameMessage();

        receiver = new MessageReceiver(NetworkController.IpAddress, NetworkController.Port, 1024);
        receiver.OnReceivedEvent += OnMessageReceived;

        Thread thread = new Thread(new ThreadStart(receiver.StartListen));
        thread.Start();
    }

    private void SendJoinGameMessage()
    {
        PlayerGuid = Guid.NewGuid();
        var playerState = new PlayerState(PlayerGuid.ToString(), Direction.Up);
        var body = NetworkController.WrapMessage(playerState);
        string json = JsonConvert.SerializeObject(body);
        sender.SendMessage(json);
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
        var body = NetworkController.WrapMessage(Constants.StartGameCode);
        string json = JsonConvert.SerializeObject(body);
        sender.SendMessage(json);
        Debug.Log("OnStarted");
    }

    public void OnMessageReceived(string newPlayersJoinedJson)
    {
        UnityMainThreadDispatcher taskDispatcher = UnityMainThreadDispatcher.Instance();
        Task task = taskDispatcher.EnqueueAsync(() => HandleMessageRecieved(newPlayersJoinedJson));
    }

    public void HandleMessageRecieved(string newPlayersJoinedJson)
    {
        BroadcastMessage message = NetworkController.ParseMessage(newPlayersJoinedJson);
        GameState state = NetworkController.ParseGameState(message.body);

        if(state.isRunnning)
        {
            SceneManager.LoadScene("SnakeMultiplayer");
            return;
        }

        if (state.playerStates.Any())
        {
            player.text = "";
        }

        for (int i = 0; i < state.playerStates.Count; i++)
        {
            PlayerState playerState = state.playerStates[i];
            player.text += string.Format("Player #{0}: {1}\n", i, playerState.id);
        }
    }

    private void OnDestroy()
    {
        receiver.StopListen();
    }
}
