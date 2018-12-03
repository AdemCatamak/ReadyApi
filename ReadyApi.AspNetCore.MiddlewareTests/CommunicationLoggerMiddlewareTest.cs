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
    public class CommunicationLoggerMiddlewareTest : IDisposable
    {
        private const string BASE_URL = "http://localhost:50010";
        private readonly IWebHost _webHost;
        private readonly Mock<ILogger<CommunicationLoggerMiddleware>> _loggerMock;

        public CommunicationLoggerMiddlewareTest()
        {
            _loggerMock = new Mock<ILogger<CommunicationLoggerMiddleware>>();

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

                                             builder.UseComminucationLoggerMiddleware(_loggerMock.Object);

                                             builder.UseMvc();
                                         })
                              .UseKestrel()
                              .UseUrls(BASE_URL)
                              .Build();

            _webHost.Start();
        }

        [Fact]
        public async Task CommunicationLogger_WithoutOption()
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
                                          LogLevel.Debug,
                                          It.IsAny<EventId>(),
                                          It.IsAny<Type>(),
                                          It.IsAny<Exception>(),
                                          It.IsAny<Func<Type, Exception, string>>()
                                         ),
                               Times.Exactly(2)
                              );
        }


        public void Dispose()
        {
            _webHost?.Dispose();
        }
    }
}