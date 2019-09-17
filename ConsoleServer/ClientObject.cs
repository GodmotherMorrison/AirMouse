using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using Library;

namespace ConsoleServer
{
    public class ClientObject
    {
        private readonly TcpClient _client;
        private readonly string _clientIp;

        public ClientObject(TcpClient tcpClient)
        {
            _client = tcpClient;
            _clientIp = ((IPEndPoint)_client?.Client.RemoteEndPoint)?.Address.ToString();
        }

        public void Process()
        {
            Console.WriteLine($"{_clientIp}: connection established.");
            while (true)
            {
                NetManager.Stream = _client.GetStream();
                NetManager.Client = _client;
                var data = NetManager.Take();
                var input = data.Split('\n');
                foreach (var inp in input)
                {
                    if (inp == "") continue;

                    var inpByte = Encoding.UTF8.GetBytes(inp);
                    var cmd = (NetManager.Command)inpByte[0];
                    var message = Encoding.UTF8.GetString(SubArray(inpByte, 1, inp.Length - 1));

                    switch (cmd)
                    {
                        case NetManager.Command.Move:
                            var pos = message.Split('|');
                            Console.WriteLine($"{_clientIp}: {{X={int.Parse(pos[0])}, Y={int.Parse(pos[1])}}}");
                            VirtualMouse.Move(int.Parse(pos[0]), int.Parse(pos[1]));
                            break;
                        case NetManager.Command.ConnectClose:
                            Console.WriteLine($"The client({_clientIp}) has broken the connection.\n");
                            Console.WriteLine($"Host name : {IpManager.GetHostName()}");
                            Console.WriteLine($"Local IP Address : {IpManager.GetLocalIpAddress()}");
                            Console.WriteLine($"Public IP Address : {IpManager.GetPublicIpAddress()}");
                            return;
                        case NetManager.Command.RightDown:
                            VirtualMouse.RightDown();
                            break;
                        case NetManager.Command.RightUp:
                            VirtualMouse.RightUp();
                            break;
                        case NetManager.Command.LeftDown:
                            VirtualMouse.LeftDown();
                            break;
                        case NetManager.Command.LeftUp:
                            VirtualMouse.LeftUp();
                            break;
                        default:
                            Console.WriteLine(cmd + message);
                            break;
                    }
                }
            }
        }

        private static byte[] SubArray(byte[] data, int index, int length)
        {
            var result = new byte[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

    }
}
