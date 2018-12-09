using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace ReadyApi.AspNetCore.MiddlewareTests.Controllers
{
    [Route("")]
    public class DummyController : Controller
    {
        [Route("health-check")]
        [HttpGet]
        public IActionResult HealthCheck()
        {
            return StatusCode((int) HttpStatusCode.OK);
        }

        [Route("system-ex")]
        [HttpGet]
        public void SystemException()
        {
            throw new ApplicationException("dummy message cannot be seen by client");
        }
    }
}