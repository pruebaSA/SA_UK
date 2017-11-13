namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity.Properties;
    using Microsoft.Practices.Unity.Utility;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    public class ResolvedArrayParameter : TypedInjectionValue
    {
        private readonly Type elementType;
        private readonly List<InjectionParameterValue> elementValues;

        public ResolvedArrayParameter(Type elementType, params object[] elementValues) : this(GetArrayType(elementType), elementType, elementValues)
        {
        }

        protected ResolvedArrayParameter(Type arrayParameterType, Type elementType, params object[] elementValues) : base(arrayParameterType)
        {
            this.elementValues = new List<InjectionParameterValue>();
            Guard.ArgumentNotNull(elementType, "elementType");
            Guard.ArgumentNotNull(elementValues, "elementValues");
            this.elementType = elementType;
            this.elementValues.AddRange(InjectionParameterValue.ToParameters(elementValues));
            foreach (InjectionParameterValue value2 in this.elementValues)
            {
                if (!value2.MatchesType(elementType))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.TypesAreNotAssignable, new object[] { elementType, value2.ParameterTypeName }));
                }
            }
        }

        private static Type GetArrayType(Type elementType)
        {
            Guard.ArgumentNotNull(elementType, "elementType");
            return elementType.MakeArrayType();
        }

        public override IDependencyResolverPolicy GetResolverPolicy(Type typeToBuild)
        {
            Guard.ArgumentNotNull(typeToBuild, "typeToBuild");
            List<IDependencyResolverPolicy> list = new List<IDependencyResolverPolicy>();
            foreach (InjectionParameterValue value2 in this.elementValues)
            {
                list.Add(value2.GetResolverPolicy(this.elementType));
            }
            return new ResolvedArrayWithElementsResolverPolicy(this.elementType, list.ToArray());
        }
    }
}

