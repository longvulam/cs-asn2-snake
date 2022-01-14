using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace GameNetwork
{
    public class MessageReceiver
    {

        // Define a delegate named LogHandler, which will encapsulate
        // any method that takes a string as the parameter and returns no value
        public delegate void MessageReceivedHandler(string message);

        // Define an Event based on the above Delegate
        public event MessageReceivedHandler OnReceivedEvent;

        private IPAddress mcastAddress;
        private int mcastPort;
        private Socket mcastSocket;
        private MulticastOption mcastOption;
        private bool isListening = false;

        public bool IsListening { get => isListening; set => isListening = value; }

        public MessageReceiver(string ipAddress, int port)
        {
            bool isValidAddr = IPAddress.TryParse(ipAddress, out mcastAddress);
            if (isValidAddr)
            {
                mcastPort = port;
            }
            else
            {
                throw new ArgumentException("Could not parse IP address", ipAddress);
            }
        }

        public void StartListen()
        {
            try
            {
                mcastSocket = new Socket(AddressFamily.InterNetwork,
                               SocketType.Dgram,
                               ProtocolType.Udp);


                IPAddress localIP = IPAddress.Any;
                EndPoint localEP = new IPEndPoint(localIP, mcastPort);
                mcastSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);

                mcastSocket.Bind(localEP);


                // Define a MulticastOption object specifying the multicast group 
                // address and the local IPAddress.
                // The multicast group address is the same as the address used by the server.
                mcastOption = new MulticastOption(mcastAddress, localIP);

                mcastSocket.SetSocketOption(SocketOptionLevel.IP,
                                            SocketOptionName.AddMembership,
                                            mcastOption);

                byte[] bytes = new byte[100];
                int receivedBytes = 0;
                EndPoint groupEP = new IPEndPoint(mcastAddress, mcastPort);
                EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                isListening = true;
                while (isListening)
                {
                    receivedBytes = mcastSocket.ReceiveFrom(bytes, ref groupEP);

                    string message = Encoding.ASCII.GetString(bytes, 0, receivedBytes);

                    OnMessageReceived(message);
                }

                mcastSocket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.ToString());
                throw;
            }
        }

        public void StopListen()
        {
            isListening = false;
        }

        private void OnMessageReceived(string message)
        {
            if (OnReceivedEvent != null)
            {
                OnReceivedEvent.Invoke(message);
            }
        }
    }
}