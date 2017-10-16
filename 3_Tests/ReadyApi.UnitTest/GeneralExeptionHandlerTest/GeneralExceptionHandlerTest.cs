using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using NUnit.Framework;

namespace ReadyApi.UnitTest.GeneralExeptionHandlerTest
{
    public class GeneralExceptionHandlerTest
    {
        private readonly string _baseAddress = "http://localhost:9000/";

        [Test]
        public void AdemCatamak_Api_Test__GeneralExceptionHandlerTest__FriendlyException()
        {
            using (WebApp.Start<GeneralExceptionStartup>(_baseAddress))
            {
                using (HttpClient client = new HttpClient())
                {
                    const string message = "FriendlyMessage";
                    Task<HttpResponseMessage> rawResponse = client.GetAsync($"{_baseAddress}/throw-friendly-exception/{message}");

                    Assert.IsFalse(rawResponse.Result.IsSuccessStatusCode);
                    Assert.AreEqual(HttpStatusCode.BadRequest,rawResponse.Result.StatusCode);
                    Assert.AreEqual(message, rawResponse.Result.ReasonPhrase);
                }
            }
        }

        [Test]
        public void AdemCatamak_Api_Test__GeneralExceptionHandlerTest__BusinessException()
        {
            using (WebApp.Start<GeneralExceptionStartup>(_baseAddress))
            {
                using (HttpClient client = new HttpClient())
                {
                    const string message = "BusinessException";
                    Task<HttpResponseMessage> rawResponse = client.GetAsync($"{_baseAddress}/throw-business-exception/{message}");

                    Assert.IsFalse(rawResponse.Result.IsSuccessStatusCode);
                    Assert.AreEqual(HttpStatusCode.InternalServerError,rawResponse.Result.StatusCode);
                    Assert.AreEqual(message, rawResponse.Result.ReasonPhrase);
                }
            }
        }

        [Test]
        public void AdemCatamak_Api_Test__GeneralExceptionHandlerTest__Exception()
        {
            using (WebApp.Start<GeneralExceptionStartup>(_baseAddress))
            {
                using (HttpClient client = new HttpClient())
                {
                    const string message = "SystemExceptions";
                    Task<HttpResponseMessage> rawResponse = client.GetAsync($"{_baseAddress}/throw-exception/{message}");

                    Assert.IsFalse(rawResponse.Result.IsSuccessStatusCode);
                    Assert.AreEqual(HttpStatusCode.InternalServerError, rawResponse.Result.StatusCode);
                    Assert.AreEqual("Unexpected Error", rawResponse.Result.ReasonPhrase);
                }
            }
        }
    }
}