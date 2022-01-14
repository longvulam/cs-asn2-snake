using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GameNetwork
{
    public class MessageSender
    {
        private IPAddress mcastAddress;
        private int mcastPort;
        private Socket mcastSocket;
        private IPEndPoint endPoint;

        public MessageSender(string ipAddress, int port)
        {
            bool isValidAddr = IPAddress.TryParse(ipAddress, out mcastAddress);
            if (isValidAddr)
            {
                mcastPort = port;
                endPoint = new IPEndPoint(mcastAddress, mcastPort);
            }
            else
            {
                throw new ArgumentException("Could not parse IP address", ipAddress);
            }
        }

        public void SendMessage(string message)
        {
            try
            {
                //Send multicast packets to the listener.
                mcastSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                byte[] asBytes = Encoding.ASCII.GetBytes(message);
                mcastSocket.SendTo(asBytes, asBytes.Length, SocketFlags.None, endPoint);
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.ToString());
            }

            mcastSocket.Close();
        }
    }
}