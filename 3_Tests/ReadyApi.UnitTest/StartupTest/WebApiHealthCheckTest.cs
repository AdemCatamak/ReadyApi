using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using NUnit.Framework;

namespace ReadyApi.UnitTest.StartupTest
{
    
    public class WebApiHealthCheckTest
    {
        private readonly string _baseAddress = "http://localhost:9000/";

        [Test]
        public void CheckHealth()
        {
            using (WebApp.Start(_baseAddress, Startup.Configuration))
            {
                using (HttpClient client = new HttpClient())
                {
                    Task<HttpResponseMessage> response = client.GetAsync($"{_baseAddress}/health-check");

                    string responseContent = response.Result.Content.ReadAsStringAsync().Result;
                    Assert.IsTrue(response.Result.IsSuccessStatusCode);
                    Assert.AreEqual(HttpStatusCode.OK, response.Result.StatusCode);
                    Assert.AreEqual("true", responseContent);
                }
            }
        }
    }
}