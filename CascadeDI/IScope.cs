using System;

namespace CascadeDI
{
    public interface IScope 
    {
        object Resolve(Type service, bool collectParentDescriptors = true);
    }
}