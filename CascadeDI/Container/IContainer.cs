using CascadeDI.Descriptors;

namespace CascadeDI.Container
{
    public interface IContainer
    {
        void Register(ServiceDescriptor descriptor);
        IScope CreateScope();
        IContainer CreateChild();
    }
}

