using Microsoft.AspNetCore.Hosting;
using System;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Xunit;

namespace ReadyApi.Core.Test.Controllers
{
    public class DefaultControllerTest : IDisposable
    {
        private const string BASE_URL = "http://localhost:50000";

        private readonly IWebHost _webhost;

        public DefaultControllerTest()
        {
            _webhost = WebHost.CreateDefaultBuilder(null)
                              .UseStartup<Startup>()
                              .UseKestrel()
                              .UseUrls(BASE_URL)
                              .Build();

            _webhost.Start();
        }

        [Fact]
        public void HealthCheck_Test()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                HttpResponseMessage result = client.GetAsync("/health-check").Result;
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Equal(Environment.MachineName, result.Content.ReadAsStringAsync().Result);
            }
        }

        [Fact]
        public void RootRoute_Test()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                HttpResponseMessage result = client.GetAsync("/").Result;
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            }
        }


        public void Dispose()
        {
            _webhost?.Dispose();
        }

        private class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddCors();
                services.AddMvc();
                services.AddSwaggerGen(c => { c.SwaggerDoc("test-swagger", new Info()); });
            }

            public void Configure(IApplicationBuilder app, IHostingEnvironment env)
            {
                app.UseCors(option => option.WithOrigins("*")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod()
                                            .AllowAnyOrigin());

                app.UseMvc();
                app.UseSwagger();
                app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", $"test-swagger V1"); });
            }
        }
    }
}