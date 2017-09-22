using System;
using System.Web.Http;
using ReadyApi.Model.CustomExceptions;

namespace ReadyApi.UnitTest.BasicAuthenticationTest
{
    [RoutePrefix("dummy")]
    public class BasicAuthenticationDummyController : ApiController
    {
        [HttpGet]
        [Route("double/{parameter:int}")]
        public int Twice([FromUri] int parameter)
        {
            return parameter * 2;
        }

        [HttpGet]
        [Route("user-double/{parameter:int}")]
        [Authorize]
        public int TwiceUser([FromUri] int parameter)
        {
            return parameter * 2;
        }

        [HttpGet]
        [Route("special-double-role2/{parameter:int}")]
        [Authorize(Roles = "role2")]
        public int TwiceSingleSpecialNotValid([FromUri] int parameter)
        {
            return parameter * 2;
        }

        [HttpGet]
        [Route("special-double-role3/{parameter:int}")]
        [Authorize(Roles = "role3")]
        public int TwiceSingleSpecialValid([FromUri] int parameter)
        {
            return parameter * 2;
        }

        [HttpGet]
        [Route("special-double-roles-valid/{parameter:int}")]
        [Authorize(Roles = "role1,role2")]
        public int TwiceMultiSpecialValid([FromUri] int parameter)
        {
            return parameter * 2;
        }

        [HttpGet]
        [Route("special-double-roles-notvalid/{parameter:int}")]
        [Authorize(Roles = "role2,role4")]
        public int TwiceMultiSpecialNotValid([FromUri] int parameter)
        {
            return parameter * 2;
        }
    }
}