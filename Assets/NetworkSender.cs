using GameNetwork;
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
        Vector3 pos = this.transform.position;
        try
        {
            string playerState = JsonUtility.ToJson(new PlayerState(playerInput.ID, pos));
            BroadcastMessage message = new BroadcastMessage { 
                dest = BroadcastMessageDestination.Server,
                body = playerState,
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
