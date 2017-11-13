namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity.Properties;
    using Microsoft.Practices.Unity.Utility;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    public class GenericResolvedArrayParameter : InjectionParameterValue
    {
        private readonly List<InjectionParameterValue> elementValues = new List<InjectionParameterValue>();
        private readonly string genericParameterName;

        public GenericResolvedArrayParameter(string genericParameterName, params object[] elementValues)
        {
            Guard.ArgumentNotNull(genericParameterName, "genericParameterName");
            this.genericParameterName = genericParameterName;
            this.elementValues.AddRange(InjectionParameterValue.ToParameters(elementValues));
        }

        public override IDependencyResolverPolicy GetResolverPolicy(Type typeToBuild)
        {
            this.GuardTypeToBuildIsGeneric(typeToBuild);
            this.GuardTypeToBuildHasMatchingGenericParameter(typeToBuild);
            Type namedGenericParameter = new ReflectionHelper(typeToBuild).GetNamedGenericParameter(this.genericParameterName);
            List<IDependencyResolverPolicy> list = new List<IDependencyResolverPolicy>();
            foreach (InjectionParameterValue value2 in this.elementValues)
            {
                list.Add(value2.GetResolverPolicy(typeToBuild));
            }
            return new ResolvedArrayWithElementsResolverPolicy(namedGenericParameter, list.ToArray());
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
            if (!(t.IsArray && (t.GetArrayRank() == 1)))
            {
                return false;
            }
            Type elementType = t.GetElementType();
            return (elementType.IsGenericParameter && (elementType.Name == this.genericParameterName));
        }

        public override string ParameterTypeName =>
            (this.genericParameterName + "[]");
    }
}

