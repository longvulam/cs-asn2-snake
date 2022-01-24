using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Text;

namespace WebsocketServer
{
    public class MessageReceiver
    {
        string playerId;
        int bufferSize;
        Uri serverUri;

        public delegate void MessageReceivedHandler(string message);
        public event MessageReceivedHandler OnReceivedEvent;
        public bool isListening;

        public MessageReceiver(int port, string id, int bufferSize = 1024) : this("localhost", port, id, bufferSize)
        {
        }

        public MessageReceiver(string host, int port, string id, int bufferSize)
        {
            playerId = id;
            this.bufferSize = bufferSize;
            serverUri = new Uri($"ws://{host}:{port}/ws.ashx?playerId={playerId}");
        }

        public void StartListen()
        {
            isListening = true;
            var taskWebConnect = Task.Run(() =>
            {
                while (isListening)
                {
                    ReceiveMessage();
                }
            });

            taskWebConnect.Wait();
        }

        public async void ReceiveMessage()
        {
            using (ClientWebSocket ws = new ClientWebSocket())
            {
                Console.WriteLine($"Trying to connect to {serverUri.AbsoluteUri}");

                var source = new CancellationTokenSource();
                await ws.ConnectAsync(serverUri, source.Token);
                if (ws.State != WebSocketState.Open)
                    return;

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
                OnReceivedEvent(message);
                Console.WriteLine("Complete response: {0}", message);
            }
        }

        public void StopListen()
        {
            isListening = false;
        }
    }
}