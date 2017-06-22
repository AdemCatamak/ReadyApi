using System.Web.Http;

namespace AdemCatamak.Api.Test.TestControllers
{
    [RoutePrefix("test")]
    public class TestController : ApiController
    {
        [HttpGet]
        [Route("double/{parameter:int}")]
        public int Sum([FromUri]int parameter)
        {
            return parameter * 2;
        }

        [HttpGet]
        [Route("special-double/{parameter:int}")]
        [Authorize]
        public int SumSpecial([FromUri]int parameter)
        {
            return parameter * 2;
        }
    }
}