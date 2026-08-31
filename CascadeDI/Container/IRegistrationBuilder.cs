using System;

namespace CascadeDI.Container
{
    public interface IRegistrationBuilder<TImplementation>
    {
        IRegistrationBuilder<TImplementation> ToInterface<TInterface>();
        IRegistrationBuilder<TImplementation> ToInterface(Type interfaceType);
        IRegistrationBuilder<TImplementation> OnCreate(Action<TImplementation> callback);

        IContainer AsSingleton();
        IContainer AsTransient();
        IContainer AsScoped();
    }
}
