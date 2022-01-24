using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Text;

namespace WebsocketClient
{
    public class MessageReceiver
    {
        int bufferSize;
        Uri serverUri;
        ClientWebSocket ws;
        CancellationTokenSource source;

        public delegate void MessageReceivedHandler(string message);
        public event MessageReceivedHandler OnReceivedEvent;
        public bool isListening;


        public MessageReceiver(int port, string playerId, int bufferSize = 1024) : this("localhost", port, playerId, bufferSize)
        {
        }

        public MessageReceiver(string host, int port, string playerId, int bufferSize = 1024)
        {
            this.bufferSize = bufferSize;
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

        public void StartListen()
        {
            isListening = true;
            var taskWebConnect = Task.Run(async () =>
            {
                while (isListening && ws.State == WebSocketState.Open)
                {
                    string message = await ReceiveMessage();
                    OnReceivedEvent(message);
                }
            });
        }

        public async Task<string> ReceiveMessage()
        {
            //Receive buffer
            byte[] receiveBuffer = new byte[bufferSize];
            int offset = 0, dataPerPacket = 10;
            while (true)
            {
                ArraySegment<byte> bytesReceived = new ArraySegment<byte>(receiveBuffer, offset, dataPerPacket);
                WebSocketReceiveResult result = await ws.ReceiveAsync(bytesReceived, source.Token);

                offset += result.Count;
                if (result.EndOfMessage)
                    break;
            }

            string message = Encoding.UTF8.GetString(receiveBuffer, 0, offset);
            return message;
        }

        public void StopListen()
        {
            isListening = false;
            ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Close", source.Token);
        }
    }
}