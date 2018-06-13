using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ReadyApi.Core.BasicAuth.Test
{
    public class BasicAuthTest : IDisposable
    {
        private const string BASE_URL = "http://localhost:50000";

        private readonly IWebHost _webhost;

        public BasicAuthTest()
        {
            _webhost = WebHost.CreateDefaultBuilder(null)
                              .UseStartup<Startup>()
                              .UseKestrel()
                              .UseUrls(BASE_URL)
                              .Build();

            _webhost.Start();
        }

        [Fact]
        public void General_WithoutUserInfo()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                HttpResponseMessage result = client.GetAsync("general").Result;
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Contains("This is general response", result.Content.ReadAsStringAsync().Result);
            }
        }

        [Fact]
        public void General_WithCorrectUserInfo()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                string cridentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:123"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BasicAuthDefaults.AuthenticationScheme, cridentials);

                HttpResponseMessage result = client.GetAsync("general").Result;
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Contains("This is general response", result.Content.ReadAsStringAsync().Result);
            }
        }

        [Fact]
        public void General_WithIncorrectUserInfo()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                string cridentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:1"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BasicAuthDefaults.AuthenticationScheme, cridentials);

                HttpResponseMessage result = client.GetAsync("general").Result;
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Contains("This is general response", result.Content.ReadAsStringAsync().Result);
            }
        }

        [Fact]
        public void UserOnly_WithoutUserInfo()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                HttpResponseMessage result = client.GetAsync("user-only").Result;
                Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
            }
        }

        [Fact]
        public void UserOnly_WithCorrectUserInfo()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                string cridentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:123"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BasicAuthDefaults.AuthenticationScheme, cridentials);

                HttpResponseMessage result = client.GetAsync("user-only").Result;
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Contains("This response is exist for all user", result.Content.ReadAsStringAsync().Result);
            }
        }

        [Fact]
        public void UserOnly_WithIncorrectUserInfo()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                string cridentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:1"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BasicAuthDefaults.AuthenticationScheme, cridentials);

                HttpResponseMessage result = client.GetAsync("user-only").Result;
                Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
            }
        }

        [Fact]
        public void AdminOnly_WithoutUserInfo()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                HttpResponseMessage result = client.GetAsync("admin-only").Result;
                Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
            }
        }

        [Fact]
        public void AdminOnly_WithCorrectUserInfo()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                string cridentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:123"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BasicAuthDefaults.AuthenticationScheme, cridentials);

                HttpResponseMessage result = client.GetAsync("admin-only").Result;
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Contains("This response is exist for admin", result.Content.ReadAsStringAsync().Result);
            }
        }

        [Fact]
        public void AdminOnly_WithIncorrectUserInfo()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                string cridentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:1"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BasicAuthDefaults.AuthenticationScheme, cridentials);

                HttpResponseMessage result = client.GetAsync("admin-only").Result;
                Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
            }
        }

        [Fact]
        public void Admin_WithoutUserInfo()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                HttpResponseMessage result = client.GetAsync("admin-and-other").Result;
                Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
            }
        }

        [Fact]
        public void Admin_WithCorrectUserInfo()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                string cridentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:123"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BasicAuthDefaults.AuthenticationScheme, cridentials);

                HttpResponseMessage result = client.GetAsync("admin-and-other").Result;
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
                Assert.Contains("This response is exist for admin and some other response", result.Content.ReadAsStringAsync().Result);
            }
        }

        [Fact]
        public void Admin_WithIncorrectUserInfo()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                string cridentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:1"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BasicAuthDefaults.AuthenticationScheme, cridentials);

                HttpResponseMessage result = client.GetAsync("admin-and-other").Result;
                Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
            }
        }

        [Fact]
        public void NotAdmin_WithoutUserInfo()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                HttpResponseMessage result = client.GetAsync("exluce-admin").Result;
                Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
            }
        }

        [Fact]
        public void NotAdmin_WithCorrectUserInfo()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                string cridentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:123"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BasicAuthDefaults.AuthenticationScheme, cridentials);

                HttpResponseMessage result = client.GetAsync("exluce-admin").Result;
                Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
            }
        }

        [Fact]
        public void NotAdmin_WithIncorrectUserInfo()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASE_URL);

                string cridentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:1"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BasicAuthDefaults.AuthenticationScheme, cridentials);

                HttpResponseMessage result = client.GetAsync("exluce-admin").Result;
                Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
            }
        }

        public void Dispose()
        {
            _webhost?.Dispose();
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddCors();
                services.AddAuthentication(BasicAuthDefaults.AuthenticationScheme)
                        .AddBasic(options =>
                                  {
                                      options.AllowInsecureProtocol = true;
                                      options.ExecuteBasicAuthHandler = (username, password) =>
                                                                        {
                                                                            ClaimsPrincipal claimsPrincipal = null;
                                                                            if (username == "admin" && password == "123")
                                                                            {
                                                                                GenericIdentity identity = new GenericIdentity("1");
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