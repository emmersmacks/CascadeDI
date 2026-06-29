using System.Collections.Generic;
using CascadeDI.Container;
using CascadeDI.Descriptors;

namespace CascadeDI.Builder
{
    public class ContainerBuilder : IContainerBuilder
    {
        private readonly List<ServiceDescriptor> _descriptors = new ();
        private readonly Container.Container _parent;

        public ContainerBuilder(Container.Container parent = null)
        {
            _parent = parent;
        }

        public void Register(ServiceDescriptor descriptor)
        {
            _descriptors.Add(descriptor);
        }

        public IContainer Build()
        {
            return new Container.Container(_descriptors, _parent);
        }
    }
}

