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
    public class ExceptionLoggerMiddlewareTest : IDisposable
    {
        private const string BASE_URL = "http://localhost:5009";
        private readonly IWebHost _webHost;
        private readonly Mock<ILogger<ExceptionLoggerMiddleware>> _loggerMock;

        public ExceptionLoggerMiddlewareTest()
        {
            _loggerMock = new Mock<ILogger<ExceptionLoggerMiddleware>>();
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

                                             builder.UseExceptionLoggerMiddleware(_loggerMock.Object);

                                             builder.UseMvc();
                                         })
                              .UseKestrel()
                              .UseUrls(BASE_URL)
                              .Build();

            _webHost.Start();
        }

        [Fact]
        public async Task SystemEx_Test()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                HttpResponseMessage result = await client.GetAsync("system-ex");
                Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);

                _loggerMock.Verify(logger => logger.Log(LogLevel.Error,
                                                        It.IsAny<EventId>(),
                                                        It.IsAny<Type>(),
                                                        It.IsAny<Exception>(),
                                                        It.IsAny<Func<Type, Exception, string>>()
                                                       ),
                                   Times.Once);
            }
        }

        [Theory]
        [InlineData(200)]
        [InlineData(304)]
        public async Task CustomEx_Test_InfoLevelLog(int httpStatus)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);
                HttpResponseMessage result = await client.GetAsync($"custom-ex-with-api-prob/{httpStatus}");
                await result.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);

                _loggerMock.Verify(logger => logger.Log(LogLevel.Information,
                                                        It.IsAny<EventId>(),
                                                        It.IsAny<Type>(),
                                                        It.IsAny<Exception>(),
                                                        It.IsAny<Func<Type, Exception, string>>()
                                                       ),
                                   Times.Once);
            }
        }


        [Theory]
        [InlineData(401)]
        [InlineData(410)]
        public async Task CustomEx_Test_WarningLevelLog(int httpStatus)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);
                HttpResponseMessage result = await client.GetAsync($"custom-ex-with-api-prob/{httpStatus}");
                await result.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);

                _loggerMock.Verify(logger => logger.Log(LogLevel.Warning,
                                                        It.IsAny<EventId>(),
                                                        It.IsAny<Type>(),
                                                        It.IsAny<Exception>(),
                                                        It.IsAny<Func<Type, Exception, string>>()
                                                       ),
                                   Times.Once);
            }
        }

        [Theory]
        [InlineData(500)]
        [InlineData(511)]
        public async Task CustomEx_Test_ErrorLevelLog(int httpStatus)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);
                HttpResponseMessage result = await client.GetAsync($"custom-ex-with-api-prob/{httpStatus}");
                await result.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);

                _loggerMock.Verify(logger => logger.Log(LogLevel.Error,
                                                        It.IsAny<EventId>(),
                                                        It.IsAny<Type>(),
                                                        It.IsAny<Exception>(),
                                                        It.IsAny<Func<Type, Exception, string>>()
                                                       ),
                                   Times.Once);
            }
        }


        public void Dispose()
        {
            _webHost?.Dispose();
        }
    }
}