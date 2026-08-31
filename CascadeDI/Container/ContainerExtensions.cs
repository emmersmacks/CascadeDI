using System;
using CascadeDI.Data;
using CascadeDI.Descriptors.Impl;

namespace CascadeDI.Container
{
    public static class ContainerExtensions
    {
        private static IContainer RegisterType(this IContainer container, Type service, Type implementation,
            Lifetime lifetime, Action<object> onCreate = null)
        {
            var descriptor = new TypeBasedServiceDescriptor()
            {
                ImplementationType = implementation,
                Lifetime = lifetime,
                ServiceType = service,
                OnCreate = onCreate
            };
            container.Register(descriptor);
            return container;
        }

        private static IContainer RegisterFactory(this IContainer container, Type type,
            Func<IScope, object> factory, Lifetime lifetime, Action<object> onCreate = null)
        {
            var descriptor = new FactoryBasedServiceDescriptor()
            {
                Factory = factory,
                Lifetime = lifetime,
                ServiceType = type,
                OnCreate = onCreate
            };
            container.Register(descriptor);
            return container;
        }

        private static IContainer RegisterInstance(this IContainer container, Type type, object instance,
            Action<object> onCreate = null)
        {
            var descriptor = new InstanceBasedServiceDescriptor(type, instance)
            {
                OnCreate = onCreate
            };
            container.Register(descriptor);
            return container;
        }

        #region ByType

        public static IContainer RegisterSingleton(
            this IContainer container,
            Type @serviceInterface,
            Type serviceImplementation,
            Action<object> onCreate = null)
            => container.RegisterType(serviceInterface, serviceImplementation, Lifetime.Singleton, onCreate);

        public static IContainer RegisterSingletonFor(this IContainer container, Type implementation,
            Action<object> onCreate, params Type[] serviceTypes)
        {
            if (implementation == null) throw new ArgumentNullException(nameof(implementation));
            if (serviceTypes == null || serviceTypes.Length == 0)
                throw new ArgumentException("At least one service type must be provided", nameof(serviceTypes));

            foreach (var service in serviceTypes)
                container.RegisterType(service, implementation, Lifetime.Singleton, onCreate);

            return container;
        }

        public static IContainer RegisterSingletonFor(this IContainer container, Type implementation, params Type[] serviceTypes)
            => container.RegisterSingletonFor(implementation, null, serviceTypes);

        public static IContainer RegisterTransient(
            this IContainer container,
            Type @serviceInterface,
            Type serviceImplementation,
            Action<object> onCreate = null)
            => container.RegisterType(serviceInterface, serviceImplementation, Lifetime.Transient, onCreate);

        public static IContainer RegisterTransientFor(this IContainer container, Type implementation,
            Action<object> onCreate, params Type[] serviceTypes)
        {
            if (implementation == null) throw new ArgumentNullException(nameof(implementation));
            if (serviceTypes == null || serviceTypes.Length == 0)
                throw new ArgumentException("At least one service type must be provided", nameof(serviceTypes));

            foreach (var service in serviceTypes)
                container.RegisterType(service, implementation, Lifetime.Transient, onCreate);

            return container;
        }

        public static IContainer RegisterTransientFor(this IContainer container, Type implementation, params Type[] serviceTypes)
            => container.RegisterTransientFor(implementation, null, serviceTypes);

        public static IContainer RegisterScoped(
            this IContainer container,
            Type @serviceInterface,
            Type serviceImplementation,
            Action<object> onCreate = null)
            => container.RegisterType(serviceInterface, serviceImplementation, Lifetime.Scoped, onCreate);

        #endregion

        #region ByTypeGeneric

        public static IContainer RegisterSingleton<TInterface, TImplementation>(
            this IContainer container, Action<TImplementation> onCreate = null)
            => container.RegisterType(typeof(TInterface), typeof(TImplementation), Lifetime.Singleton, Wrap(onCreate));

        public static IContainer RegisterTransient<TInterface, TImplementation>(
            this IContainer container, Action<TImplementation> onCreate = null)
            => container.RegisterType(typeof(TInterface), typeof(TImplementation), Lifetime.Transient, Wrap(onCreate));

        public static IContainer RegisterScoped<TInterface, TImplementation>(
            this IContainer container, Action<TImplementation> onCreate = null)
            => container.RegisterType(typeof(TInterface), typeof(TImplementation), Lifetime.Scoped, Wrap(onCreate));

        public static IContainer RegisterSingleton<TImplementation>(this IContainer container, Action<TImplementation> onCreate = null)
            => container.RegisterType(typeof(TImplementation), typeof(TImplementation), Lifetime.Singleton, Wrap(onCreate));

        public static IContainer RegisterSingletonFor<TImplementation>(this IContainer container, Action<TImplementation> onCreate, params Type[] serviceTypes)
            => container.RegisterSingletonFor(typeof(TImplementation), Wrap(onCreate), serviceTypes);

        public static IContainer RegisterSingletonFor<TImplementation>(this IContainer container, params Type[] serviceTypes)
            => container.RegisterSingletonFor(typeof(TImplementation), null, serviceTypes);

        public static IContainer RegisterTransientFor<TImplementation>(this IContainer container, Action<TImplementation> onCreate, params Type[] serviceTypes)
            => container.RegisterTransientFor(typeof(TImplementation), Wrap(onCreate), serviceTypes);

        public static IContainer RegisterTransientFor<TImplementation>(this IContainer container, params Type[] serviceTypes)
            => container.RegisterTransientFor(typeof(TImplementation), null, serviceTypes);

        #endregion

        #region ByInstance

        public static IContainer RegisterSingleton(
            this IContainer container,
            Type type, object instance, Action<object> onCreate = null)
            => container.RegisterInstance(type, instance, onCreate);

        public static IContainer RegisterSingleton<T>(
            this IContainer container,
            object instance, Action<T> onCreate = null)
            => container.RegisterInstance(typeof(T), instance, Wrap(onCreate));

        public static IContainer RegisterSingletonFor(this IContainer container, object instance,
            Action<object> onCreate, params Type[] serviceTypes)
        {
            if (serviceTypes == null || serviceTypes.Length == 0)
                throw new ArgumentException("At least one service type must be provided", nameof(serviceTypes));

            foreach (var service in serviceTypes)
                container.RegisterInstance(service, instance, onCreate);

            return container;
        }

        public static IContainer RegisterSingletonFor(this IContainer container, object instance, params Type[] serviceTypes)
            => container.RegisterSingletonFor(instance, null, serviceTypes);

        #endregion

        private static Action<object> Wrap<T>(Action<T> onCreate)
            => onCreate == null ? null : instance => onCreate((T)instance);
    }
}
