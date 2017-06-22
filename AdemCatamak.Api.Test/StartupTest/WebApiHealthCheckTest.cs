using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdemCatamak.Api.Test.StartupTest
{
    [TestClass]
    public class WebApiHealthCheckTest
    {
        private readonly string _baseAddress = "http://localhost:9000/";

        [TestMethod]
        public void CheckHealth()
        {
            using (WebApp.Start<Startup>(_baseAddress))
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