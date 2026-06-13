using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections;
using DIFramework.Builder;
using DIFramework.Descriptors;
using DIFramework.Descriptors.Impl;

namespace DIFramework.Container
{
    public class Container : IContainer
    {
        private class Scope : IScope
        {
            private readonly Container _container;
            
            public Scope(Container container)
            {
                _container = container;
            }
    
            public object Resolve(Type service) 
                => _container.CreateInstance(service, this);
        }
    
        internal object CreateInstance(Type service, IScope scope)
        {
            // handle collection types like IEnumerable<T>, List<T>, IList<T>, ICollection<T>
            if (service.IsGenericType)
            {
                var genDef = service.GetGenericTypeDefinition();
                var arg = service.GetGenericArguments()[0];

                if (genDef == typeof(System.Collections.Generic.IEnumerable<>)
                    || genDef == typeof(System.Collections.Generic.List<>)
                    || genDef == typeof(System.Collections.Generic.IList<>)
                    || genDef == typeof(System.Collections.Generic.ICollection<>))
                {
                    var descriptors = GetDescriptorsFor(arg);

                    var listType = typeof(System.Collections.Generic.List<>).MakeGenericType(arg);
                    var list = (IList)Activator.CreateInstance(listType);

                    foreach (var d in descriptors)
                    {
                        object inst;
                        if (d is InstanceBasedServiceDescriptor id)
                            inst = id.Instance;
                        else if (d is FactoryBasedServiceDescriptor fd)
                            inst = fd.Factory(scope);
                        else
                        {
                            var td = d as TypeBasedServiceDescriptor;
                            var impl = td.ImplementationType;
                            var ctors = impl.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
                            if (ctors.Length == 0)
                                inst = Activator.CreateInstance(impl);
                            else
                            {
                                var ctor = ctors.Single();
                                var ps = ctor.GetParameters();
                                var args = new object[ps.Length];
                                for (int i = 0; i < ps.Length; i++)
                                    args[i] = CreateInstance(ps[i].ParameterType, scope);
                                inst = ctor.Invoke(args);
                            }
                        }

                        list.Add(inst);
                    }

                    return list;
                }
            }

            var descriptorsForType = GetDescriptorsFor(service);
            if (descriptorsForType == null || descriptorsForType.Count == 0)
            {
                if (_parent != null)
                    return _parent.CreateInstance(service, scope);

                throw new InvalidOperationException();
            }

            // resolve single service - pick last registered (highest precedence)
            var descriptor = descriptorsForType[descriptorsForType.Count - 1];

            if (descriptor is InstanceBasedServiceDescriptor instanceDescriptor)
                return instanceDescriptor.Instance;
            if (descriptor is FactoryBasedServiceDescriptor factoryDescriptor)
                return factoryDescriptor.Factory(scope);

            var typeDescriptor = descriptor as TypeBasedServiceDescriptor;
            var implementation = typeDescriptor.ImplementationType;

            var constructors = implementation.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            if (constructors.Length == 0)
                return Activator.CreateInstance(implementation);

            var constructor = constructors.Single();
            var parameters = constructor.GetParameters();
            var argsForConstructor = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                argsForConstructor[i] = CreateInstance(parameters[i].ParameterType, scope);
            }

            return constructor.Invoke(argsForConstructor);
        }
    
        private Dictionary<Type, List<ServiceDescriptor>> _descriptors;
        private readonly Container _parent;
    
        public Container(IEnumerable<ServiceDescriptor> descriptors, Container parent = null)
        {
            _descriptors = descriptors
                .GroupBy(x => x.ServiceType)
                .ToDictionary(g => g.Key, g => g.ToList());
            _parent = parent;
        }
        
        public IScope CreateScope()
        {
            return new Scope(this);
        }

        public IContainer CreateChild(IEnumerable<ServiceDescriptor> descriptors = null)
        {
            var list = descriptors == null ? new List<ServiceDescriptor>() : new List<ServiceDescriptor>(descriptors);
            return new Container(list, this);
        }

        public IContainerBuilder CreateChildBuilder()
        {
            return new ContainerBuilder(this);
        }

        // returns descriptors for a service type from this container and parents (this first)
        internal List<ServiceDescriptor> GetDescriptorsFor(Type serviceType)
        {
            var result = new List<ServiceDescriptor>();
            if (_descriptors != null && _descriptors.TryGetValue(serviceType, out var list))
                result.AddRange(list);
            if (_parent != null)
                result.AddRange(_parent.GetDescriptorsFor(serviceType));
            return result;
        }
    }
}

