using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace BubbleBot.Website
{
    public class Program
    {
        public static class Constants
        { 
            public static string HostAddress = "http://localhost:80";
            public static string VpsIpAddress = "http://93.113.207.95:80"; // Vps ip + port
            public static string VpsApiIpAddress = "http://93.113.207.95:5001"; // Vps ip API
        }
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls(Constants.VpsIpAddress)
                .Build();

    }
}
