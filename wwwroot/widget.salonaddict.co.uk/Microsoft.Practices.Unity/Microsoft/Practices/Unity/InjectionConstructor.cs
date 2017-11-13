namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity.ObjectBuilder;
    using Microsoft.Practices.Unity.Properties;
    using Microsoft.Practices.Unity.Utility;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    public class InjectionConstructor : InjectionMember
    {
        private readonly List<InjectionParameterValue> parameterValues;

        public InjectionConstructor(params object[] parameterValues)
        {
            this.parameterValues = InjectionParameterValue.ToParameters(parameterValues).ToList<InjectionParameterValue>();
        }

        public override void AddPolicies(Type serviceType, Type implementationType, string name, IPolicyList policies)
        {
            ConstructorInfo ctor = this.FindConstructor(implementationType);
            policies.Set<IConstructorSelectorPolicy>(new SpecifiedConstructorSelectorPolicy(ctor, this.parameterValues.ToArray()), new NamedTypeBuildKey(implementationType, name));
        }

        private ConstructorInfo FindConstructor(Type typeToCreate)
        {
            ParameterMatcher matcher = new ParameterMatcher(this.parameterValues);
            foreach (ConstructorInfo info in typeToCreate.GetConstructors())
            {
                if (matcher.Matches(info.GetParameters()))
                {
                    return info;
                }
            }
            string str = string.Join(", ", (from p in this.parameterValues select p.ParameterTypeName).ToArray<string>());
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.NoSuchConstructor, new object[] { typeToCreate.FullName, str }));
        }
    }
}

