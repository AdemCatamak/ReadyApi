using System;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ReadyApi.Core.Middlewares;
using Xunit;

namespace ReadyApi.Core.Test.Middlewares
{
    public class InsecureProtocolManagerTest
    {
        [Fact]
        public void InsecureProtocolManager_Secure()
        {
            const string baseUrl = "http://localhost:5001";

            IWebHost webHost = WebHost.CreateDefaultBuilder(null)
                                      .UseStartup<SecureStartup>()
                                      .UseKestrel()
                                      .UseUrls(baseUrl)
                                      .Build();

            webHost.Start();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);

                HttpResponseMessage result = client.GetAsync("/health-check").Result;
                Assert.Equal(HttpStatusCode.PreconditionFailed, result.StatusCode);
            }

            webHost.Dispose();
        }


        [Fact]
        public void InsecureProtocolManager_Insecure()
        {
            const string baseUrl = "http://localhost:5002";

            IWebHost webHost = WebHost.CreateDefaultBuilder(null)
                                      .UseStartup<InsecureStartup>()
                                      .UseKestrel()
                                      .UseUrls(baseUrl)
                                      .Build();

            webHost.Start();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);

                HttpResponseMessage result = client.GetAsync("/health-check").Result;
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            }

            webHost.Dispose();
        }


        private class InsecureStartup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddCors();
                services.AddMvc();
            }

            public void Configure(IApplicationBuilder app)
            {
                app.UseInsecureProtocol(new InsecureProtocolMiddlewareOptions() { InsecureProtocolAllowed = true });

                app.UseCors(option => option.WithOrigins("*")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod()
                                            .AllowAnyOrigin());

                app.UseMvc();
            }
        }

        private class SecureStartup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddCors();
                services.AddMvc();
            }

            public void Configure(IApplicationBuilder app)
            {
                app.UseInsecureProtocol(new InsecureProtocolMiddlewareOptions() { InsecureProtocolAllowed = false });

                app.UseCors(option => option.WithOrigins("*")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod()
                                            .AllowAnyOrigin());

                app.UseMvc();
            }
        }
    }
}