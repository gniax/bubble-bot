using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace BubbleBot.Api
{
    public class Program
    {
        public static class Constants
        {
            //public static string WebsiteIpAddress = "http://localhost:80"; // Website address
            //public static string ApiIpAddress = "http://localhost:5001"; // VPS address 

            public static string WebsiteIpAddress = "http://api.example.com:80"; // Website address 
            public static string ApiIpAddress = "http://api.example.com:5001"; // VPS address 
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
