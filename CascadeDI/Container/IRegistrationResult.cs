using System;

namespace CascadeDI.Container
{
    public interface IRegistrationResult<TImplementation> : IContainer
    {
        IRegistrationResult<TImplementation> OnCreate(Action<TImplementation> callback);
    }
}
