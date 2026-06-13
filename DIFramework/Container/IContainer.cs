using DIFramework.Builder;

namespace DIFramework.Container
{
    public interface IContainer
    {
        IScope CreateScope();
        IContainerBuilder CreateChildBuilder();
    }
}

