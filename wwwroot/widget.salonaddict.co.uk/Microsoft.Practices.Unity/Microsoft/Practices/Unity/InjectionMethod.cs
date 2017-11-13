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

    public class InjectionMethod : InjectionMember
    {
        private readonly string methodName;
        private readonly List<InjectionParameterValue> methodParameters;

        public InjectionMethod(string methodName, params object[] methodParameters)
        {
            this.methodName = methodName;
            this.methodParameters = InjectionParameterValue.ToParameters(methodParameters).ToList<InjectionParameterValue>();
        }

        public override void AddPolicies(Type serviceType, Type implementationType, string name, IPolicyList policies)
        {
            MethodInfo method = this.FindMethod(implementationType);
            this.ValidateMethodCanBeInjected(method, implementationType);
            GetSelectorPolicy(policies, implementationType, name).AddMethodAndParameters(method, this.methodParameters);
        }

        private MethodInfo FindMethod(Type typeToCreate)
        {
            ParameterMatcher matcher = new ParameterMatcher(this.methodParameters);
            foreach (MethodInfo info in typeToCreate.GetMethods())
            {
                if (this.MethodNameMatches(info, this.methodName) && matcher.Matches(info.GetParameters()))
                {
                    return info;
                }
            }
            return null;
        }

        private static SpecifiedMethodsSelectorPolicy GetSelectorPolicy(IPolicyList policies, Type typeToCreate, string name)
        {
            NamedTypeBuildKey buildKey = new NamedTypeBuildKey(typeToCreate, name);
            IMethodSelectorPolicy policy = policies.GetNoDefault<IMethodSelectorPolicy>(buildKey, false);
            if (!((policy != null) && (policy is SpecifiedMethodsSelectorPolicy)))
            {
                policy = new SpecifiedMethodsSelectorPolicy();
                policies.Set<IMethodSelectorPolicy>(policy, buildKey);
            }
            return (SpecifiedMethodsSelectorPolicy) policy;
        }

        private void GuardMethodHasNoOutParams(MethodInfo info, Type typeToCreate)
        {
            if (info.GetParameters().Any<ParameterInfo>(param => param.IsOut))
            {
                this.ThrowIllegalInjectionMethod(Resources.CannotInjectMethodWithOutParams, typeToCreate);
            }
        }

        private void GuardMethodHasNoRefParams(MethodInfo info, Type typeToCreate)
        {
            if (info.GetParameters().Any<ParameterInfo>(param => param.ParameterType.IsByRef))
            {
                this.ThrowIllegalInjectionMethod(Resources.CannotInjectMethodWithRefParams, typeToCreate);
            }
        }

        private void GuardMethodNotGeneric(MethodInfo info, Type typeToCreate)
        {
            if (info.IsGenericMethodDefinition)
            {
                this.ThrowIllegalInjectionMethod(Resources.CannotInjectGenericMethod, typeToCreate);
            }
        }

        private void GuardMethodNotNull(MethodInfo info, Type typeToCreate)
        {
            if (info == null)
            {
                this.ThrowIllegalInjectionMethod(Resources.NoSuchMethod, typeToCreate);
            }
        }

        private void GuardMethodNotStatic(MethodInfo info, Type typeToCreate)
        {
            if (info.IsStatic)
            {
                this.ThrowIllegalInjectionMethod(Resources.CannotInjectStaticMethod, typeToCreate);
            }
        }

        protected virtual bool MethodNameMatches(MemberInfo targetMethod, string nameToMatch) => 
            (targetMethod.Name == nameToMatch);

        private void ThrowIllegalInjectionMethod(string message, Type typeToCreate)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, message, new object[] { typeToCreate.Name, this.methodName, this.methodParameters.JoinStrings<InjectionParameterValue>(", ", mp => mp.ParameterTypeName) }));
        }

        private void ValidateMethodCanBeInjected(MethodInfo method, Type typeToCreate)
        {
            this.GuardMethodNotNull(method, typeToCreate);
            this.GuardMethodNotStatic(method, typeToCreate);
            this.GuardMethodNotGeneric(method, typeToCreate);
            this.GuardMethodHasNoOutParams(method, typeToCreate);
            this.GuardMethodHasNoRefParams(method, typeToCreate);
        }
    }
}

