using GameNetwork;
using GameNetwork.Models;
using Newtonsoft.Json;
using System;
using UnityEngine;

public class NetworkSender : MonoBehaviour
{
    Direction _direction = Direction.Right;
    MessageSender sender;
    string playerId;

    private void Start()
    {
        sender = new MessageSender(NetworkController.IpAddress, NetworkController.Port);
        Guid id = WaitingLobby.PlayerGuid;
        playerId = id == Guid.Empty ? Guid.NewGuid().ToString() : id.ToString();
        //InvokeRepeating("SendState", 0, 1);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.W) && (_direction != Direction.Down))
        //{
        //    _direction = Direction.Up;
        //}
        //else if (Input.GetKeyDown(KeyCode.S) && (_direction != Direction.Up))
        //{
        //    _direction = Direction.Down;
        //}
        //else if (Input.GetKeyDown(KeyCode.A) && (_direction != Direction.Right))
        //{
        //    _direction = Direction.Left;
        //}
        //else if (Input.GetKeyDown(KeyCode.D) && (_direction != Direction.Left))
        //{
        //    _direction = Direction.Right;
        //}
    }

    private void FixedUpdate()
    {
        //SendState();
    }

    private void SendState()
    {
        try
        {
            var player = new PlayerState(playerId, _direction);
            BroadcastMessage message = new BroadcastMessage
            {
                dest = BroadcastMessageDestination.Server,
                body = JsonConvert.SerializeObject(player),
            };

            string jsonMsg = JsonConvert.SerializeObject(message);
            Debug.Log("Sendind: " + jsonMsg);
            sender.SendMessage(jsonMsg);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}
