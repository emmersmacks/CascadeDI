using DIFramework.Container;
using DIFramework.Descriptors;

namespace DIFramework.Builder
{
    public interface IContainerBuilder
    {
        void Register(ServiceDescriptor descriptor);
        IContainer Build();
    }
}

