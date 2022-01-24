using System.Linq;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;
using Newtonsoft.Json;
using WebsocketClient;
using WebsocketClient.Models;

public class WebsocketWaitingLobby : MonoBehaviour
{
    public static Guid PlayerGuid;
    Text player;
    MessageReceiver receiver;
    MessageSender sender;

    private void Start()
    {
        player = GameObject.Find("PlayerName").GetComponent<Text>();
        PlayerGuid = Guid.NewGuid();
        string playerId = PlayerGuid.ToString();
        sender = new MessageSender(WebsocketNetworkController.IpAddress, WebsocketNetworkController.Port, playerId);
        receiver = new MessageReceiver(WebsocketNetworkController.IpAddress, WebsocketNetworkController.Port, playerId);
        receiver.OnReceivedEvent += OnMessageReceived;
        receiver.StartListen();

        SendJoinGameMessage();
    }

    private void SendJoinGameMessage()
    {
        var playerState = new PlayerState(PlayerGuid.ToString(), Direction.None);
        var body = WebsocketNetworkController.WrapMessage(playerState);
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
        var body = WebsocketNetworkController.WrapMessage(Constants.StartGameCode);
        string json = JsonConvert.SerializeObject(body);
        sender.SendMessage(json);
        Debug.Log("OnStarted");
    }

    public void OnMessageReceived(string newPlayersJoinedJson)
    {
        Debug.Log($"New state:\n{newPlayersJoinedJson}");
        BroadcastMessage message = WebsocketNetworkController.ParseMessage(newPlayersJoinedJson);
        if (message == null)
        {
            Debug.LogError(newPlayersJoinedJson);
            return;
        }

        UnityMainThreadDispatcher taskDispatcher = UnityMainThreadDispatcher.Instance();
        Task task = taskDispatcher.EnqueueAsync(() => HandleMessageRecieved(message));
    }

    public void HandleMessageRecieved(BroadcastMessage message)
    {

        GameState state = WebsocketNetworkController.ParseGameState(message.body);

        if (state.isRunnning)
        {
            SceneManager.LoadScene("WebsocketSnakeMultiplayer");
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
        sender.CloseSocket();
    }
}
