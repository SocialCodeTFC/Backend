using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using SocialCode.API;

namespace SocialCode.UnitTesting.Resources
{
    public class TestFixture: IDisposable
    {
        private readonly TestServer _server;

        public TestFixture()
        {
            var builder = new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.Development.json");
                    Configuration = config.Build();
                });
            _server = new TestServer(builder);
            Client = _server.CreateClient();
        }

        public HttpClient Client { get; }
        
        public IConfiguration Configuration { get; set; }
        
        public void Dispose()
        {
            Client.Dispose();
            _server.Dispose();
        }
    }
}