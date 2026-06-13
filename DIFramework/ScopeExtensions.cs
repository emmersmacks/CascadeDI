namespace DIFramework
{
    public static class ScopeExtensions
    {
        public static T Resolve<T>(this IScope scope)
        {
            return (T)scope.Resolve(typeof(T));
        }
    }
}


