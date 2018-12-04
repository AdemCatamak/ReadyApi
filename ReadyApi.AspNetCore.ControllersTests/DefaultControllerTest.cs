using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ReadyApi.AspNetCore.ControllersTests
{
    public class DefaultControllerTest : IDisposable
    {
        private const string BASE_URL = "http://localhost:50001";

        private readonly IWebHost _webhost;

        public DefaultControllerTest()
        {
            _webhost = WebHost.CreateDefaultBuilder(null)
                              .ConfigureServices(services =>
                                                 {
                                                     services.AddCors();
                                                     services.AddMvc();
                                                 })
                              .Configure(builder =>
                                         {
                                             builder.UseCors(p => p.AllowAnyHeader()
                                                                   .AllowAnyOrigin());
                                             builder.UseMvc();
                                         })
                              .UseKestrel()
                              .UseUrls(BASE_URL)
                              .Build();

            _webhost.Start();
        }

        [Fact]
        public async Task HealthCheck()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                HttpResponseMessage result = await client.GetAsync("/health-check");
                string content = await result.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Equal(Environment.MachineName, content);
            }
        }

        public void Dispose()
        {
            _webhost?.Dispose();
        }
    }
}