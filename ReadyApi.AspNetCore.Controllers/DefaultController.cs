using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ReadyApi.AspNetCore.Controllers
{
    [Route("")]
    public class DefaultController : Controller
    {
        [HttpGet, Route("health-check")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public IActionResult Get()
        {
            return StatusCode((int) HttpStatusCode.OK, Environment.MachineName);
        }

        private static bool? _swaggerEndpoint = null;

        [HttpGet, Route("")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Home()
        {
            using (var client = new HttpClient())
            {
                string defaultSwaggerUrl = $"{Request.Scheme}://{Request.Host.ToUriComponent()}/swagger";
                HttpResponseMessage httpResponseMessage = await client.GetAsync(defaultSwaggerUrl);
                _swaggerEndpoint = httpResponseMessage.IsSuccessStatusCode;
            }

            if (_swaggerEndpoint != null && _swaggerEndpoint.Value)
                return Redirect($"{Request.Scheme}://{Request.Host.ToUriComponent()}/swagger");
            else
                return Ok();
        }
    }
}