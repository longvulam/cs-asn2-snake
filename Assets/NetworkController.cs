﻿using GameNetwork;
using GameNetwork.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkController : MonoBehaviour
{
    public static string IpAddress = "230.0.0.1";
    public static int Port = 11000;
    public GameObject SegmentPrefab;
    public Color[] playerColors = { Color.cyan, Color.magenta, Color.blue, Color.green };

    string playerId;
    Direction _direction = Direction.Right;
    Direction prevDir = Direction.Right;

    MessageSender sender;
    MessageReceiver receiver;
    List<GameObject> _segments = new List<GameObject>();
    GameObject food;

    private void Start()
    {
        food = Instantiate(SegmentPrefab);
        food.GetComponent<SpriteRenderer>().color = Color.red;

        sender = new MessageSender(NetworkController.IpAddress, NetworkController.Port);
        Guid id = WaitingLobby.PlayerGuid;
        playerId = id == Guid.Empty ? Guid.NewGuid().ToString() : id.ToString();

        receiver = new MessageReceiver(IpAddress, Port, 1024);
        receiver.OnReceivedEvent += OnGameStateReceived;

        Thread thread = new Thread(new ThreadStart(receiver.StartListen));
        thread.Start();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && (prevDir != Direction.Down))
        {
            _direction = Direction.Up;
        }
        else if (Input.GetKeyDown(KeyCode.S) && (prevDir != Direction.Up))
        {
            _direction = Direction.Down;
        }
        else if (Input.GetKeyDown(KeyCode.A) && (prevDir != Direction.Right))
        {
            _direction = Direction.Left;
        }
        else if (Input.GetKeyDown(KeyCode.D) && (prevDir != Direction.Left))
        {
            _direction = Direction.Right;
        }
    }

    private void FixedUpdate()
    {
        SendState();
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
            sender.SendMessage(jsonMsg);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }


    private void OnGameStateReceived(string commonGameState) // shared state coming from the server
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
                UpdateGUI(gs);

                PlayerState ps = gs.playerStates.FirstOrDefault(st => st.id.Equals(playerId));
                if(ps != null)
                {
                    prevDir = ps.direction;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"{e}");
            }
        });
    }

    private void UpdateGUI(GameState gs)
    {

        Vector3 foodPos = food.transform.position;
        Coordinate newFoodPos = gs.foodPos;
        if (foodPos.x != newFoodPos.x || foodPos.y != newFoodPos.y)
        {
            food.transform.position = new Vector3(newFoodPos.x, newFoodPos.y);
        }

        foreach (GameObject segment in _segments)
        {
            Destroy(segment);
        }
        _segments.Clear();

        List<PlayerState> players = gs.playerStates;
        for (int i = 0; i < players.Count; i++)
        {
            PlayerState playerState = players[i];
            Color playerColor = playerColors[i];
            foreach (Coordinate coord in playerState.coordinates)
            {
                GameObject segment = Instantiate(this.SegmentPrefab);
                _segments.Add(segment);
                var renderer = segment.GetComponent<SpriteRenderer>();
                renderer.color = playerColor;
                segment.transform.position = new Vector3(coord.x, coord.y);
            }
        }
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void OnDestroy()
    {
        receiver.StopListen();
    }

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

}
