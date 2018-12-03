using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ReadyApi.AspNetCore.Middleware;
using Xunit;

namespace ReadyApi.AspNetCore.MiddlewareTests
{
    public class CorrelationIdMiddlewareTest : IDisposable
    {
        private const string BASE_URL = "http://localhost:5009";
        private readonly IWebHost _webHost;

        public CorrelationIdMiddlewareTest()
        {
            _webHost = WebHost.CreateDefaultBuilder(null)
                              .ConfigureServices(services =>
                                                 {
                                                     services.AddCors();
                                                     services.AddMvc();
                                                 })
                              .Configure(builder =>
                                         {
                                             builder.UseCors(option => option.AllowAnyHeader()
                                                                             .AllowAnyMethod()
                                                                             .AllowAnyOrigin());

                                             builder.UseCorrelationIdMiddleware();

                                             builder.UseMvc();
                                         })
                              .UseKestrel()
                              .UseUrls(BASE_URL)
                              .Build();

            _webHost.Start();
        }

        [Fact]
        public async Task CorrelationIdMiddleware_Test()
        {
            HttpResponseMessage result;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                result = await client.GetAsync("/health-check");
            }

            Assert.NotEmpty(result.Headers.GetValues(CorrelationIdMiddlewareOptions.DEFAULT_HEADER));
        }

        [Fact]
        public async Task CorrelationIdMiddleware_RequestHasCorrId_Test()
        {
            string correlationId = "test-123";
            HttpResponseMessage result;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);
                client.DefaultRequestHeaders.Add(CorrelationIdMiddlewareOptions.DEFAULT_HEADER, correlationId);

                result = await client.GetAsync("/health-check");
            }

            Assert.Equal(correlationId, result.Headers.GetValues(CorrelationIdMiddlewareOptions.DEFAULT_HEADER).FirstOrDefault());
        }


        public void Dispose()
        {
            _webHost?.Dispose();
        }
    }
}