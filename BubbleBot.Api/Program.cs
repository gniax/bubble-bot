using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace BubbleBot.Api
{
    public class Program
    {
        public static class Constants
        {
            public static string WebsiteIpAddress = "http://localhost:80"; // Website address
            public static string ApiIpAddress = "http://localhost:5001"; // VPS address 

            //public static string WebsiteIpAddress = "http://93.113.207.95:80"; // Website address 
            //public static string ApiIpAddress = "http://93.113.207.95:5001"; // VPS address 
        }
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls(Constants.ApiIpAddress)
                .Build();
    }
}
