using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using NUnit.Framework;

namespace ReadyApi.UnitTest.BasicAuthenticationTest
{
    public class BasicAuthenticationFilterTest
    {
        private readonly string _baseAddress = "http://localhost:9000/";

        [Test]
        public void AdemCatamak_Api_Test__BasicAuthenticationFilterTest__NotAuthenticate()
        {
            using (WebApp.Start<BasicAuthenticationStartup>(_baseAddress))
            {
                using (HttpClient client = new HttpClient())
                {
                    Task<HttpResponseMessage> response = client.GetAsync($"{_baseAddress}/dummy/double/4");

                    string responseContent = response.Result.Content.ReadAsStringAsync().Result;

                    Assert.AreEqual("8", responseContent);
                }
            }
        }

        [Test]
        public void AdemCatamak_Api_Test__BasicAuthenticationFilterTest__AuthenticateSuccess_GeneralUser()
        {
            using (WebApp.Start<BasicAuthenticationStartup>(_baseAddress))
            {
                using (HttpClient client = new HttpClient())
                {
                    string cridentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("adem:1"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", cridentials);
                    Task<HttpResponseMessage> response = client.GetAsync($"{_baseAddress}/dummy/user-double/4");

                    string responseContent = response.Result.Content.ReadAsStringAsync().Result;

                    Assert.AreEqual("8", responseContent);
                }
            }
        }

        [Test]
        public void AdemCatamak_Api_Test__BasicAuthenticationFilterTest__AuthenticateFail_GeneralUser()
        {
            using (WebApp.Start<BasicAuthenticationStartup>(_baseAddress))
            {
                using (HttpClient client = new HttpClient())
                {
                    string cridentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("adem:2"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", cridentials);
                    Task<HttpResponseMessage> response = client.GetAsync($"{_baseAddress}/dummy/user-double/4");

                    Assert.IsFalse(response.Result.IsSuccessStatusCode);
                    Assert.IsTrue(response.Result.StatusCode == HttpStatusCode.Unauthorized);
                }
            }
        }


        [Test]
        public void AdemCatamak_Api_Test__BasicAuthenticationFilterTest__AuthenticateSuccess_SpecialUser_OnlyOneRole()
        {
            using (WebApp.Start<BasicAuthenticationStartup>(_baseAddress))
            {
                using (HttpClient client = new HttpClient())
                {
                    string cridentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("adem:1"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", cridentials);
                    Task<HttpResponseMessage> response = client.GetAsync($"{_baseAddress}/dummy/special-double-role3/4");

                    string responseContent = response.Result.Content.ReadAsStringAsync().Result;
                    Assert.AreEqual("8", responseContent);
                }
            }
        }

        [Test]
        public void AdemCatamak_Api_Test__BasicAuthenticationFilterTest__AuthenticateFail_SpecialUser_OnlyOneRole_WrongPassword()
        {
            using (WebApp.Start<BasicAuthenticationStartup>(_baseAddress))
            {
                using (HttpClient client = new HttpClient())
                {
                    string cridentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("adem:2"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", cridentials);
                    Task<HttpResponseMessage> response = client.GetAsync($"{_baseAddress}/dummy/special-double-role3/4");

                    Assert.IsFalse(response.Result.IsSuccessStatusCode);
                    Assert.AreEqual(HttpStatusCode.Unauthorized, response.Result.StatusCode);
                }
            }
        }

        [Test]
        public void AdemCatamak_Api_Test__BasicAuthenticationFilterTest__AuthenticateFail_SpecialUser_OnlyOneRole_NotValidRole()
        {
            using (WebApp.Start<BasicAuthenticationStartup>(_baseAddress))
            {
                using (HttpClient client = new HttpClient())
                {
                    string cridentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("adem:1"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", cridentials);
                    Task<HttpResponseMessage> response = client.GetAsync($"{_baseAddress}/dummy/special-double-role2/4");

                    Assert.IsFalse(response.Result.IsSuccessStatusCode);
                    Assert.IsTrue(response.Result.StatusCode == HttpStatusCode.Unauthorized);
                }
            }
        }


        [Test]
        public void AdemCatamak_Api_Test__BasicAuthenticationFilterTest__AuthenticateSuccess_SpecialUser_MultipleRole()
        {
            using (WebApp.Start<BasicAuthenticationStartup>(_baseAddress))
            {
                using (HttpClient client = new HttpClient())
                {
                    string cridentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("adem:1"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", cridentials);
                    Task<HttpResponseMessage> response = client.GetAsync($"{_baseAddress}/dummy/special-double-roles-valid/4");

                    string responseContent = response.Result.Content.ReadAsStringAsync().Result;
                    Assert.AreEqual("8", responseContent);
                }
            }
        }

        [Test]
        public void AdemCatamak_Api_Test__BasicAuthenticationFilterTest__AuthenticateFail_SpecialUser_MultipleRole_WrongPassword()
        {
            using (WebApp.Start<BasicAuthenticationStartup>(_baseAddress))
            {
                using (HttpClient client = new HttpClient())
                {
                    string cridentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("adem:2"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", cridentials);
                    Task<HttpResponseMessage> response = client.GetAsync($"{_baseAddress}/dummy/special-double-roles-notvalid/4");

                    Assert.IsFalse(response.Result.IsSuccessStatusCode);
                    Assert.AreEqual(HttpStatusCode.Unauthorized, response.Result.StatusCode);
                }
            }
        }

        [Test]
        public void AdemCatamak_Api_Test__BasicAuthenticationFilterTest__AuthenticateFail_SpecialUser_MultipleRole_NotValid()
        {
            using (WebApp.Start<BasicAuthenticationStartup>(_baseAddress))
            {
                using (HttpClient client = new HttpClient())
                {
                    string cridentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("adem:1"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", cridentials);
                    Task<HttpResponseMessage> response = client.GetAsync($"{_baseAddress}/dummy/special-double-roles-notvalid/4");

                    Assert.IsFalse(response.Result.IsSuccessStatusCode);
                    Assert.AreEqual(HttpStatusCode.Unauthorized, response.Result.StatusCode);
                }
            }
        }
    }
}