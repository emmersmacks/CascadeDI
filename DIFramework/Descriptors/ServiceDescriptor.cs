using System;
using DIFramework.Data;

namespace DIFramework.Descriptors
{
    public abstract class ServiceDescriptor
    {
        public Type ServiceType { get; set; }
        public Lifetime Lifetime { get; set; }
    }
}

