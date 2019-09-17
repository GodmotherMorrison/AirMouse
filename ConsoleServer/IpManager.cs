using System.Net;
using System.Net.Sockets;

namespace ConsoleServer
{
    public static class IpManager
    {
        public static string GetLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();

            return "No network adapters with an IPv4 address in the system!";
        }

        public static string GetHostName()
        {
            return Dns.GetHostEntry(Dns.GetHostName()).HostName;
        }

        public static string GetPublicIpAddress()
        {
            return new WebClient().DownloadString("http://icanhazip.com");
        }
    }
}
