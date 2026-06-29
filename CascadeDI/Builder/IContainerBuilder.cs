using CascadeDI.Container;
using CascadeDI.Descriptors;

namespace CascadeDI.Builder
{
    public interface IContainerBuilder
    {
        void Register(ServiceDescriptor descriptor);
        IContainer Build();
    }
}

