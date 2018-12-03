using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ReadyApi.Common.ExceptionFilter;
using RestSharp;
using Xunit;

namespace ReadyApi.Common.ExceptionFiltersTests
{
    public class ExceptionLoggerFilterTest : IDisposable
    {
        private const string BASE_URL = "http://localhost:50001";

        private readonly IWebHost _webhost;

        public ExceptionLoggerFilterTest()
        {
            _webhost = WebHost.CreateDefaultBuilder(null)
                              .ConfigureServices(services =>
                                                 {
                                                     services.AddCors();
                                                     services.AddMvc(options =>
                                                                     {
                                                                         options.Filters.Add<ExceptionLoggerFilter>();
                                                                     });
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

            _webhost.Start();
        }

        [Fact]
        public void SystemEx_Test()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                HttpResponseMessage result = client.GetAsync("system-ex").Result;
                Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
                Assert.Contains("Unexpected error occur", result.Content.ReadAsStringAsync().Result);
            }
        }

        [Theory]
        [InlineData(200)]
        [InlineData(304)]
        [InlineData(401)]
        [InlineData(500)]
        public void CustomEx_Test(int httpStatus)
        {
            RestClient restClient = new RestClient(BASE_URL) {Encoding = Encoding.UTF8};
            IRestResponse<BasicErrorResponse> errorResponse = restClient.Get<BasicErrorResponse>(new RestRequest($"custom-ex/{httpStatus}"));
            Assert.Equal((HttpStatusCode) httpStatus, errorResponse.StatusCode);
            if (httpStatus < 300 || httpStatus >= 400)
                Assert.Contains("Some error occur", errorResponse.Data.FriendlyMessage);
            else
                Assert.Null(errorResponse.Data);
        }


        public void Dispose()
        {
            _webhost?.Dispose();
        }
    }
}