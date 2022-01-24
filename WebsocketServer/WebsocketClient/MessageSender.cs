using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Text;

namespace WebsocketClient
{
    public class MessageSender
    {
        ClientWebSocket ws;
        CancellationTokenSource source;
        Uri serverUri;

        public MessageSender(int port, string playerId) : this("localhost", port, playerId)
        {
        }

        public MessageSender(string host, int port, string playerId)
        {
            serverUri = new Uri($"ws://{host}:{port}/ws.ashx?playerId={playerId}");

            source = new CancellationTokenSource();
            //source.CancelAfter(5000);

            ws = new ClientWebSocket();
            Task.Run(async () =>
            {
                Console.WriteLine($"Trying to connect to {serverUri.AbsoluteUri}");
                await ws.ConnectAsync(serverUri, source.Token);
            }).Wait();
        }

        public void SendMessage(string message)
        {
            var taskWebConnect = Task.Run(async () =>
            {
                if (ws.State != WebSocketState.Open) return;

                ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
                await ws.SendAsync(bytesToSend, WebSocketMessageType.Text, true, source.Token);
            });
        }

        public void CloseSocket()
        {
            ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Close", source.Token);
        }
    }
}