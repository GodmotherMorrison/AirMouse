using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ConsoleServer
{
    internal class ProgramServ
    {
        private const int Port = 7777;

        private static TcpListener _listener;

        private static void Main()
        {
            try
            {
                Console.WriteLine($"Host name : {IpManager.GetHostName()}");
                Console.WriteLine($"Local IP Address : {IpManager.GetLocalIpAddress()}");
                Console.WriteLine($"Public IP Address : {IpManager.GetPublicIpAddress()}");
                _listener = new TcpListener(IPAddress.Any, Port);
                _listener.Start();
                Console.WriteLine("Waiting for connections ...");
                while (true)
                {
                    var client = _listener.AcceptTcpClient();
                    var clientObject = new ClientObject(client);

                    var clientThread = new Thread(clientObject.Process);
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                _listener?.Stop();
            }
        }
    }
}