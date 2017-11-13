namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity.Properties;
    using Microsoft.Practices.Unity.Utility;
    using System;
    using System.Globalization;

    public class OptionalDependencyResolverPolicy : IDependencyResolverPolicy, IBuilderPolicy
    {
        private readonly string name;
        private readonly Type type;

        public OptionalDependencyResolverPolicy(Type type) : this(type, null)
        {
        }

        public OptionalDependencyResolverPolicy(Type type, string name)
        {
            Guard.ArgumentNotNull(type, "type");
            if (type.IsValueType)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.OptionalDependenciesMustBeReferenceTypes, new object[] { type.Name }));
            }
            this.type = type;
            this.name = name;
        }

        public object Resolve(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            NamedTypeBuildKey newBuildKey = new NamedTypeBuildKey(this.type, this.name);
            try
            {
                return context.NewBuildUp(newBuildKey);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Type DependencyType =>
            this.type;

        public string Name =>
            this.name;
    }
}

