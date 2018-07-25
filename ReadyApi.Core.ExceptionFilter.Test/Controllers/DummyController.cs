using System;
using System.Net;
using Alternatives.CustomExceptions;
using Microsoft.AspNetCore.Mvc;

namespace ReadyApi.Core.ExceptionFilter.Test.Controllers
{
    [Route("")]
    public class DummyController : Controller
    {
        [Route("system-ex")]
        [HttpGet]
        public void SystemException()
        {
            throw new ApplicationException("dummy message cannot be seen by client");
        }

        [Route("custom-ex/{httpStatus}")]
        [HttpGet]
        public void CustomException(int httpStatus)
        {
            throw new CustomApiException("Some error occur", (HttpStatusCode) httpStatus);
        }
    }
}