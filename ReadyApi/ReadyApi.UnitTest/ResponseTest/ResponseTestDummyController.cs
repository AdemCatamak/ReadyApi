using System;
using System.Web.Http;
using Alternatives.CustomExceptions;

namespace ReadyApi.UnitTest.ResponseTest
{
    [RoutePrefix("response-test-dummy")]
    public class ResponseTestDummyController : ApiController
    {
        [Route("primative-response")]
        [HttpGet]
        public bool GetBasicResponse()
        {
            return true;
        }

        [Route("friendly-ex-response")]
        [HttpGet]
        public bool GetFriendlyErrorResponse()
        {
            throw new FriendlyException("Friendly Exception Explanation");
        }

        [Route("business-ex-response")]
        [HttpGet]
        public bool GetBusinessErrorResponse()
        {
            throw new BusinessException("Business Exception Explanation");
        }

        [Route("unexpected-ex-response")]
        [HttpGet]
        public bool GetUnexpectedErrorResponse()
        {
            throw new Exception("Unexpected Exception Explanation");
        }
    }
}