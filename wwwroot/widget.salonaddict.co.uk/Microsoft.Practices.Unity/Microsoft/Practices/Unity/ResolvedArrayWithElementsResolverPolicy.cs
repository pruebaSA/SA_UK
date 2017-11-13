namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity.Utility;
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public class ResolvedArrayWithElementsResolverPolicy : IDependencyResolverPolicy, IBuilderPolicy
    {
        private readonly IDependencyResolverPolicy[] elementPolicies;
        private readonly Resolver resolver;

        public ResolvedArrayWithElementsResolverPolicy(Type elementType, params IDependencyResolverPolicy[] elementPolicies)
        {
            Guard.ArgumentNotNull(elementType, "elementType");
            MethodInfo method = typeof(ResolvedArrayWithElementsResolverPolicy).GetMethod("DoResolve", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly).MakeGenericMethod(new Type[] { elementType });
            this.resolver = (Resolver) Delegate.CreateDelegate(typeof(Resolver), method);
            this.elementPolicies = elementPolicies;
        }

        private static object DoResolve<T>(IBuilderContext context, IDependencyResolverPolicy[] elementPolicies)
        {
            T[] localArray = new T[elementPolicies.Length];
            for (int i = 0; i < elementPolicies.Length; i++)
            {
                localArray[i] = (T) elementPolicies[i].Resolve(context);
            }
            return localArray;
        }

        public object Resolve(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            return this.resolver(context, this.elementPolicies);
        }

        private delegate object Resolver(IBuilderContext context, IDependencyResolverPolicy[] elementPolicies);
    }
}

