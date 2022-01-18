using GameNetwork;
using GameNetwork.Models;
using UnityEngine;

public class NetworkSender : MonoBehaviour
{
    private const string IpAddress = "230.0.0.1";
    private const int Port = 11000;

    MessageSender sender;
    private Snake playerInput;

    private void Start()
    {
        sender = new MessageSender(IpAddress, Port);
        playerInput = GetComponent<Snake>();
    }

    private void FixedUpdate()
    {
        try
        {
            //Vector2 pos = this.transform.position;
            string playerId = playerInput.ID.ToString();
            var player = new PlayerState(playerId, Direction.Down);
            BroadcastMessage message = new BroadcastMessage
            {
                dest = BroadcastMessageDestination.Server,
                body = JsonUtility.ToJson(player),
            };

            string jsonMsg = JsonUtility.ToJson(message);
            sender.SendMessage(jsonMsg);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }
}
