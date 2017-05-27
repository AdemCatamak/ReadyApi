using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AdemCatamak.Api.Controller
{
    [RoutePrefix("")]
    public class DefaultController : ApiController
    {
        [Route("")]
        [HttpGet]
        public HttpResponseMessage Home()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Moved);
            response.Headers.Location = new Uri(Request.RequestUri + "\\swagger");
            return response;
        }
    }
}