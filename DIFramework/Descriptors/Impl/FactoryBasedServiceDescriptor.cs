using System;

namespace DIFramework.Descriptors.Impl
{
    public class FactoryBasedServiceDescriptor : ServiceDescriptor
    {
        public Func<IScope, object> Factory { get; set; }
    }
}

