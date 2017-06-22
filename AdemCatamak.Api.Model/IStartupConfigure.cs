using System.Web.Http;

namespace AdemCatamak.Api.Model
{
    public interface IStartupConfigure
    {
        void Configure(HttpConfiguration config);
    }
}