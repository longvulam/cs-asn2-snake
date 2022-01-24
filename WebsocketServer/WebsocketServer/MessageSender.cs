using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Text;

namespace WebsocketServer
{
    public class MessageSender
    {
        string playerId;
        Uri serverUri;

        public MessageSender(int port, string id) : this("localhost", port, id)
        {
        }

        public MessageSender(string host, int port, string id)
        {
            playerId = id;
            serverUri = new Uri($"ws://{host}:{port}/ws.ashx?playerId={playerId}");
        }

        public void SendMessage(string message)
        {
            var taskWebConnect = Task.Run(async () => {
                using (ClientWebSocket ws = new ClientWebSocket())
                {
                    Console.WriteLine($"Trying to connect to {serverUri.AbsoluteUri}");

                    var source = new CancellationTokenSource();
                    //source.CancelAfter(5000);

                    await ws.ConnectAsync(serverUri, source.Token);
                    while (ws.State == WebSocketState.Open)
                    {
                        ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
                        await ws.SendAsync(bytesToSend, WebSocketMessageType.Text, true, source.Token);
                    }
                }
            });

            taskWebConnect.Wait();
        }
    }
}