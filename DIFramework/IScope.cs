using System;

namespace DIFramework
{
    public interface IScope 
    {
        object Resolve(Type service, bool includeParent = true);
    }
}