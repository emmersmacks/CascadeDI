using System;
using CascadeDI.Data;
using CascadeDI.Descriptors;
using CascadeDI.Descriptors.Impl;

namespace CascadeDI.Container
{
    public static class ContainerExtensions
    {
        private static TypeBasedServiceDescriptor RegisterType(this IContainer container, Type service, Type implementation, Lifetime lifetime)
        {
            var descriptor = new TypeBasedServiceDescriptor()
            {
                ImplementationType = implementation,
                Lifetime = lifetime,
                ServiceType = service
            };
            container.Register(descriptor);
            return descriptor;
        }

        private static FactoryBasedServiceDescriptor RegisterFactory(this IContainer container, Type type,
            Func<IScope, object> factory, Lifetime lifetime)
        {
            var descriptor = new FactoryBasedServiceDescriptor()
            {
                Factory = factory,
                Lifetime = lifetime,
                ServiceType = type
            };
            container.Register(descriptor);
            return descriptor;
        }

        private static InstanceBasedServiceDescriptor RegisterInstance(this IContainer container, Type type, object instance)
        {
            var descriptor = new InstanceBasedServiceDescriptor(type, instance);
            container.Register(descriptor);
            return descriptor;
        }

        #region ByType

        public static IRegistrationResult<object> RegisterSingleton(
            this IContainer container,
            Type @serviceInterface,
            Type serviceImplementation)
        {
            var descriptor = container.RegisterType(serviceInterface, serviceImplementation, Lifetime.Singleton);
            return new RegistrationResult<object>(container, descriptor);
        }

        public static IRegistrationResult<object> RegisterSingletonFor(this IContainer container, Type implementation, params Type[] serviceTypes)
        {
            if (implementation == null) throw new ArgumentNullException(nameof(implementation));
            if (serviceTypes == null || serviceTypes.Length == 0)
                throw new ArgumentException("At least one service type must be provided", nameof(serviceTypes));

            var descriptors = new ServiceDescriptor[serviceTypes.Length];
            for (int i = 0; i < serviceTypes.Length; i++)
                descriptors[i] = container.RegisterType(serviceTypes[i], implementation, Lifetime.Singleton);

            return new RegistrationResult<object>(container, descriptors);
        }

        public static IRegistrationResult<object> RegisterTransient(
            this IContainer container,
            Type @serviceInterface,
            Type serviceImplementation)
        {
            var descriptor = container.RegisterType(serviceInterface, serviceImplementation, Lifetime.Transient);
            return new RegistrationResult<object>(container, descriptor);
        }

        public static IRegistrationResult<object> RegisterTransientFor(this IContainer container, Type implementation, params Type[] serviceTypes)
        {
            if (implementation == null) throw new ArgumentNullException(nameof(implementation));
            if (serviceTypes == null || serviceTypes.Length == 0)
                throw new ArgumentException("At least one service type must be provided", nameof(serviceTypes));

            var descriptors = new ServiceDescriptor[serviceTypes.Length];
            for (int i = 0; i < serviceTypes.Length; i++)
                descriptors[i] = container.RegisterType(serviceTypes[i], implementation, Lifetime.Transient);

            return new RegistrationResult<object>(container, descriptors);
        }

        public static IRegistrationResult<object> RegisterScoped(
            this IContainer container,
            Type @serviceInterface,
            Type serviceImplementation)
        {
            var descriptor = container.RegisterType(serviceInterface, serviceImplementation, Lifetime.Scoped);
            return new RegistrationResult<object>(container, descriptor);
        }

        #endregion

        #region ByTypeGeneric

        public static IRegistrationResult<TImplementation> RegisterSingleton<TInterface, TImplementation>(
            this IContainer container)
        {
            var descriptor = container.RegisterType(typeof(TInterface), typeof(TImplementation), Lifetime.Singleton);
            return new RegistrationResult<TImplementation>(container, descriptor);
        }

        public static IRegistrationResult<TImplementation> RegisterTransient<TInterface, TImplementation>(
            this IContainer container)
        {
            var descriptor = container.RegisterType(typeof(TInterface), typeof(TImplementation), Lifetime.Transient);
            return new RegistrationResult<TImplementation>(container, descriptor);
        }

        public static IRegistrationResult<TImplementation> RegisterScoped<TInterface, TImplementation>(
            this IContainer container)
        {
            var descriptor = container.RegisterType(typeof(TInterface), typeof(TImplementation), Lifetime.Scoped);
            return new RegistrationResult<TImplementation>(container, descriptor);
        }

        public static IRegistrationResult<TImplementation> RegisterSingleton<TImplementation>(this IContainer container)
        {
            var descriptor = container.RegisterType(typeof(TImplementation), typeof(TImplementation), Lifetime.Singleton);
            return new RegistrationResult<TImplementation>(container, descriptor);
        }

        public static IRegistrationResult<TImplementation> RegisterSingletonFor<TImplementation>(this IContainer container, params Type[] serviceTypes)
        {
            if (serviceTypes == null || serviceTypes.Length == 0)
                throw new ArgumentException("At least one service type must be provided", nameof(serviceTypes));

            var implementation = typeof(TImplementation);
            var descriptors = new ServiceDescriptor[serviceTypes.Length];
            for (int i = 0; i < serviceTypes.Length; i++)
                descriptors[i] = container.RegisterType(serviceTypes[i], implementation, Lifetime.Singleton);

            return new RegistrationResult<TImplementation>(container, descriptors);
        }

        public static IRegistrationResult<TImplementation> RegisterTransientFor<TImplementation>(this IContainer container, params Type[] serviceTypes)
        {
            if (serviceTypes == null || serviceTypes.Length == 0)
                throw new ArgumentException("At least one service type must be provided", nameof(serviceTypes));

            var implementation = typeof(TImplementation);
            var descriptors = new ServiceDescriptor[serviceTypes.Length];
            for (int i = 0; i < serviceTypes.Length; i++)
                descriptors[i] = container.RegisterType(serviceTypes[i], implementation, Lifetime.Transient);

            return new RegistrationResult<TImplementation>(container, descriptors);
        }

        #endregion

        #region Fluent

        public static IRegistrationBuilder<TImplementation> Register<TImplementation>(this IContainer container)
            => new RegistrationBuilder<TImplementation>(container, typeof(TImplementation));

        public static IRegistrationBuilder<object> Register(this IContainer container, Type implementationType)
            => new RegistrationBuilder<object>(container, implementationType);

        #endregion

        #region ByInstance

        public static IRegistrationResult<object> RegisterSingleton(
            this IContainer container,
            Type type, object instance)
        {
            var descriptor = container.RegisterInstance(type, instance);
            return new RegistrationResult<object>(container, descriptor);
        }

        public static IRegistrationResult<T> RegisterSingleton<T>(
            this IContainer container,
            object instance)
        {
            var descriptor = container.RegisterInstance(typeof(T), instance);
            return new RegistrationResult<T>(container, descriptor);
        }

        public static IRegistrationResult<object> RegisterSingletonFor(this IContainer container, object instance, params Type[] serviceTypes)
        {
            if (serviceTypes == null || serviceTypes.Length == 0)
                throw new ArgumentException("At least one service type must be provided", nameof(serviceTypes));

            var descriptors = new ServiceDescriptor[serviceTypes.Length];
            for (int i = 0; i < serviceTypes.Length; i++)
                descriptors[i] = container.RegisterInstance(serviceTypes[i], instance);

            return new RegistrationResult<object>(container, descriptors);
        }

        #endregion
    }
}
