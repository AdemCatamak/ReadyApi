using System;
using System.Net;
using System.Net.Http;
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
    public class CommunicationLoggerTest : IDisposable
    {
        const string baseUrl = "http://localhost:5008";
        private readonly IWebHost _webHost;

        public CommunicationLoggerTest()
        {
            _webHost = WebHost.CreateDefaultBuilder(null)
                              .UseStartup<Startup>()
                              .UseKestrel()
                              .UseUrls(baseUrl)
                              .Build();

            _webHost.Start();
        }

        [Fact]
        public void CommunicationLogger_WithoutOption()
        {
            HttpResponseMessage result;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);

                result = client.GetAsync("/health-check").Result;
            }

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            _logger.Verify(
                           m => m.Log(
                                      LogLevel.Debug,
                                      It.IsAny<EventId>(),
                                      It.Is<FormattedLogValues>(values => true),
                                      It.IsAny<Exception>(),
                                      It.IsAny<Func<object, Exception, string>>()
                                     ),
                           Times.Exactly(2)
                          );
        }


        public void Dispose()
        {
            _webHost?.Dispose();
        }

        private static Mock<ILogger> _logger;

        private class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                _logger = new Mock<ILogger>();
                services.AddCors();
                services.AddMvc();
            }

            public void Configure(IApplicationBuilder app)
            {
                app.UseComminucationLogger(_logger.Object);

                app.UseCors(option => option.WithOrigins("*")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod()
                                            .AllowAnyOrigin());

                app.UseMvc();
            }
        }
    }
}