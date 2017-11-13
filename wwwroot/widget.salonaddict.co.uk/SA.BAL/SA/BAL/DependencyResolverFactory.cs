namespace SA.BAL
{
    using System;

    public sealed class DependencyResolverFactory : IDependencyResolverFactory
    {
        private readonly Type _resolverType;

        public DependencyResolverFactory(string resolverTypeName)
        {
            if (string.IsNullOrEmpty(resolverTypeName))
            {
                throw new ArgumentException($"DependencyResolverFactory(string resolverTypeName): {"resolverTypeName"}");
            }
            this._resolverType = Type.GetType(resolverTypeName, true, true);
        }

        public DependencyResolverFactory(Type resolverType)
        {
            this._resolverType = resolverType;
        }

        public IDependencyResolver CreateInstance() => 
            (Activator.CreateInstance(this._resolverType) as IDependencyResolver);
    }
}

