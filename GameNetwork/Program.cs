using System;
using System.Threading;

namespace GameNetwork
{
    class Program
    {
        private const string IpAddress = "230.0.0.1";
        private const int Port = 11000;

        static void Main(string[] args)
        {

            new Server(IpAddress, Port).Start();
        }
    }
}
