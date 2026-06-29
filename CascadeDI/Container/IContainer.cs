using CascadeDI.Builder;

namespace CascadeDI.Container
{
    public interface IContainer
    {
        IScope CreateScope();
        IContainerBuilder CreateChildBuilder();
    }
}

