using System.Web.Http;

namespace ReadyApi.Model
{
    public interface IStartupConfigure
    {
        void Configure(HttpConfiguration config);
    }
}