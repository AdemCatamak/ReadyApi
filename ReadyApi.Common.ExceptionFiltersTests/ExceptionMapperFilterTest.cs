using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ReadyApi.Common.Model;
using Xunit;

namespace ReadyApi.Common.ExceptionFiltersTests
{
    public class ExceptionMapperFilterTest : IDisposable
    {
        private const string BASE_URL = "http://localhost:50015";

        private readonly IWebHost _webHost;

        public ExceptionMapperFilterTest()
        {
            _webHost = WebHost.CreateDefaultBuilder(null)
                              .ConfigureServices(services =>
                                                 {
                                                     services.AddCors();
                                                     services.AddMvc(options => { });
                                                 })
                              .Configure(builder =>
                                         {
                                             builder.UseCors(option => option.AllowAnyHeader()
                                                                             .AllowAnyMethod()
                                                                             .AllowAnyOrigin());

                                             builder.UseMvc();
                                         })
                              .UseKestrel()
                              .UseUrls(BASE_URL)
                              .Build();

            _webHost.Start();
        }

        [Fact]
        public async Task ValidationExceptionMapping_Test()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                HttpResponseMessage result = await client.GetAsync($"exception-mapper-filter/{nameof(ValidationException)}");
                string content = await result.Content.ReadAsStringAsync();

                var basicErrorResponse = JsonConvert.DeserializeObject<BasicErrorResponse>(content);
                Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
                Assert.Equal("Please verify your request is valid or not", basicErrorResponse.FriendlyMessage);
            }
        }

        [Fact]
        public async Task NullReferenceExceptionMapping_Test()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                HttpResponseMessage result = await client.GetAsync($"exception-mapper-filter/{nameof(NullReferenceException)}");
                Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
            }
        }


        [Fact]
        public async Task ObjectNotFoundMapping_Test()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                HttpResponseMessage result = await client.GetAsync($"exception-mapper-filter/{nameof(KeyNotFoundException)}");
                Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
            }
        }

        [Fact]
        public async Task AnotherTypeOfException_Test()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                HttpResponseMessage result = await client.GetAsync($"exception-mapper-filter/{nameof(FileNotFoundException)}");
                Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
            }
        }

        public void Dispose()
        {
            _webHost?.Dispose();
        }
    }
}