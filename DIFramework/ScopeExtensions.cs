namespace DIFramework
{
    public static class ScopeExtensions
    {
        public static T Resolve<T>(this IScope scope, bool includeParent = true)
        {
            return (T)scope.Resolve(typeof(T), includeParent);
        }
    }
}


