namespace WebsocketClient.Models
{
    public class BroadcastMessage
    {
        public BroadcastMessage()
        {

        }

        public BroadcastMessageDestination dest;
        public string body;
    }

    public enum BroadcastMessageDestination
    {
        Player = 'p',
        Server = 's',
    }
}