using System;
using CascadeDI.Descriptors;

namespace CascadeDI.Container
{
    internal sealed class RegistrationResult<TImplementation> : IRegistrationResult<TImplementation>
    {
        private readonly IContainer _container;
        private readonly ServiceDescriptor[] _descriptors;

        public RegistrationResult(IContainer container, params ServiceDescriptor[] descriptors)
        {
            _container = container;
            _descriptors = descriptors;
        }

        public IRegistrationResult<TImplementation> OnCreate(Action<TImplementation> callback)
        {
            Action<object> wrapped = callback == null ? null : instance => callback((TImplementation)instance);
            foreach (var descriptor in _descriptors)
                descriptor.OnCreate = wrapped;

            return this;
        }

        public void Register(ServiceDescriptor descriptor) => _container.Register(descriptor);
        public IScope CreateScope() => _container.CreateScope();
        public IContainer CreateChild() => _container.CreateChild();
    }
}
