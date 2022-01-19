using GameNetwork;
using GameNetwork.Models;
using UnityEngine;

public class NetworkSender : MonoBehaviour
{
    Direction _direction = Direction.Right;
    MessageSender sender;
    string playerId;

    private void Start()
    {
        sender = new MessageSender(NetworkController.IpAddress, NetworkController.Port);
        playerId = WaitingLobby.PlayerGuid.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && (_direction != Direction.Down))
        {
            _direction = Direction.Up;
        }
        else if (Input.GetKeyDown(KeyCode.S) && (_direction != Direction.Up))
        {
            _direction = Direction.Down;
        }
        else if (Input.GetKeyDown(KeyCode.A) && (_direction != Direction.Right))
        {
            _direction = Direction.Left;
        }
        else if (Input.GetKeyDown(KeyCode.D) && (_direction != Direction.Left))
        {
            _direction = Direction.Right;
        }
    }

    private void FixedUpdate()
    {
        try
        {
            var player = new PlayerState(playerId, _direction);
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
