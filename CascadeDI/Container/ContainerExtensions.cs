using System;
using CascadeDI.Data;
using CascadeDI.Descriptors.Impl;

namespace CascadeDI.Container
{
    public static class ContainerExtensions
    {
        private static IContainer RegisterType(this IContainer container, Type service, Type implementation, Lifetime lifetime)
        {
            var descriptor = new TypeBasedServiceDescriptor()
            {
                ImplementationType = implementation,
                Lifetime = lifetime,
                ServiceType = service
            };
            container.Register(descriptor);
            return container;
        }

        private static IContainer RegisterFactory(this IContainer container, Type type,
            Func<IScope, object> factory, Lifetime lifetime)
        {
            var descriptor = new FactoryBasedServiceDescriptor()
            {
                Factory = factory,
                Lifetime = lifetime,
                ServiceType = type
            };
            container.Register(descriptor);
            return container;
        }

        private static IContainer RegisterInstance(this IContainer container, Type type, object instance)
        {
            var descriptor = new InstanceBasedServiceDescriptor(type, instance);
            container.Register(descriptor);
            return container;
        }

        #region ByType

        public static IContainer RegisterSingleton(
            this IContainer container,
            Type @serviceInterface,
            Type serviceImplementation)
            => container.RegisterType(serviceInterface, serviceImplementation, Lifetime.Singleton);

        public static IContainer RegisterSingletonFor(this IContainer container, Type implementation, params Type[] serviceTypes)
        {
            if (implementation == null) throw new ArgumentNullException(nameof(implementation));
            if (serviceTypes == null || serviceTypes.Length == 0)
                throw new ArgumentException("At least one service type must be provided", nameof(serviceTypes));

            foreach (var service in serviceTypes)
                container.RegisterType(service, implementation, Lifetime.Singleton);

            return container;
        }

        public static IContainer RegisterTransient(
            this IContainer container,
            Type @serviceInterface,
            Type serviceImplementation)
            => container.RegisterType(serviceInterface, serviceImplementation, Lifetime.Transient);

        public static IContainer RegisterTransientFor(this IContainer container, Type implementation, params Type[] serviceTypes)
        {
            if (implementation == null) throw new ArgumentNullException(nameof(implementation));
            if (serviceTypes == null || serviceTypes.Length == 0)
                throw new ArgumentException("At least one service type must be provided", nameof(serviceTypes));

            foreach (var service in serviceTypes)
                container.RegisterType(service, implementation, Lifetime.Transient);

            return container;
        }

        public static IContainer RegisterScoped(
            this IContainer container,
            Type @serviceInterface,
            Type serviceImplementation)
            => container.RegisterType(serviceInterface, serviceImplementation, Lifetime.Scoped);

        #endregion

        #region ByTypeGeneric

        public static IContainer RegisterSingleton<TInterface, TImplementation>(
            this IContainer container)
            => container.RegisterType(typeof(TInterface), typeof(TImplementation), Lifetime.Singleton);

        public static IContainer RegisterTransient<TInterface, TImplementation>(
            this IContainer container)
            => container.RegisterType(typeof(TInterface), typeof(TImplementation), Lifetime.Transient);

        public static IContainer RegisterScoped<TInterface, TImplementation>(
            this IContainer container)
            => container.RegisterType(typeof(TInterface), typeof(TImplementation), Lifetime.Scoped);

        public static IContainer RegisterSingleton<TImplementation>(this IContainer container)
            => container.RegisterType(typeof(TImplementation), typeof(TImplementation), Lifetime.Singleton);

        public static IContainer RegisterSingletonFor<TImplementation>(this IContainer container, params Type[] serviceTypes)
            => container.RegisterSingletonFor(typeof(TImplementation), serviceTypes);

        public static IContainer RegisterTransientFor<TImplementation>(this IContainer container, params Type[] serviceTypes)
            => container.RegisterTransientFor(typeof(TImplementation), serviceTypes);

        #endregion

        #region ByInstance

        public static IContainer RegisterSingleton(
            this IContainer container,
            Type type, object instance)
            => container.RegisterInstance(type, instance);

        public static IContainer RegisterSingleton<T>(
            this IContainer container,
            object instance)
            => container.RegisterInstance(typeof(T), instance);

        public static IContainer RegisterSingletonFor(this IContainer container, object instance, params Type[] serviceTypes)
        {
            if (serviceTypes == null || serviceTypes.Length == 0)
                throw new ArgumentException("At least one service type must be provided", nameof(serviceTypes));

            foreach (var service in serviceTypes)
                container.RegisterInstance(service, instance);

            return container;
        }

        #endregion
    }
}
