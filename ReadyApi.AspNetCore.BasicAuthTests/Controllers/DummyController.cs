using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ReadyApi.AspNetCore.BasicAuthTests.Controllers
{
    [Route("")]
    public class DummyController : Controller
    {
        [Route("general")]
        [HttpGet]
        public string General()
        {
            return "This is general response";
        }

        [Route("user-only")]
        [HttpGet]
        [Authorize]
        public string UserOnly()
        {
            return "This response is exist for all user";
        }

        [Route("admin-only")]
        [HttpGet]
        [Authorize(Roles = "admin")]
        public string AdminOnly()
        {
            return "This response is exist for admin";
        }

        [Route("admin-and-other")]
        [HttpGet]
        [Authorize(Roles = "otherrole , admin")]
        public string MultiRole()
        {
            return "This response is exist for admin and some other response";
        }

        [Route("exluce-admin")]
        [HttpGet]
        [Authorize(Roles = "otherrole")]
        public string NotAdmin()
        {
            return "This response is exist for other roles";
        }
    }
}
