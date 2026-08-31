using System;
using System.Collections.Generic;
using CascadeDI.Data;
using CascadeDI.Descriptors.Impl;

namespace CascadeDI.Container
{
    internal sealed class RegistrationBuilder<TImplementation> : IRegistrationBuilder<TImplementation>
    {
        private readonly IContainer _container;
        private readonly Type _implementationType;
        private readonly HashSet<Type> _serviceTypes;
        private Action<object> _onCreate;

        public RegistrationBuilder(IContainer container, Type implementationType)
        {
            if (implementationType == null) throw new ArgumentNullException(nameof(implementationType));
            if (implementationType.IsInterface || implementationType.IsAbstract)
                throw new ArgumentException(
                    $"'{implementationType}' must be a concrete class.", nameof(implementationType));

            _container = container ?? throw new ArgumentNullException(nameof(container));
            _implementationType = implementationType;
            _serviceTypes = new HashSet<Type> { implementationType };
        }

        public IRegistrationBuilder<TImplementation> ToInterface<TInterface>() => ToInterface(typeof(TInterface));

        public IRegistrationBuilder<TImplementation> ToInterface(Type interfaceType)
        {
            if (interfaceType == null) throw new ArgumentNullException(nameof(interfaceType));
            if (!interfaceType.IsInterface)
                throw new ArgumentException($"'{interfaceType}' is not an interface.", nameof(interfaceType));
            if (!interfaceType.IsAssignableFrom(_implementationType))
                throw new ArgumentException(
                    $"'{_implementationType}' does not implement '{interfaceType}'.", nameof(interfaceType));

            _serviceTypes.Add(interfaceType);
            return this;
        }

        public IRegistrationBuilder<TImplementation> OnCreate(Action<TImplementation> callback)
        {
            _onCreate = callback == null ? null : instance => callback((TImplementation)instance);
            return this;
        }

        public IContainer AsSingleton() => Finish(Lifetime.Singleton);
        public IContainer AsTransient() => Finish(Lifetime.Transient);
        public IContainer AsScoped() => Finish(Lifetime.Scoped);

        private IContainer Finish(Lifetime lifetime)
        {
            foreach (var serviceType in _serviceTypes)
            {
                _container.Register(new TypeBasedServiceDescriptor
                {
                    ServiceType = serviceType,
                    ImplementationType = _implementationType,
                    Lifetime = lifetime,
                    OnCreate = _onCreate
                });
            }

            return _container;
        }
    }
}
