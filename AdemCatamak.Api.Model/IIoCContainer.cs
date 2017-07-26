using Autofac;

namespace ReadyApi.Model
{
    public interface IIoCContainer
    {
        ContainerBuilder Register(ContainerBuilder containerBuilder);
    }
}
