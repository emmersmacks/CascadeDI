using System;
using CascadeDI.Data;

namespace CascadeDI.Descriptors
{
    public abstract class ServiceDescriptor
    {
        public Type ServiceType { get; set; }
        public Lifetime Lifetime { get; set; }
    }
}

