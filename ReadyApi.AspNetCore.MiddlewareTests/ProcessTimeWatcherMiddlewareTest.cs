using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using ReadyApi.AspNetCore.Middleware;
using Xunit;

namespace ReadyApi.AspNetCore.MiddlewareTests
{
    public class ProcessTimeWatcherMiddlewareTest : IDisposable
    {
        private const string BASE_URL = "http://localhost:5013";
        private readonly IWebHost _webHost;
        private readonly Mock<ILogger<ProcessTimeWatcherMiddleware>> _loggerMock;

        public ProcessTimeWatcherMiddlewareTest()
        {
            _loggerMock = new Mock<ILogger<ProcessTimeWatcherMiddleware>>();
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

                                             builder.UseProcessTimeWatcherMiddleware(_loggerMock.Object);

                                             builder.UseMvc();
                                         })
                              .UseKestrel()
                              .UseUrls(BASE_URL)
                              .Build();

            _webHost.Start();
        }

        [Fact]
        public async Task ProcessTimeWatcher_Test()
        {
            HttpResponseMessage result;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                result = await client.GetAsync("/health-check");
            }

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            _loggerMock.Verify(
                               m => m.Log(
                                          LogLevel.Information,
                                          It.IsAny<EventId>(),
                                          It.IsAny<Type>(),
                                          It.IsAny<Exception>(),
                                          It.IsAny<Func<Type, Exception, string>>()
                                         ),
                               Times.Once
                              );
        }


        public void Dispose()
        {
            _webHost?.Dispose();
        }
    }
}