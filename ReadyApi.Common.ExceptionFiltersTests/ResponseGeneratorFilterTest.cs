using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ReadyApi.Common.ExceptionFilters;
using ReadyApi.Common.Model;
using Xunit;

namespace ReadyApi.Common.ExceptionFiltersTests
{
    public class ResponseGeneratorFilterTest : IDisposable
    {
        private const string BASE_URL = "http://localhost:50003";

        private readonly IWebHost _webHost;

        public ResponseGeneratorFilterTest()
        {
            _webHost = WebHost.CreateDefaultBuilder(null)
                              .ConfigureServices(services =>
                                                 {
                                                     services.AddCors();
                                                     services.AddMvc(options => { options.Filters.Add<ResponseGeneratorFilter>(); });
                                                 })
                              .Configure(builder =>
                                         {
                                             builder.UseCors(option => option.WithOrigins("*")
                                                                             .AllowAnyHeader()
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
        public async Task SystemEx_Test()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                HttpResponseMessage result = await client.GetAsync("system-ex");
                string content = await result.Content.ReadAsStringAsync();

                var basicErrorResponse = JsonConvert.DeserializeObject<BasicErrorResponse>(content);

                Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
                Assert.Equal("Unexpected Error Occurs", basicErrorResponse.FriendlyMessage);
            }
        }

        [Theory]
        [InlineData(200)]
        [InlineData(401)]
        [InlineData(500)]
        public async Task CustomEx_Test(int httpStatus)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);
                HttpResponseMessage result = await client.GetAsync($"custom-ex-with-api-prob/{httpStatus}");
                string content = await result.Content.ReadAsStringAsync();

                var basicErrorResponse = JsonConvert.DeserializeObject<BasicErrorResponse>(content);

                Assert.Equal(httpStatus, (int) result.StatusCode);
                Assert.Equal("Title : Test Title", basicErrorResponse.FriendlyMessage);
            }
        }


        public void Dispose()
        {
            _webHost?.Dispose();
        }
    }
}