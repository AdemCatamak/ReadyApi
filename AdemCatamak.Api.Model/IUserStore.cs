using System.Collections.Generic;

namespace AdemCatamak.Api.Model
{
    public interface IUserRoleStore
    {
        IEnumerable<string> GetRoles(string userName);
    }
}