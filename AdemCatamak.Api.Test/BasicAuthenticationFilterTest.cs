using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using AdemCatamak.Api.Filters;
using AdemCatamak.Api.Model;
using AdemCatamak.Api.Test.TestBasicAuthenticationChecker;
using Microsoft.Owin.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdemCatamak.Api.Test
{
    [TestClass]
    public class BasicAuthenticationFilterTest
    {
        private readonly string _baseAddress = "http://localhost:9000/";

        [TestMethod]
        public void AdemCatamak_Api_Test__BasicAuthenticationFilterTest__NotAuthenticate()
        {
            using (WebApp.Start<Startup>(_baseAddress))
            {
                HttpConfiguration config = Startup.HttpConfig;

                IAuthenticationChecker checker = new AuthenticationCheckerTest();
                BasicAuthenticationFilter authenticationFilter = new BasicAuthenticationFilter(checker);

                config.Filters.Add(authenticationFilter);

                Startup.UpdateConfiguration(config);

                using (HttpClient client = new HttpClient())
                {
                    Task<HttpResponseMessage> response = client.GetAsync($"{_baseAddress}/test/double/4");

                    string responseContent = response.Result.Content.ReadAsStringAsync().Result;

                    Assert.AreEqual("8", responseContent);
                }
            }
        }

        [TestMethod]
        public void AdemCatamak_Api_Test__BasicAuthenticationFilterTest__AuthenticateSuccess()
        {
            using (WebApp.Start<Startup>(_baseAddress))
            {
                HttpConfiguration config = Startup.HttpConfig;

                IAuthenticationChecker checker = new AuthenticationCheckerTest();
                BasicAuthenticationFilter authenticationFilter = new BasicAuthenticationFilter(checker);

                config.Filters.Add(authenticationFilter);

                Startup.UpdateConfiguration(config);

                using (HttpClient client = new HttpClient())
                {
                    string cridentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("adem:1"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", cridentials);
                    Task<HttpResponseMessage> response = client.GetAsync($"{_baseAddress}/test/special-double/4");

                    string responseContent = response.Result.Content.ReadAsStringAsync().Result;

                    Assert.AreEqual("8", responseContent);
                }
            }
        }

        [TestMethod]
        public void AdemCatamak_Api_Test__BasicAuthenticationFilterTest__AuthenticateFail()
        {
            using (WebApp.Start<Startup>(_baseAddress))
            {
                HttpConfiguration config = Startup.HttpConfig;

                IAuthenticationChecker checker = new AuthenticationCheckerTest();
                BasicAuthenticationFilter authenticationFilter = new BasicAuthenticationFilter(checker);

                config.Filters.Add(authenticationFilter);

                Startup.UpdateConfiguration(config);

                using (HttpClient client = new HttpClient())
                {
                    string cridentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("adem:2"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", cridentials);
                    Task<HttpResponseMessage> response = client.GetAsync($"{_baseAddress}/test/special-double/4");

                    Assert.IsFalse(response.Result.IsSuccessStatusCode);
                    Assert.IsTrue(response.Result.StatusCode == HttpStatusCode.Unauthorized);
                }
            }
        }
    }
}