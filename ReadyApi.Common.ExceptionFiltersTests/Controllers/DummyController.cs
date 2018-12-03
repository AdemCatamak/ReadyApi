using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using ReadyApi.Common.Exceptions.CustomExceptions;
using ReadyApi.Common.Exceptions.ProbDetails;

namespace ReadyApi.Common.ExceptionFiltersTests.Controllers
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

        [Route("custom-ex-with-basic-prob/{httpStatus}")]
        [HttpGet]
        public void CustomExceptionWithBasicProblemDetails(int httpStatus)
        {
            throw new CustomException(new BasicProblemDetails("Test Title"));
        }

        [Route("custom-ex-with-api-prob/{httpStatus}")]
        [HttpGet]
        public void CustomExceptionWithApiProblemDetails(int httpStatus)
        {
            throw new CustomException(new ApiProblemDetails("Test Title", (HttpStatusCode) httpStatus));
        }
    }
}