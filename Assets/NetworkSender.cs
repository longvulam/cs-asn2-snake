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
            string json = JsonUtility.ToJson(new PlayerState(playerInput.ID, pos));
            sender.SendMessage(json);
        }
        catch (System.Exception)
        {
            throw;
        }
    }
}
