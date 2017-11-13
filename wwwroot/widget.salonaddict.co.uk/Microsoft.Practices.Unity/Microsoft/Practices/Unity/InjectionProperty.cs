namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity.ObjectBuilder;
    using Microsoft.Practices.Unity.Properties;
    using Microsoft.Practices.Unity.Utility;
    using System;
    using System.Globalization;
    using System.Reflection;

    public class InjectionProperty : InjectionMember
    {
        private InjectionParameterValue parameterValue;
        private readonly string propertyName;

        public InjectionProperty(string propertyName)
        {
            this.propertyName = propertyName;
        }

        public InjectionProperty(string propertyName, object propertyValue)
        {
            this.propertyName = propertyName;
            this.parameterValue = InjectionParameterValue.ToParameter(propertyValue);
        }

        public override void AddPolicies(Type serviceType, Type implementationType, string name, IPolicyList policies)
        {
            Guard.ArgumentNotNull(implementationType, "implementationType");
            PropertyInfo property = implementationType.GetProperty(this.propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            GuardPropertyExists(property, implementationType, this.propertyName);
            GuardPropertyIsSettable(property);
            GuardPropertyIsNotIndexer(property);
            this.InitializeParameterValue(property);
            GuardPropertyValueIsCompatible(property, this.parameterValue);
            GetSelectorPolicy(policies, implementationType, name).AddPropertyAndValue(property, this.parameterValue);
        }

        private static string ExceptionMessage(string format, params object[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is Type)
                {
                    args[i] = ((Type) args[i]).Name;
                }
            }
            return string.Format(CultureInfo.CurrentCulture, format, args);
        }

        private static SpecifiedPropertiesSelectorPolicy GetSelectorPolicy(IPolicyList policies, Type typeToInject, string name)
        {
            NamedTypeBuildKey buildKey = new NamedTypeBuildKey(typeToInject, name);
            IPropertySelectorPolicy policy = policies.GetNoDefault<IPropertySelectorPolicy>(buildKey, false);
            if (!((policy != null) && (policy is SpecifiedPropertiesSelectorPolicy)))
            {
                policy = new SpecifiedPropertiesSelectorPolicy();
                policies.Set<IPropertySelectorPolicy>(policy, buildKey);
            }
            return (SpecifiedPropertiesSelectorPolicy) policy;
        }

        private static void GuardPropertyExists(PropertyInfo propInfo, Type typeToCreate, string propertyName)
        {
            if (propInfo == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.NoSuchProperty, new object[] { typeToCreate.Name, propertyName }));
            }
        }

        private static void GuardPropertyIsNotIndexer(PropertyInfo property)
        {
            if (property.GetIndexParameters().Length > 0)
            {
                throw new InvalidOperationException(ExceptionMessage(Resources.CannotInjectIndexer, new object[] { property.Name, property.DeclaringType }));
            }
        }

        private static void GuardPropertyIsSettable(PropertyInfo propInfo)
        {
            if (!propInfo.CanWrite)
            {
                throw new InvalidOperationException(ExceptionMessage(Resources.PropertyNotSettable, new object[] { propInfo.Name, propInfo.DeclaringType }));
            }
        }

        private static void GuardPropertyValueIsCompatible(PropertyInfo property, InjectionParameterValue value)
        {
            if (!value.MatchesType(property.PropertyType))
            {
                throw new InvalidOperationException(ExceptionMessage(Resources.PropertyTypeMismatch, new object[] { property.Name, property.DeclaringType, property.PropertyType, value.ParameterTypeName }));
            }
        }

        private InjectionParameterValue InitializeParameterValue(PropertyInfo propInfo)
        {
            if (this.parameterValue == null)
            {
                this.parameterValue = new ResolvedParameter(propInfo.PropertyType);
            }
            return this.parameterValue;
        }
    }
}

