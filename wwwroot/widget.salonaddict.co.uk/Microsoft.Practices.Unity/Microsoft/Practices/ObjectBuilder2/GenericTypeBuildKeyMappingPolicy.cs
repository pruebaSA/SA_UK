namespace Microsoft.Practices.ObjectBuilder2
{
    using Microsoft.Practices.Unity.Properties;
    using Microsoft.Practices.Unity.Utility;
    using System;
    using System.Globalization;

    public class GenericTypeBuildKeyMappingPolicy : IBuildKeyMappingPolicy, IBuilderPolicy
    {
        private readonly NamedTypeBuildKey destinationKey;

        public GenericTypeBuildKeyMappingPolicy(NamedTypeBuildKey destinationKey)
        {
            Guard.ArgumentNotNull(destinationKey, "destinationKey");
            if (!destinationKey.Type.IsGenericTypeDefinition)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.MustHaveOpenGenericType, new object[] { destinationKey.Type.Name }));
            }
            this.destinationKey = destinationKey;
        }

        private void GuardSameNumberOfGenericArguments(Type sourceType)
        {
            if (sourceType.GetGenericArguments().Length != this.DestinationType.GetGenericArguments().Length)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.MustHaveSameNumberOfGenericArguments, new object[] { sourceType.Name, this.DestinationType.Name }), "sourceType");
            }
        }

        public NamedTypeBuildKey Map(NamedTypeBuildKey buildKey, IBuilderContext context)
        {
            Guard.ArgumentNotNull(buildKey, "buildKey");
            Type sourceType = buildKey.Type;
            this.GuardSameNumberOfGenericArguments(sourceType);
            Type[] genericArguments = sourceType.GetGenericArguments();
            return new NamedTypeBuildKey(this.destinationKey.Type.MakeGenericType(genericArguments), this.destinationKey.Name);
        }

        private Type DestinationType =>
            this.destinationKey.Type;
    }
}

