using System;

namespace CascadeDI.Descriptors.Impl
{
    public class TypeBasedServiceDescriptor : ServiceDescriptor
    {
        public Type ImplementationType { get; set; }
    }
}

