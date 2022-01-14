using GameNetwork;
using System.Threading;
using UnityEngine;

public class NetworkController : MonoBehaviour
{

    public Snake player;

    private const string IpAddress = "230.0.0.1";
    private const int Port = 11000;
    private MessageReceiver receiver;

    private void Start()
    {
        receiver = new MessageReceiver(IpAddress, Port);
        receiver.OnReceivedEvent += UpdatePlayer;

        Thread thread = new Thread(new ThreadStart(receiver.StartListen));
        thread.Start();

    }

    private void UpdatePlayer(string otherPlayerJson)
    {
        Debug.Log($"otherPlayerJson: {otherPlayerJson}");
        try
        {
            PlayerState state = JsonUtility.FromJson<PlayerState>(otherPlayerJson);
            if (player.ID.ToString() != state.Id)
            {
                Debug.Log($"ID: {state.Id}");
                Debug.Log($"Position: {state.position}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"{e}");
        }
    }
}
