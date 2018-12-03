using System;
using System.Net;
using System.Net.Http;

namespace ReadyApi.Common.Controllers
{
    [RoutePrefix("")]
    public class DefaultController : Controller
    {
        [Route("")]
        [HttpGet]
        public HttpResponseMessage Home()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Moved);
            response.Headers.Location = new Uri(Request.RequestUri + "\\swagger");
            return response;
        }

        [Route("health-check")]
        [HttpGet]
        public HttpResponseMessage Check()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, true);
            return response;
        }
    }
}