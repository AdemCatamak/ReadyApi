using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ReadyApi.AspNetCore.BasicAuth;
using Xunit;

namespace ReadyApi.AspNetCore.BasicAuthTests
{
    public class BasicAuthTest : IDisposable
    {
        private const string BASE_URL = "http://localhost:50000";

        private readonly IWebHost _webHost;

        public BasicAuthTest()
        {
            _webHost = WebHost.CreateDefaultBuilder(null)
                              .UseStartup<Startup>()
                              .UseKestrel()
                              .UseUrls(BASE_URL)
                              .Build();

            _webHost.Start();
        }

        [Fact]
        public async Task General_WithoutUserInfo()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                HttpResponseMessage result = await client.GetAsync("general");
                string content = await result.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Contains("This is general response", content);
            }
        }

        [Fact]
        public async Task General_WithCorrectUserInfo()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                string cridentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:123"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BasicAuthDefaults.AUTHENTICATION_SCHEME, cridentials);

                HttpResponseMessage result = await client.GetAsync("general");
                string content = await result.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Contains("This is general response", content);
            }
        }

        [Fact]
        public async Task General_WithIncorrectUserInfo()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                string cridentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:1"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BasicAuthDefaults.AUTHENTICATION_SCHEME, cridentials);

                HttpResponseMessage result = await client.GetAsync("general");
                string content = await result.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Contains("This is general response", content);
            }
        }

        [Fact]
        public async Task UserOnly_WithoutUserInfo()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                HttpResponseMessage result = await client.GetAsync("user-only");

                Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
            }
        }

        [Fact]
        public async Task UserOnly_WithCorrectUserInfo()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                string cridentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:123"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BasicAuthDefaults.AUTHENTICATION_SCHEME, cridentials);

                HttpResponseMessage result = await client.GetAsync("user-only");
                string content = await result.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Contains("This response is exist for all user", content);
            }
        }

        [Fact]
        public async Task UserOnly_WithIncorrectUserInfo()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                string cridentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:1"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BasicAuthDefaults.AUTHENTICATION_SCHEME, cridentials);

                HttpResponseMessage result = await client.GetAsync("user-only");
                Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
            }
        }

        [Fact]
        public async Task AdminOnly_WithoutUserInfo()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                HttpResponseMessage result = await client.GetAsync("admin-only");
                Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
            }
        }

        [Fact]
        public async Task AdminOnly_WithCorrectUserInfo()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                string cridentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:123"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BasicAuthDefaults.AUTHENTICATION_SCHEME, cridentials);

                HttpResponseMessage result = await client.GetAsync("admin-only");
                string content = await result.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Contains("This response is exist for admin", content);
            }
        }

        [Fact]
        public async Task AdminOnly_WithIncorrectUserInfo()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                string cridentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:1"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BasicAuthDefaults.AUTHENTICATION_SCHEME, cridentials);

                HttpResponseMessage result = await client.GetAsync("admin-only");
                Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
            }
        }

        [Fact]
        public async Task Admin_WithoutUserInfo()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                HttpResponseMessage result = await client.GetAsync("admin-and-other");
                Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
            }
        }

        [Fact]
        public async Task Admin_WithCorrectUserInfo()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                string cridentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:123"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BasicAuthDefaults.AUTHENTICATION_SCHEME, cridentials);

                HttpResponseMessage result = await client.GetAsync("admin-and-other");
                string content = await result.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Contains("This response is exist for admin and some other response", content);
            }
        }

        [Fact]
        public async Task Admin_WithIncorrectUserInfo()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                string cridentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:1"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BasicAuthDefaults.AUTHENTICATION_SCHEME, cridentials);

                HttpResponseMessage result = await client.GetAsync("admin-and-other");
                Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
            }
        }

        [Fact]
        public async Task NotAdmin_WithoutUserInfo()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                HttpResponseMessage result = await client.GetAsync("exluce-admin");
                Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
            }
        }

        [Fact]
        public async Task NotAdmin_WithCorrectUserInfo()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                string cridentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:123"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BasicAuthDefaults.AUTHENTICATION_SCHEME, cridentials);

                HttpResponseMessage result = await client.GetAsync("exluce-admin");
                Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
            }
        }

        [Fact]
        public async Task NotAdmin_WithIncorrectUserInfo()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                string cridentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:1"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BasicAuthDefaults.AUTHENTICATION_SCHEME, cridentials);

                HttpResponseMessage result = await client.GetAsync("exluce-admin");
                Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
            }
        }

        public void Dispose()
        {
            _webHost?.Dispose();
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddCors();
                services.AddAuthentication(BasicAuthDefaults.AUTHENTICATION_SCHEME)
                        .AddBasic(options =>
                                  {
                                      options.AllowInsecureProtocol = true;
                                      options.ExecuteBasicAuthHandler = (username, password) =>
                                                                        {
                                                                            ClaimsPrincipal claimsPrincipal = null;
                                                                            if (username == "admin" && password == "123")
                                                                            {
                                                                                var identity = new GenericIdentity("1");
                                                                                identity.AddClaim(new Claim(ClaimTypes.Role, "admin"));
                                                                                claimsPrincipal = new ClaimsPrincipal(identity);
                                                                            }

                                                                            return claimsPrincipal;
                                                                        };
                                  });
                services.AddMvc();
            }

            public void Configure(IApplicationBuilder app, IHostingEnvironment env)
            {
                app.UseAuthentication();
                app.UseCors(option => option.WithOrigins("*")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod()
                                            .AllowAnyOrigin());
                app.UseMvc();
            }
        }
    }
}