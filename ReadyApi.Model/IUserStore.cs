using System.Collections.Generic;

namespace ReadyApi.Model
{
    public interface IUserRoleStore
    {
        IEnumerable<string> GetRoles(string userName);
    }
}