using System.Drawing;
using System.Net.Sockets;
using Encoding = System.Text.Encoding;
using Stream = System.IO.Stream;

namespace AirMouse
{
    public static class TcpManager
    {
        private const int Port = 7777;
        private static TcpClient _client;
        private static NetworkStream _stream;

        public static bool Connected => _client != null && _client.Connected;

        public static void Connect(string address)
        {
            _client = new TcpClient(address, Port);
            _stream = _client.GetStream();
        }

        public static NetworkStream GetStream() => _stream;

        public static void Disconnect()
        {
            _stream?.Close();
            _client?.Close();
        }

        public static TcpClient GetClient() => _client;
    }
}