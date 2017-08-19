using System.Collections.Generic;
using ReadyApi.Model;

namespace ReadyApi.UnitTest.BasicAuthenticationTest
{
    public class UserRoleStore : IUserRoleStore
    {
        public IEnumerable<string> GetRoles(string userName)
        {
            string[] roles = { };
            if (userName == "adem")
            {
                roles = new[] {"role1", "role3"};
            }
            return roles;
        }
    }
}