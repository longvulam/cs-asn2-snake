using GameNetwork;
using Newtonsoft.Json;
using System;
using System.Threading;
using UnityEngine;

public class NetworkController : MonoBehaviour
{
    public static string IpAddress = "230.0.0.1";
    public static int Port = 11000;

    MessageReceiver receiver;

    public static BroadcastMessage WrapMessage(string body)
    {
        return new BroadcastMessage
        {
            dest = BroadcastMessageDestination.Server,
            body = body
        };
    }

    public static BroadcastMessage WrapMessage(PlayerState body)
    {
        return new BroadcastMessage
        {
            dest = BroadcastMessageDestination.Server,
            body = JsonConvert.SerializeObject(body)
        };
    }

    public static BroadcastMessage ParseMessage(string msg)
    {
        var message = JsonConvert.DeserializeObject<BroadcastMessage>(msg);
        return message;
    }

    public static GameState ParseGameState(string msg)
    {
        var message = JsonConvert.DeserializeObject<GameState>(msg);
        return message;
    }

    private void Start()
    {
        receiver = new MessageReceiver(IpAddress, Port);
        receiver.OnReceivedEvent += UpdatePlayer;

        Thread thread = new Thread(new ThreadStart(receiver.StartListen));
        thread.Start();
    }

    private void UpdatePlayer(string commonGameState) // shared state coming from the server
    {
        UnityMainThreadDispatcher taskDispatcher = UnityMainThreadDispatcher.Instance();
        taskDispatcher.EnqueueAsync(() =>
        {
            try
            {
                BroadcastMessage message = ParseMessage(commonGameState);

                bool isToPlayer = message.dest == BroadcastMessageDestination.Player;
                if (isToPlayer == false) return;

                GameState gs = ParseGameState(message.body);

                Debug.Log($"Received: {commonGameState}");
            }
            catch (Exception e)
            {
                Debug.LogError($"{e}");
            }
        });

    }

    private void OnDestroy()
    {
        receiver.StopListen();
    }
}
