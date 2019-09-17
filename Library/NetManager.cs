using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Library
{
    public static class NetManager
    {
        public static NetworkStream Stream { get; set; }
        public static TcpClient Client { get; set; }

        public enum Command
        {
            Move,
            RightClick,
            RightDown,
            RightUp,
            LeftClick,
            LeftDown,
            LeftUp,
            ConnectClose
        }

        public static void Send(Command cmd, string message)
        {
            var data = Encoding.UTF8.GetBytes(message + "\n");
            data = AddByteToArray(data, (byte)(int)cmd);
            Stream.Write(data, 0, data.Length);
        }

        public static string Take()
        {
            return Encoding.UTF8.GetString(Receive(Stream));
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            var result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        private static byte[] Receive(Stream stream)
        {
            var bytes = new byte[Client.ReceiveBufferSize];
            var bytesRead = stream.Read(bytes, 0, bytes.Length);

            var newArray = new byte[bytesRead];
            for (var i = 0; i < bytesRead; i++)
                newArray[i] = bytes[i];

            return newArray;
        }

        public static byte[] AddByteToArray(byte[] bArray, byte newByte)
        {
            var newArray = new byte[bArray.Length + 1];
            bArray.CopyTo(newArray, 1);
            newArray[0] = newByte;
            return newArray;
        }
    }
}
