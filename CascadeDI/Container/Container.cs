using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CascadeDI.Descriptors;
using CascadeDI.Descriptors.Impl;

namespace CascadeDI.Container
{
    public class Container : IContainer
    {
        private class Scope : IScope
        {
            private readonly Container _container;
            private readonly Dictionary<ServiceDescriptor, object> _scopedInstances = new ();
            
            public Scope(Container container)
            {
                _container = container;
            }
    
            public object Resolve(Type service, bool collectParentDescriptors = true) 
                => _container.CreateInstance(service, this, collectParentDescriptors);
            
            internal bool TryGetScopedInstance(ServiceDescriptor descriptor, out object instance)
            {
                return _scopedInstances.TryGetValue(descriptor, out instance);
            }

            internal void SetScopedInstance(ServiceDescriptor descriptor, object instance)
            {
                _scopedInstances[descriptor] = instance;
            }
        }
        
        private static readonly HashSet<Type> CollectionGenericDefinitions = new ()
        {
            typeof(IEnumerable<>), typeof(List<>), typeof(IList<>), typeof(ICollection<>)
        };

        private readonly Dictionary<Type, List<ServiceDescriptor>> _descriptors = new ();
        private readonly Dictionary<ServiceDescriptor, object> _singletonInstances = new ();
        private readonly HashSet<ServiceDescriptor> _onCreateNotified = new ();
        private readonly Container _parent;

        public Container(Container parent = null)
        {
            _parent = parent;
        }

        public void Register(ServiceDescriptor descriptor)
        {
            if (_descriptors.TryGetValue(descriptor.ServiceType, out var existing) && existing.Count > 0)
            {
                if (!descriptor.ServiceType.IsInterface)
                    throw new InvalidOperationException(
                        $"Service type '{descriptor.ServiceType}' is already registered on this container.");

                existing.Add(descriptor);
                return;
            }

            _descriptors[descriptor.ServiceType] = new List<ServiceDescriptor> { descriptor };
        }
    
        internal object CreateInstance(Type service, IScope scope, bool collectParentDescriptors = true)
        {
            if (service.IsGenericType && CollectionGenericDefinitions.Contains(service.GetGenericTypeDefinition()))
            {
                var elementType = service.GetGenericArguments()[0];
                return CreateCollectionInstance(elementType, scope, collectParentDescriptors);
            }

            if (_descriptors.TryGetValue(service, out var localDescriptors) && localDescriptors.Count > 0)
                return ResolveInstance(localDescriptors[0], scope);

            if (_parent != null && collectParentDescriptors)
                return _parent.CreateInstance(service, scope, true);

            return null;
        }

        private object CreateCollectionInstance(Type elementType, IScope scope, bool collectParentDescriptors)
        {
            var descriptors = GetDescriptorsFor(elementType, collectParentDescriptors);

            var listType = typeof(List<>).MakeGenericType(elementType);
            var list = (IList)Activator.CreateInstance(listType);

            foreach (var descriptor in descriptors)
                list.Add(ResolveInstance(descriptor, scope));

            return list;
        }

        private object ResolveInstance(ServiceDescriptor descriptor, IScope scope)
        {
            switch (descriptor)
            {
                case InstanceBasedServiceDescriptor instanceDescriptor:
                    NotifyOnCreateOnce(instanceDescriptor, instanceDescriptor.Instance);
                    return instanceDescriptor.Instance;

                case FactoryBasedServiceDescriptor factoryDescriptor:
                    return ResolveWithLifetime(factoryDescriptor, scope, () => CreateAndNotify(factoryDescriptor, factoryDescriptor.Factory(scope)));

                case TypeBasedServiceDescriptor typeDescriptor:
                    return ResolveTypeBasedInstance(typeDescriptor, scope);

                default:
                    throw new NotSupportedException($"Unsupported descriptor type '{descriptor.GetType()}'.");
            }
        }

        private object ResolveWithLifetime(ServiceDescriptor descriptor, IScope scope, Func<object> factory)
        {
            switch (descriptor.Lifetime)
            {
                case Data.Lifetime.Singleton:
                    if (!_singletonInstances.TryGetValue(descriptor, out var singleton))
                    {
                        singleton = factory();
                        _singletonInstances[descriptor] = singleton;
                    }
                    return singleton;

                case Data.Lifetime.Scoped when scope is Scope scopedContainer:
                    if (!scopedContainer.TryGetScopedInstance(descriptor, out var scoped))
                    {
                        scoped = factory();
                        scopedContainer.SetScopedInstance(descriptor, scoped);
                    }
                    return scoped;

                default:
                    return factory();
            }
        }

        private object ResolveTypeBasedInstance(TypeBasedServiceDescriptor descriptor, IScope scope)
        {
            if (descriptor.Lifetime == Data.Lifetime.Singleton)
            {
                // Keyed by ImplementationType, not descriptor identity, so RegisterSingletonFor's
                // multiple service-type registrations for the same implementation share one instance.
                var instance = _singletonInstances
                    .Where(p => p.Key is TypeBasedServiceDescriptor d &&
                                d.ImplementationType == descriptor.ImplementationType)
                    .Select(p => p.Value)
                    .FirstOrDefault();
                if (instance == null)
                {
                    instance = CreateAndNotify(descriptor, CreateByType(descriptor.ImplementationType, scope));
                    _singletonInstances[descriptor] = instance;
                }
                return instance;
            }

            if (descriptor.Lifetime == Data.Lifetime.Scoped && scope is Scope scopedContainer)
            {
                if (!scopedContainer.TryGetScopedInstance(descriptor, out var scopedInstance))
                {
                    scopedInstance = CreateAndNotify(descriptor, CreateByType(descriptor.ImplementationType, scope));
                    scopedContainer.SetScopedInstance(descriptor, scopedInstance);
                }
                return scopedInstance;
            }

            return CreateAndNotify(descriptor, CreateByType(descriptor.ImplementationType, scope));
        }

        private object CreateAndNotify(ServiceDescriptor descriptor, object instance)
        {
            descriptor.OnCreate?.Invoke(instance);
            return instance;
        }

        private void NotifyOnCreateOnce(ServiceDescriptor descriptor, object instance)
        {
            if (descriptor.OnCreate == null)
                return;

            if (_onCreateNotified.Add(descriptor))
                descriptor.OnCreate(instance);
        }

        private object CreateByType(Type implementation, IScope scope)
        {
            var constructors = implementation.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            if (constructors.Length == 0)
                return Activator.CreateInstance(implementation);

            var constructor = constructors.Single();
            var parameters = constructor.GetParameters();
            var argsForConstructor = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType == typeof(IScope))
                {
                    argsForConstructor[i] = scope;
                }
                else if (parameters[i].ParameterType == typeof(IContainer))
                {
                    argsForConstructor[i] = this;
                }
                else
                {
                    argsForConstructor[i] = CreateInstance(parameters[i].ParameterType, scope);
                }
            }

            return constructor.Invoke(argsForConstructor);
        }
        
        public IScope CreateScope()
        {
            return new Scope(this);
        }

        public IContainer CreateChild()
        {
            return new Container(this);
        }

        internal List<ServiceDescriptor> GetDescriptorsFor(Type serviceType, bool includeParent)
        {
            var result = new List<ServiceDescriptor>();
            if (_descriptors != null && _descriptors.TryGetValue(serviceType, out var list))
                result.AddRange(list);
            if (_parent != null && includeParent)
                result.AddRange(_parent.GetDescriptorsFor(serviceType, true));
            return result;
        }
    }
}

