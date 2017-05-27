using Autofac;

namespace AdemCatamak.Api.Model
{
    public interface IIoCContainer
    {
        ContainerBuilder Register(ContainerBuilder containerBuilder);
    }
}
