using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using ReadyApi.Common.ExceptionFilters;
using ReadyApi.Common.Exceptions;
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

        [Route("custom-ex-with-basic-prob/{exceptionTag}")]
        [HttpGet]
        public void CustomExceptionWithBasicProblemDetails(ExceptionTags exceptionTag)
        {
            throw new CustomException(new BasicProblemDetails("Test Title"), exceptionTag);
        }

        [Route("custom-ex-with-api-prob/{httpStatus}")]
        [HttpGet]
        public void CustomExceptionWithApiProblemDetails(int httpStatus)
        {
            throw new CustomException(new ApiProblemDetails("Test Title", (HttpStatusCode) httpStatus));
        }

        [Route("exception-mapper-filter/{exceptionName}")]
        [HttpGet]
        [ExceptionMapperFilter(typeof(KeyNotFoundException), HttpStatusCode.NotFound)]
        [ExceptionMapperFilter(typeof(NullReferenceException), HttpStatusCode.InternalServerError)]
        [ExceptionMapperFilter(typeof(ValidationException), HttpStatusCode.BadRequest, "Please verify your request is valid or not")]
        public void ExceptionMapperTest(string exceptionName)
        {
            if (exceptionName == nameof(KeyNotFoundException))
            {
                throw new KeyNotFoundException();
            }
            else if (exceptionName == nameof(NullReferenceException))
            {
                throw new NullReferenceException();
            }
            else if (exceptionName == nameof(ValidationException))
            {
                throw new ValidationException();
            }
            else
            {
                throw new Exception();
            }
        }
    }
}