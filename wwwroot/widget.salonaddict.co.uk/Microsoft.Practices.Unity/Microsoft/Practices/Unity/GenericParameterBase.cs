namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity.Properties;
    using Microsoft.Practices.Unity.Utility;
    using System;
    using System.Globalization;

    public abstract class GenericParameterBase : InjectionParameterValue
    {
        private readonly string genericParameterName;
        private readonly bool isArray;
        private readonly string resolutionKey;

        protected GenericParameterBase(string genericParameterName) : this(genericParameterName, null)
        {
        }

        protected GenericParameterBase(string genericParameterName, string resolutionKey)
        {
            Guard.ArgumentNotNull(genericParameterName, "genericParameterName");
            if (genericParameterName.EndsWith("[]", StringComparison.Ordinal) || genericParameterName.EndsWith("()", StringComparison.Ordinal))
            {
                this.genericParameterName = genericParameterName.Replace("[]", "").Replace("()", "");
                this.isArray = true;
            }
            else
            {
                this.genericParameterName = genericParameterName;
                this.isArray = false;
            }
            this.resolutionKey = resolutionKey;
        }

        protected abstract IDependencyResolverPolicy DoGetResolverPolicy(Type typeToResolve, string resolutionKey);
        public override IDependencyResolverPolicy GetResolverPolicy(Type typeToBuild)
        {
            this.GuardTypeToBuildIsGeneric(typeToBuild);
            this.GuardTypeToBuildHasMatchingGenericParameter(typeToBuild);
            Type namedGenericParameter = new ReflectionHelper(typeToBuild).GetNamedGenericParameter(this.genericParameterName);
            if (this.isArray)
            {
                namedGenericParameter = namedGenericParameter.MakeArrayType();
            }
            return this.DoGetResolverPolicy(namedGenericParameter, this.resolutionKey);
        }

        private void GuardTypeToBuildHasMatchingGenericParameter(Type typeToBuild)
        {
            foreach (Type type in typeToBuild.GetGenericTypeDefinition().GetGenericArguments())
            {
                if (type.Name == this.genericParameterName)
                {
                    return;
                }
            }
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.NoMatchingGenericArgument, new object[] { typeToBuild.Name, this.genericParameterName }));
        }

        private void GuardTypeToBuildIsGeneric(Type typeToBuild)
        {
            if (!typeToBuild.IsGenericType)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.NotAGenericType, new object[] { typeToBuild.Name, this.genericParameterName }));
            }
        }

        public override bool MatchesType(Type t)
        {
            Guard.ArgumentNotNull(t, "t");
            if (!this.isArray)
            {
                return (t.IsGenericParameter && (t.Name == this.genericParameterName));
            }
            return ((t.IsArray && t.GetElementType().IsGenericParameter) && (t.GetElementType().Name == this.genericParameterName));
        }

        public override string ParameterTypeName =>
            this.genericParameterName;
    }
}

