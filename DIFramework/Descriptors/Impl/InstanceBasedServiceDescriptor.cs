using System;
using DIFramework.Data;

namespace DIFramework.Descriptors.Impl
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

