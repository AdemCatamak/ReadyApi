using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using NUnit.Framework;

namespace ReadyApi.UnitTest.ResponseTest
{
    public class ResponseTest
    {
        private string _baseAddress = "http://localhost:9000";

        [Test]
        public void GetBasicResponseTest()
        {
            using (WebApp.Start(_baseAddress, Startup.Configuration))
            {
                using (HttpClient client = new HttpClient())
                {
                    Task<HttpResponseMessage> response = client.GetAsync($"{_baseAddress}/response-test-dummy/primative-response");
                    string responseContent = response.Result.Content.ReadAsStringAsync().Result;
                    bool responseValue = Convert.ToBoolean(responseContent);

                    Assert.IsTrue(response.Result.IsSuccessStatusCode);
                    Assert.IsTrue(responseValue);
                }
            }
        }

        [Test]
        public void GetFriendlyExResponseTest()
        {
            using (WebApp.Start(_baseAddress, Startup.Configuration))
            {
                using (HttpClient client = new HttpClient())
                {
                    Task<HttpResponseMessage> response = client.GetAsync($"{_baseAddress}/response-test-dummy/friendly-ex-response");

                    Assert.IsFalse(response.Result.IsSuccessStatusCode);
                    Assert.AreEqual(HttpStatusCode.BadRequest, response.Result.StatusCode);
                    Assert.AreEqual("Friendly Exception Explanation", response.Result.ReasonPhrase);
                }
            }
        }
        
        [Test]
        public void GetBusinessExResponseTest()
        {
            using (WebApp.Start(_baseAddress, Startup.Configuration))
            {
                using (HttpClient client = new HttpClient())
                {
                    Task<HttpResponseMessage> response = client.GetAsync($"{_baseAddress}/response-test-dummy/business-ex-response");

                    Assert.IsFalse(response.Result.IsSuccessStatusCode);
                    Assert.AreEqual(HttpStatusCode.InternalServerError, response.Result.StatusCode);
                    Assert.AreEqual("Business Exception Explanation", response.Result.ReasonPhrase);
                }
            }
        }

        [Test]
        public void GetUnexpectedExResponseTest()
        {
            using (WebApp.Start(_baseAddress, Startup.Configuration))
            {
                using (HttpClient client = new HttpClient())
                {
                    Task<HttpResponseMessage> response = client.GetAsync($"{_baseAddress}/response-test-dummy/unexpected-ex-response");

                    Assert.IsFalse(response.Result.IsSuccessStatusCode);
                    Assert.AreEqual(HttpStatusCode.InternalServerError, response.Result.StatusCode);
                    Assert.AreEqual("Unexpected Exception Explanation", response.Result.ReasonPhrase);
                }
            }
        }
    }
}