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
using System.Collections;

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
        var body = NetworkController.WrapMessage(Constants.StartGameCode);
        string json = JsonConvert.SerializeObject(body);
        sender.SendMessage(json);
        Debug.Log("OnStarted");
    }

    public void OnMessageReceived(string newPlayersJoinedJson)
    {
        BroadcastMessage message = NetworkController.ParseMessage(newPlayersJoinedJson);
        if (message == null)
        {
            Debug.LogError(newPlayersJoinedJson);
            return;
        }

        UnityMainThreadDispatcher taskDispatcher = UnityMainThreadDispatcher.Instance();
        Task task = taskDispatcher.EnqueueAsync(() => HandleMessageRecieved(message));
    }

    IEnumerator ActionWrapper(Action a)
    {
        a();
        yield return null;
    }

    public void HandleMessageRecieved(BroadcastMessage message)
    {

        GameState state = NetworkController.ParseGameState(message.body);

        if (state.isRunnning)
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
