using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Alternatives.Extensions;
using Microsoft.Owin.Hosting;
using NUnit.Framework;
using System.Collections.Generic;

namespace ReadyApi.UnitTest.GeneralExeptionHandlerTest
{
    public class GeneralExceptionHandlerTest
    {
        private class BasicResponse
        {
            public bool Success { get; set; }
            public List<BasicResponse.Message> Messages { get; set; }

            internal class Message
            {
                public string MessageContent { get; set; }
                public string Type { get; set; }
            }
        }

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
                    BasicResponse response = rawResponse.Result.Content.ReadAsStringAsync().Result.Deserialize<BasicResponse>();
                    Assert.IsFalse(response.Success);
                    Assert.AreEqual(message, response.Messages.First().MessageContent);
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
                    BasicResponse response = rawResponse.Result.Content.ReadAsStringAsync().Result.Deserialize<BasicResponse>();
                    Assert.IsFalse(response.Success);
                    Assert.AreEqual(message, response.Messages.First().MessageContent);
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
                    BasicResponse response = rawResponse.Result.Content.ReadAsStringAsync().Result.Deserialize<BasicResponse>();
                    Assert.IsFalse(response.Success);
                    Assert.AreEqual("Unexpected Error", response.Messages.First().MessageContent);
                }
            }
        }
    }
}