using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using ReadyApi.Core.Middlewares;
using Xunit;

namespace ReadyApi.Core.Test.Middlewares
{
    public class ProcessTimeWatcherTest : IDisposable
    {
        const string baseUrl = "http://localhost:5009";
        private readonly IWebHost _webHost;

        public ProcessTimeWatcherTest()
        {
            _webHost = WebHost.CreateDefaultBuilder(null)
                              .UseStartup<Startup>()
                              .UseKestrel()
                              .UseUrls(baseUrl)
                              .Build();

            _webHost.Start();
        }

        [Fact]
        public async Task ProcessTimeWatcher_Test()
        {
            HttpResponseMessage result;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);

                result = await client.GetAsync("/health-check");
            }

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            _logger.Verify(
                           m => m.Log(
                                      LogLevel.Trace,
                                      It.IsAny<EventId>(),
                                      It.Is<FormattedLogValues>(values => true),
                                      It.IsAny<Exception>(),
                                      It.IsAny<Func<object, Exception, string>>()
                                     ),
                           Times.Once
                          );
        }


        public void Dispose()
        {
            _webHost?.Dispose();
        }

        private static Mock<ILogger<ProcessTimeWatcherMiddleware>> _logger;

        private class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                _logger = new Mock<ILogger<ProcessTimeWatcherMiddleware>>();
                services.AddCors();
                services.AddMvc();
            }

            public void Configure(IApplicationBuilder app)
            {
                app.UseProcessTimeWatcherMiddleware(_logger.Object);

                app.UseCors(option => option.AllowAnyHeader()
                                            .AllowAnyMethod()
                                            .AllowAnyOrigin());

                app.UseMvc();
            }
        }
    }
}