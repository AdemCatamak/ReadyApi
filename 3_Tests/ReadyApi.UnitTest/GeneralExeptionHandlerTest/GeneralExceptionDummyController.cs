using System;
using System.Web.Http;
using ReadyApi.Model.CustomExceptions;

namespace ReadyApi.UnitTest.GeneralExeptionHandlerTest
{
    [RoutePrefix("")]
    public class GeneralExceptionDummyController : ApiController
    {
        [HttpGet]
        [Route("throw-friendly-exception/{parameter}")]
        public int ThrowFriendlyExceptionEndPoint([FromUri] string parameter)
        {
            throw new FriendlyException(parameter);
        }

        [HttpGet]
        [Route("throw-business-exception/{parameter}")]
        public int ThrowBusinessExceptionEndPoint([FromUri] string parameter)
        {
            throw new BusinessException(parameter);
        }

        [HttpGet]
        [Route("throw-exception/{parameter}")]
        public int ThrowExceptionEndPoint([FromUri] string parameter)
        {
            throw new Exception(parameter);
        }
    }
}
