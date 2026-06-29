using System;
using CascadeDI.Data;

namespace CascadeDI.Descriptors.Impl
{
    public class InstanceBasedServiceDescriptor : ServiceDescriptor
    {
        public object Instance { get; set; }

        public InstanceBasedServiceDescriptor(Type serviceType, object instance)
        {
            Lifetime = Lifetime.Singleton;
            ServiceType = serviceType;
            Instance = instance;
        }
    }
}

